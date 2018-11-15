using DrawnTableControl.Models;

namespace DrawnTableControl.EventArguments
{
    public class CellMovedEventArgs : CellChangedEventArgs
    {
        public CellMovedEventArgs(CellLocation newLocation, CellLocation? oldLocation) : base(newLocation)
        {
            OldLocation = oldLocation;
        }

        private CellLocation? oldLocation;
        public CellLocation? OldLocation { get => oldLocation; private set => oldLocation = value; }
    }
}
