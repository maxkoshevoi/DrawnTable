using DrawnTableControl.Models;

namespace DrawnTableControl.EventArguments
{
    public class CellChangedEventArgs : CellEventArgs
    {
        public CellChangedEventArgs(CellLocation location) : base(location)
        { }

        /// <summary>
        /// Set this property to <b>true</b> inside your event handler to prevent further processing of the event in other applications.
        /// </summary>
        public bool Handled { get; set; } = false;
    }
}
