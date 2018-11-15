using DrawnTableControl.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DrawnTableControl.HeaderHelpers
{
    public class DayH : IHeaderCreator<DateTime, int>
    {
        public DateTime GetWeeksMonday(DateTime? dt = null)
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
        
        List<DateTime> lastResult = new List<DateTime>();
        List<DrawnTableHeader> lastHeaderResult = new List<DrawnTableHeader>();

        public List<DrawnTableHeader> LastGeneratedHeaders => lastHeaderResult;

        public DateTime First => lastResult.First();

        public DateTime Last => lastResult.Last();

        public IEnumerable<DrawnTableHeader> GenerateHeadersWeek(DateTime start, string format = "dddd", bool toTitleCase = true)
        {
            return GenerateHeaders(start, start.AddDays(6), null, format, toTitleCase);
        }

        public IEnumerable<DrawnTableHeader> GenerateHeaders(DateTime start, DateTime end, List<DayOfWeek> days = null, string format = "dddd", bool toTitleCase = true)
        {
            if (days != null && days.Count == 0)
            {
                days = null;
            }

            start = start.Date;
            end = end.Date;
            if (start > end) throw new ArgumentOutOfRangeException("\"start\" can't be bigger then \"end\"");
            
            lastResult.Clear();
            lastHeaderResult.Clear();

            DateTime dt = start;
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            while (dt.Date <= end.Date)
            {
                if (days == null || days.Contains(dt.DayOfWeek))
                {
                    lastResult.Add(dt.Date);

                    string text = dt.ToString(format);
                    if (toTitleCase)
                    {
                        text = ti.ToTitleCase(text);
                    }
                    DrawnTableHeader header = new DrawnTableHeader(text, tag: dt.Date);
                    lastHeaderResult.Add(header);
                    yield return header;
                }
                dt = dt.AddDays(1);
            }
        }

        public DateTime GetValueByIndex(int index) => lastResult[index];

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
