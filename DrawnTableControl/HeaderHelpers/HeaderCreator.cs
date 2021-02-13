using DrawnTableControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrawnTableControl.HeaderHelpers
{
    public class HeaderCreator
    {
        public DayH Day { get; } = new();
        public TimeH Time { get; } = new();
        public CustomH Custom1 { get; } = new();
        public CustomH Custom2 { get; } = new();

        public static List<DrawnTableHeader> ParseList(IEnumerable<string> collection) =>
            ParseList(collection.Select(c => new Tuple<string, object>(c, null)));

        public static List<DrawnTableHeader> ParseList(IEnumerable<Tuple<string, object>> collection) =>
            collection.Select(item => new DrawnTableHeader(item.Item1, tag: item.Item2)).ToList();

        public int GetRealIndex(List<DrawnTableHeader> headers, int headerIndex, int subheaderIndex)
        {
            if (headerIndex < 0 || headerIndex >= headers.Count || subheaderIndex < 0)
            {
                return -1;
            }

            int realIndex = 0;
            for (int i = 0; i < headers.Count; i++)
            {
                if (i == headerIndex)
                {
                    break;
                }
                if (headers[i].Subheaders.Count == 0)
                {
                    realIndex++;
                }
                else
                {
                    realIndex += headers[i].Subheaders.Count;
                }
            }
            realIndex += subheaderIndex;
            return realIndex;
        }

        public Tuple<DrawnTableHeader, DrawnTableSubheader> GetHeadersByRealIndex(List<DrawnTableHeader> headers, int realIndex)
        {
            if (realIndex < 0)
            {
                throw new IndexOutOfRangeException($"realIndex={realIndex}");
            }

            int currentRealIndex = 0;
            foreach (DrawnTableHeader header in headers)
            {
                if (header.Subheaders.Count == 0)
                {
                    if (realIndex - currentRealIndex == 0)
                    {
                        return new Tuple<DrawnTableHeader, DrawnTableSubheader>(header, null);
                    }
                    currentRealIndex++;
                }
                else
                {
                    if (realIndex - currentRealIndex < header.Subheaders.Count)
                    {
                        return new Tuple<DrawnTableHeader, DrawnTableSubheader>(header, header.Subheaders[realIndex - currentRealIndex]);
                    }
                    currentRealIndex += header.Subheaders.Count;
                }
            }

            return new Tuple<DrawnTableHeader, DrawnTableSubheader>(null, null);
        }
    }
}
