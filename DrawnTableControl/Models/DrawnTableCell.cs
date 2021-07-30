using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace DrawnTableControl.Models
{
    public class DrawnTableCell : ICloneable
    {
        #region Variables
        internal int InnerId { get; private set; } = 0;

        private DrawnTable? table;
        public DrawnTable? Table
        {
            get => table;
            internal set
            {
                if (value == table) return;
                table?.Cells.Remove(InnerId);
                table = value;
                ResetInnerId(table);

                GetFontFromTable = getFontFromTable;
            }
        }

        private bool getFontFromTable;
        public bool GetFontFromTable
        {
            get => getFontFromTable;
            set
            {
                getFontFromTable = value;
                if (value && Table != null)
                {
                    font = Table.GetDefaultFont();
                    RedrawTable();
                }
            }
        }
        private object? value;
        public object? Value { get => value; set { this.value = value; RedrawTable(); } }

        private CellLocation location;
        public CellLocation Location { get => location; set { location = value; UpdateArea(); } }

        private int rowspan;
        public int Rowspan
        {
            get => rowspan;
            set
            {
                if (rowspan == value) return;
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(Rowspan), $"{nameof(Rowspan)} cannot be less than 1");
                }

                rowspan = value;
                UpdateArea();
            }
        }
        private SolidBrush brush;
        public SolidBrush Brush { get => brush; set { brush = value; RedrawTable(); } }

        private Font font;
        public Font Font
        {
            get => font;
            set
            {
                if (GetFontFromTable && Table != null)
                {
                    throw new InvalidOperationException($"{nameof(GetFontFromTable)} is set to true, so you cannot set font by yourself");
                }

                font = value ?? throw new ArgumentNullException(nameof(Font));
                RedrawTable();
            }
        }
        private int margin;
        public int Margin { get => margin; set { margin = value; RedrawTable(); } }
        private StringAlignment alignment;
        public StringAlignment Alignment { get => alignment; set { alignment = value; RedrawTable(); } }
        public StringAlignment lineAligment;
        public StringAlignment LineAlignment { get => lineAligment; set { lineAligment = value; RedrawTable(); } }
        public bool Enabled { get; set; } = true;
        #endregion

        public DrawnTableCell(CellLocation location) : this(location, null, 1, null) { }

        public DrawnTableCell(CellLocation location, object? value, int rowspan = 1) : this(location, value, rowspan, null) { }

        internal DrawnTableCell(CellLocation location, object? value, int rowspan, DrawnTable? table = null) : this()
        {
            ResetInnerId(table);
            Location = location;
            Value = value;
            Rowspan = rowspan;
            Margin = margin;
            Table = table;
        }

        private DrawnTableCell()
        {
            ResetStyle();
        }

        [MemberNotNull(nameof(brush), nameof(font))]
        public void ResetStyle()
        {
            Brush = brush = (SolidBrush)Brushes.DeepSkyBlue;
            if (!GetFontFromTable || font == null) Font = font = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular);
            Margin = 2;
            Alignment = StringAlignment.Near;
            LineAlignment = StringAlignment.Near;
            GetFontFromTable = true;
        }

        private void ResetInnerId(DrawnTable? table) => InnerId = table?.Cells.GetNextId() ?? 0;

        private void UpdateArea() => Table?.Cells.UpdateArea(this);

        private void RedrawTable()
        {
            if (isCloning || Table == null) return;

            Table.Redraw();
        }

        public RectangleF Area => Table?.Cells.GetArea(this) ?? RectangleF.Empty;

        #region override
        public override string? ToString() => Value?.ToString();

        public static bool operator ==(DrawnTableCell? one, DrawnTableCell? other) => one?.Equals(other) ?? other is null;
        public static bool operator !=(DrawnTableCell? one, DrawnTableCell? other) => !(one == other);

        private bool isCloning = false;
        public object Clone(bool isCopyTable = false)
        {
            isCloning = true;

            DrawnTableCell clone = (DrawnTableCell)MemberwiseClone();
            if (!isCopyTable)
            {
                clone.table = null;
            }
            if (value is ICloneable cloneableValue)
            {
                clone.Value = cloneableValue.Clone();
            }

            isCloning = false;
            return clone;
        }

        public object Clone() => Clone(false);

        public override bool Equals(object? obj)
        {
            return obj is DrawnTableCell cell &&
                   InnerId == cell.InnerId &&
                   Table == cell.Table;
        }

        public override int GetHashCode() => base.GetHashCode();

        #endregion
    }
}
