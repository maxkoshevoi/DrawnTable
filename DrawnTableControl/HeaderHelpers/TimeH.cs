using DrawnTableControl.ExtensionMethods;
using DrawnTableControl.Models;
using System;
using System.Collections.Generic;

namespace DrawnTableControl.HeaderHelpers
{
    public class TimeH : IHeaderCreator<TimeSpan, double>
    {
        int tSpan = 1;
        TimeSpan tStart, tStop, tDelta;
        readonly List<DrawnTableHeader> lastHeaderResult = new();

        public List<DrawnTableHeader> LastGeneratedHeaders => lastHeaderResult;

        public IEnumerable<DrawnTableHeader> GenerateHeaders(TimeSpan start, TimeSpan stop, TimeSpan delta, int span = 1, string format = "hh\\:mm")
        {
            tStart = start;
            tStop = stop;
            tDelta = delta;
            tSpan = span;

            while (start <= stop)
            {
                DrawnTableHeader header = new(start.ToString(format), span: span, tag: new Tuple<TimeSpan, TimeSpan>(start, start.Add(delta)));
                lastHeaderResult.Add(header);
                yield return header;
                start = start.Add(delta);
            }
        }

        public TimeSpan GetValueByIndex(int index)
        {
            TimeSpan res = tStart;
            res = res.Add(tDelta.Mult(index / tSpan));
            index -= (index / tSpan) * tSpan;
            if (index != 0)
            {
                res = res.Add(tDelta.Div(tSpan).Mult(index));
            }
            return res;
        }

        public double GetIndexByValue(TimeSpan time)
        {
            TimeSpan pass = time - tStart;
            double index = pass.Div(tDelta.Div(tSpan));
            return index;
        }
    }
}
