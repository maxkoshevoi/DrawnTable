using System;

namespace DrawnTableControl.ExtensionMethods
{
    public static class TimeStampEx
    {
        public static TimeSpan Mult(this TimeSpan ts, int multiplier) => TimeSpan.FromTicks(ts.Ticks * multiplier);
        public static TimeSpan Div(this TimeSpan ts, int denominator) => TimeSpan.FromTicks(ts.Ticks / denominator);
        public static double Div(this TimeSpan ts, TimeSpan denominator) => ts.Ticks / (double)denominator.Ticks;
    }
}
