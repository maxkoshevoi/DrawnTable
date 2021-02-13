using DrawnTableControl.Models;

namespace DrawnTableControl.EventArguments
{
    public class CellMovedEventArgs : CellChangedEventArgs
    {
        public CellMovedEventArgs(CellLocation newLocation, CellLocation? oldLocation) : base(newLocation)
        {
            OldLocation = oldLocation;
        }

        public CellLocation? OldLocation { get; }
    }
}
