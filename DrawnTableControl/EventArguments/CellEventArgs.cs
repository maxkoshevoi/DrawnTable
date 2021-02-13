using DrawnTableControl.Models;
using System;

namespace DrawnTableControl.EventArguments
{
    public class CellEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the CellEventArgs class. 
        /// </summary>
        public CellEventArgs(CellLocation location)
        {
            Location = location;
        }

        /// <summary>
        /// Location of cell that caused the event.
        /// </summary>
        public CellLocation Location { get; }
    }
}
