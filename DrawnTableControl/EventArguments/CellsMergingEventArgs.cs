using DrawnTableControl.Models;
using System;

namespace DrawnTableControl.EventArguments
{
    public class CellsMergingEventArgs : EventArgs
    {
        public CellsMergingEventArgs(DrawnTableCell firstCell, DrawnTableCell secondCell, DrawnTableCell resultCell)
        {
            FirstCell = firstCell;
            SecondCell = secondCell;
            ResultCell = resultCell;
        }

        public DrawnTableCell FirstCell { get; }
        public DrawnTableCell SecondCell { get; }
        public DrawnTableCell ResultCell { get; set; }
    }
}
