using DrawnTableControl.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DrawnTableControl.HeaderHelpers
{
    public class DayH : IHeaderCreator<DateTime, int>
    {
        public static DateTime GetWeeksMonday(DateTime? dt = null)
        {
            if (dt == null)
            {
                dt = DateTime.Today;
            }
            while (dt.Value.DayOfWeek != DayOfWeek.Monday)
            {
                dt = dt.Value.AddDays(-1);
            }
            return dt.Value;
        }

        readonly List<DateTime> lastResult = new();

        public List<DrawnTableHeader> LastGeneratedHeaders { get; } = new();

        public DateTime First => lastResult.First();

        public DateTime Last => lastResult.Last();

        public IEnumerable<DrawnTableHeader> GenerateHeadersWeek(DateTime start, string format = "dddd", bool toTitleCase = true)
        {
            return GenerateHeaders(start, start.AddDays(6), null, format, toTitleCase);
        }

        public IEnumerable<DrawnTableHeader> GenerateHeaders(DateTime start, DateTime end, Func<DateTime, bool> condition = null, string format = "dddd", bool toTitleCase = true)
        {
            start = start.Date;
            end = end.Date;
            if (start > end) throw new ArgumentOutOfRangeException($"\"{nameof(start)}\" can't be bigger then \"{nameof(end)}\"");

            lastResult.Clear();
            LastGeneratedHeaders.Clear();

            DateTime dt = start;
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            while (dt.Date <= end.Date)
            {
                if (condition == null || condition(dt.Date))
                {
                    lastResult.Add(dt.Date);

                    string text = dt.ToString(format);
                    if (toTitleCase)
                    {
                        text = ti.ToTitleCase(text);
                    }
                    DrawnTableHeader header = new(text, tag: dt.Date);
                    LastGeneratedHeaders.Add(header);
                    yield return header;
                }
                dt = dt.AddDays(1);
            }
        }

        public DateTime this[int index] => lastResult[index];

        public int GetIndexByValue(DateTime day) => lastResult.IndexOf(day);

        public int GetNearestIndexByDay(DateTime day, bool isSearchForward)
        {
            if (isSearchForward)
            {
                for (int i = lastResult.Count - 1; i >= 0; i--)
                {
                    if (lastResult[i] <= day)
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < lastResult.Count; i++)
                {
                    if (lastResult[i] >= day)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
    }
}
