using DrawnTableControl.Models;
using System.Windows.Forms;

namespace DrawnTableControl.EventArguments
{
    public class CellClickEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the CellEventArgs class. 
        /// </summary>
        /// <param name="e">An ordinary <see cref="MouseEventArguments"/> argument to be extended.</param>
        internal CellClickEventArgs(MouseEventArgs e, CellLocation location) : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            CellLocation = location;
        }

        private CellLocation location;
        /// <summary>
        /// Location of cell that caused the event.
        /// </summary>
        public CellLocation CellLocation { get => location; private set => location = value; }
    }
}
