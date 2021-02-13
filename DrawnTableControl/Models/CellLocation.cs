namespace DrawnTableControl.Models
{
    public struct CellLocation
    {
        public int Row { get; }
        public int Column { get; }

        public CellLocation(int row, int col)
        {
            Row = row;
            Column = col;
        }

        public override string ToString()
        {
            return $"{{ {nameof(Row)} = {Row}; {nameof(Column)} = {Column} }}";
        }

        public static bool operator ==(CellLocation one, CellLocation other) => one.Row == other.Row && one.Column == other.Column;
        public static bool operator !=(CellLocation one, CellLocation other) => !(one == other);
        public override bool Equals(object obj)
        {
            if (obj is CellLocation location)
            {
                return location == this;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
