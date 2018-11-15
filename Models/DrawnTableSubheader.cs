using System;
using System.Drawing;

namespace DrawnTableControl.Models
{
    public class DrawnTableSubheader
    {
        public string Text { get; private set; }
        public StringAlignment Alignment { get; private set; }
        public StringAlignment LineAlignment { get; private set; }
        public Color ForeColor { get; private set; } = Color.Black;
        public int Span { get; private set; }
        public object Tag { get; set; }

        internal StringFormat format { get; private set; }

        public DrawnTableSubheader(string text = "", Color? foreColor = null,
            StringAlignment alignment = StringAlignment.Center, StringAlignment lineAlignment = StringAlignment.Center, int span = 1, object tag = null)
        {
            Text = text ?? "";
            if (foreColor.HasValue)
            {
                ForeColor = foreColor.Value;
            }
            format = new StringFormat()
            {
                Alignment = alignment,
                LineAlignment = lineAlignment,
                Trimming = StringTrimming.EllipsisCharacter
            };
            if (span < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(span), $"{nameof(span)} can't be less then 1.");
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
