using System.Collections.Generic;
using System.Drawing;

namespace DrawnTableControl.Models
{
    public class DrawnTableHeader : DrawnTableSubheader
    {
        public List<DrawnTableSubheader> Subheaders { get; } = new();

        public DrawnTableHeader(string text = "", 
            Color? foreColor = null,
            StringAlignment alignment = StringAlignment.Center, 
            StringAlignment lineAlignment = StringAlignment.Center, 
            int span = 1, 
            object tag = null) 
            : base(text, foreColor, alignment, lineAlignment, span, tag)
        {
        }
    }
}
