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

        public override string ToString() => $"{{ {nameof(Row)} = {Row}; {nameof(Column)} = {Column} }}";

        public static bool operator ==(CellLocation? one, CellLocation? other) => one?.Equals(other) ?? other is null;
        public static bool operator !=(CellLocation? one, CellLocation? other) => !(one == other);

        public override bool Equals(object? obj)
        {
            return obj is CellLocation location &&
                   Row == location.Row &&
                   Column == location.Column;
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}
