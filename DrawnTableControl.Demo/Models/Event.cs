using System;
using System.Drawing;

namespace DrawnTableControl.Demo.Models
{
    public class Event : ICloneable
    {
        public DateTime Date { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }

        public Location Location { get; set; }

        public Color? Color => Location?.Color;
        public bool Enabled { get; set; } = true;

        public object Clone() =>
            MemberwiseClone();

        public override string ToString()
        {
            return $"{Date:d}\n" +
                $"{Start:hh\\:mm} - {End:hh\\:mm}\n" +
                $"Location: {Location?.Name}\n" +
                $"Enabled: {Enabled}";
        }
    }
}
