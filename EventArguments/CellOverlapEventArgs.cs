using DrawnTableControl.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DrawnTableControl.EventArguments
{
    public class CellOverlapEventArgs : CellClickEventArgs
    {
        public CellOverlapEventArgs(MouseEventArgs e, CellLocation location, List<DrawnTableCell> overlappingCells) : base(e, location)
        {
            OverlappingCells = overlappingCells;
        }

        private List<DrawnTableCell> overlappingCells;
        public List<DrawnTableCell> OverlappingCells { get => overlappingCells; private set => overlappingCells = value; }
    }
}
