using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DrawnTableControl.Models
{
    public class DrawnTableCellsOverlap : DrawnTableCell
    {
        public new List<DrawnTableCell> Value { get; private set; }

        internal DrawnTableCellsOverlap(CellLocation location, List<DrawnTableCell> value, int rowspan = 1, DrawnTable table = null) : base(location, null, rowspan, table)
        {
            Value = value;
            Margin = 0;
            Alignment = StringAlignment.Center;
            LineAlignment = StringAlignment.Center;
            GetFontFromTable = false;
            Font = new Font(Font.FontFamily, Font.Size + 3); //, FontStyle.Underline
        }

        public override string ToString()
        {
            return Value.Count.ToString();
        }
    }
}
