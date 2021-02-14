using System;
using System.Drawing;

namespace DrawnTableControl.Models
{
    public class DrawnTableSubheader
    {
        public string Text { get; }
        public Color ForeColor { get; } = Color.Black;
        public int Span { get; }
        public object Tag { get; set; }

        internal StringFormat Format { get; }

        public DrawnTableSubheader(string text = "", 
            Color? foreColor = null,
            StringAlignment alignment = StringAlignment.Center, 
            StringAlignment lineAlignment = StringAlignment.Center, 
            int span = 1, 
            object tag = null)
        {
            Text = text ?? "";
            if (foreColor.HasValue)
            {
                ForeColor = foreColor.Value;
            }
            Format = new StringFormat()
            {
                Alignment = alignment,
                LineAlignment = lineAlignment,
                Trimming = StringTrimming.EllipsisCharacter
            };
            if (span < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(span), $"{nameof(span)} can't be less than 1.");
            }
            Span = span;
            Tag = tag;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
