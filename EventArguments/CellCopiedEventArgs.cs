using DrawnTableControl.Models;
using System;

namespace DrawnTableControl.EventArguments
{
    public class CellCopiedEventArgs : EventArgs
    {
        public CellCopiedEventArgs(DrawnTableCell copiedCell)
        {
            CopiedCell = copiedCell;
        }

        public DrawnTableCell CopiedCell { get; private set; }
    }
}
