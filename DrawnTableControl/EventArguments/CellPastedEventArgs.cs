using DrawnTableControl.Models;

namespace DrawnTableControl.EventArguments
{
    public class CellPastedEventArgs : CellChangedEventArgs
    {
        public CellPastedEventArgs(DrawnTableCell copiedFrom, CellLocation location) : base(location)
        {
            CopiedFrom = copiedFrom;
        }

        public DrawnTableCell CopiedFrom { get; }
    }
}
