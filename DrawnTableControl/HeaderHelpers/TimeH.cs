using DrawnTableControl.ExtensionMethods;
using DrawnTableControl.Models;
using System;
using System.Collections.Generic;

namespace DrawnTableControl.HeaderHelpers
{
    public class TimeH : IHeaderCreator<TimeSpan, double>
    {
        private int tSpan = 1;
        private TimeSpan tStart, tStop, tDelta;

        public List<DrawnTableHeader> LastGeneratedHeaders { get; } = new();

        internal TimeH()
        {
        }

        public IEnumerable<DrawnTableHeader> GenerateHeaders(TimeSpan start, TimeSpan stop, TimeSpan delta, int span = 1, string format = "hh\\:mm")
        {
            tStart = start;
            tStop = stop;
            tDelta = delta;
            tSpan = span;

            while (start <= stop)
            {
                DrawnTableHeader header = new(start.ToString(format), span: span, tag: (start, stop: start.Add(delta)));
                LastGeneratedHeaders.Add(header);
                yield return header;
                start = start.Add(delta);
            }
        }

        public TimeSpan this[int index]
        {
            get
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
        }

        public double GetIndexByValue(TimeSpan time)
        {
            TimeSpan pass = time - tStart;
            double index = pass.Div(tDelta.Div(tSpan));
            return index;
        }
    }
}
