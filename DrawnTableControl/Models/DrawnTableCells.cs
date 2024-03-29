﻿using DrawnTableControl.Enums;
using DrawnTableControl.EventArguments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using static DrawnTableControl.DrawnTable;

namespace DrawnTableControl.Models
{
    public class DrawnTableCells : IEnumerable<DrawnTableCell>
    {
        private int lastId;
        private readonly DrawnTable table;
        private readonly List<DrawnTableCell> cells;
        private readonly Dictionary<int, RectangleF> cellsArea;

        public DrawnTableBackColors BackColors { get; }

        internal DrawnTableCells(DrawnTable table)
        {
            this.table = table ?? throw new ArgumentNullException(nameof(table));
            lastId = 0;
            cells = new List<DrawnTableCell>();
            cellsArea = new Dictionary<int, RectangleF>();
            BackColors = new DrawnTableBackColors(table);
        }

        #region Cells
        internal int GetNextId() =>
            ++lastId;

        public bool Contains(DrawnTableCell cell) =>
            cells.Contains(cell);

        public int Count => cells.Count;

        #region Area
        internal RectangleF GetArea(DrawnTableCell cell)
        {
            if (cell.Table != table)
            {
                throw new ArgumentException("Tables must be the same");
            }
            return cellsArea[cell.InnerId];
        }

        internal void UpdateArea(DrawnTableCell cell)
        {
            if (cell.Table != table || !Contains(cell))
            {
                throw new ArgumentException("This cell doesn't exists in this table");
            }

            RectangleF oldArea = cellsArea[cell.InnerId];
            cellsArea[cell.InnerId] = table.DrawCell(cell);
            if (cellsArea[cell.InnerId] != oldArea)
            {
                table.Redraw();
            }
        }

        internal void UpdateArea(DrawnTableCell cell, RectangleF newArea)
        {
            if (cell.Table != table || !Contains(cell))
            {
                throw new ArgumentException("This cell doesn't exists in this table");
            }

            cellsArea[cell.InnerId] = newArea;
        }
        #endregion

        #region Add
        public void Add(int row, int col, object? value, int rowspan = 1)
        {
            DrawnTableCell cell = new(new CellLocation(row, col), value, rowspan);
            Add(cell);
        }

        public void Add(DrawnTableCell cell) => Add(cell, true);

        internal void Add(DrawnTableCell cell, bool draw = true)
        {
            if (!table.IsEnabled)
            {
                throw new InvalidOperationException($"Table must be initialized first. Call {nameof(DrawnTable.Create)} to create the table.");
            }

            if (cell.Table != table)
            {
                cell.Table = table;
            }
            else
            {
                throw new ArgumentException("This cell is already added to this table", nameof(cell));
            }

            if (cell.Location.Column < 0 || cell.Location.Column >= cell.Table.ColumnCount()
                || cell.Location.Row < 0 || cell.Location.Row >= cell.Table.RowCount())
            {
                throw new ArgumentException("Cell location is out of bounds.", nameof(cell));
            }

            List<DrawnTableCell> oCells = GetOverlap(cell);
            if (oCells.Count > 0)
            {
                if (table.IfCellsOverlap == DrawnTableOverlapOptions.ThrowError)
                {
                    throw new ArgumentException("Duplicate entry", nameof(cell));
                }

                RectangleF area = RectangleF.Empty;
                foreach (var oCell in oCells)
                {
                    HandleCellOverlap(ref cell, ref area, oCell);
                }
                cell.Table = table;
                cells.Add(cell);
                cellsArea.Add(cell.InnerId, area);
            }
            else
            {
                RectangleF area = table.DrawCell(cell);
                cells.Add(cell);
                cellsArea.Add(cell.InnerId, area);
            }
            if (draw)
            {
                table.Redraw();
            }
        }

        private void HandleCellOverlap(ref DrawnTableCell newCell, ref RectangleF totalArea, DrawnTableCell existingCell)
        {
            Remove(existingCell.InnerId);
            if (table.IfCellsOverlap == DrawnTableOverlapOptions.ReplaceWithCounter)
            {
                newCell.Table = null;
                if (existingCell is DrawnTableCellsOverlap || newCell is DrawnTableCellsOverlap)
                {
                    if (existingCell is DrawnTableCellsOverlap existingOverlap && newCell is DrawnTableCellsOverlap newOverlap)
                    {
                        foreach (DrawnTableCell item in newOverlap.Value)
                        {
                            existingOverlap.Value.Add(item);
                        }
                    }
                    else if (existingCell is DrawnTableCellsOverlap existingOverlap2)
                    {
                        existingOverlap2.Value.Add(newCell);
                    }
                    else
                    {
                        ((DrawnTableCellsOverlap)newCell).Value.Add(existingCell);
                        var tmp = existingCell;
                        existingCell = newCell;
                        newCell = tmp;
                    }

                    Rectangle rct = Projection(newCell, existingCell);
                    existingCell.Location = new CellLocation(rct.Y, rct.X);
                    existingCell.Rowspan = rct.Height;

                    totalArea = table.DrawCell(existingCell);
                    newCell = existingCell;
                }
                else
                {
                    Rectangle rct = Projection(newCell, existingCell);
                    DrawnTableCellsOverlap o = new(new CellLocation(rct.Y, rct.X), new() { existingCell, newCell }, rct.Height);
                    RectangleF cArea = table.DrawCell(o);

                    totalArea = cArea;
                    newCell = o;
                }
            }
            else if (table.IfCellsOverlap == DrawnTableOverlapOptions.Merge)
            {
                newCell.Table = null;
                Rectangle rct = Projection(newCell, existingCell);
                DrawnTableCell result = new(new CellLocation(rct.Y, rct.X), null, rct.Height);
                if (newCell.Value == existingCell.Value) result.Value = newCell.Value;
                result.GetFontFromTable = (newCell.GetFontFromTable == existingCell.GetFontFromTable) && newCell.GetFontFromTable;
                if (result.GetFontFromTable == false)
                {
                    result.Font = !newCell.GetFontFromTable ? newCell.Font : existingCell.Font;
                }
                else if (newCell.Font.Equals(existingCell.Font))
                {
                    result.Font = newCell.Font;
                }
                if (newCell.Brush == existingCell.Brush) result.Brush = newCell.Brush;
                if (newCell.Alignment == existingCell.Alignment) result.Alignment = newCell.Alignment;
                if (newCell.LineAlignment == existingCell.LineAlignment) result.LineAlignment = newCell.LineAlignment;
                if (newCell.Margin == existingCell.Margin) result.Margin = newCell.Margin;

                CellsMergingEventArgs e = new(existingCell, newCell, result);
                table.OnCellsMerging(e);

                RectangleF cArea = table.DrawCell(e.ResultCell);

                totalArea = cArea;
                newCell = e.ResultCell;
            }
        }

        private static Rectangle Projection(DrawnTableCell cell1, DrawnTableCell cell2)
        {
            Rectangle rct = new(cell2.Location.Column, cell2.Location.Row, 1, cell2.Rowspan);
            if (cell1.Location.Row < rct.Y)
            {
                rct.Height += (rct.Y - cell1.Location.Row);
                rct.Y = cell1.Location.Row;
            }
            if (cell1.Location.Row + cell1.Rowspan > rct.Y + rct.Height)
            {
                rct.Height += ((cell1.Location.Row + cell1.Rowspan) - (rct.Y + rct.Height));
            }
            return rct;
        }

        public List<DrawnTableCell> GetOverlap(DrawnTableCell cell)
        {
            List<DrawnTableCell> overlaps = new();
            for (int i = 0; i < cell.Rowspan; i++)
            {
                DrawnTableCell? overlap = this[cell.Location.Row + i, cell.Location.Column];
                if (overlap != null && overlap != cell && !overlaps.Contains(overlap))
                {
                    overlaps.Add(overlap);
                }
            }
            return overlaps;
        }
        #endregion

        #region Remove
        public bool Remove(CellLocation cellLocation) => Remove(cellLocation.Row, cellLocation.Column);

        public bool Remove(int row, int col)
        {
            DrawnTableCell? cell = this[row, col];
            if (cell == null) return false;

            bool res = cells.Remove(cell);
            if (res)
            {
                cellsArea.Remove(cell.InnerId);
                cell.Table = null;
                table.Redraw();
            }
            return res;
        }

        public bool Remove(DrawnTableCell cell)
        {
            if (cell.Table != table) return false;

            return Remove(cell.InnerId);
        }

        internal bool Remove(int InnerId)
        {
            foreach (DrawnTableCell cell in cells)
            {
                if (cell.InnerId == InnerId)
                {
                    Remove(cell.Location);
                    return true;
                }
            }
            return false;
        }

        public int RemoveByValue(object Value)
        {
            if (Value == null)
            {
                throw new ArgumentException($"'{nameof(Value)}' can't be NULL", nameof(Value));
            }

            List<int> toRemove = new();
            foreach (DrawnTableCell cell in cells)
            {
                if (cell.Value != null && cell.Value.Equals(Value))
                {
                    toRemove.Add(cell.InnerId);
                }
            }

            foreach (int innerId in toRemove)
            {
                Remove(innerId);
            }

            return toRemove.Count;
        }
        #endregion

        #region Indexers
        public DrawnTableCell? this[CellLocation cellLocation] => this[cellLocation.Row, cellLocation.Column];

        public DrawnTableCell? this[int row, int col]
        {
            get
            {
                foreach (var cell in cells)
                {
                    if (col == cell.Location.Column && (row >= cell.Location.Row && row <= cell.Location.Row + cell.Rowspan - 1))
                    {
                        return cell;
                    }
                }
                return null;
            }
        }

        public DrawnTableCell? this[PointF location]
        {
            get
            {
                foreach (var cell in cells)
                {
                    RectangleF area = cell.Area;
                    if (IsInArea(area, location))
                    {
                        return cell;
                    }
                }
                return null;
            }
        }
        #endregion

        public IEnumerator GetEnumerator() => ((IEnumerable<DrawnTableCell>)cells).GetEnumerator();

        IEnumerator<DrawnTableCell> IEnumerable<DrawnTableCell>.GetEnumerator() => ((IEnumerable<DrawnTableCell>)cells).GetEnumerator();

        #endregion

        #region BackColors
        public class DrawnTableBackColors
        {
            private readonly DrawnTable table;
            private readonly Color[,] backColors;

            internal DrawnTableBackColors(DrawnTable table)
            {
                this.table = table;
                backColors = new Color[table?.RowCount() ?? 0, table?.ColumnCount() ?? 0];
            }

            public Color this[int row, int col]
            {
                get => backColors[row, col];
                set
                {
                    if (backColors[row, col] == value) return;

                    backColors[row, col] = value;
                    table?.Redraw();
                }
            }

            public void SetAll(Color color)
            {
                for (int i = 0; i < backColors.GetLength(0); i++)
                {
                    for (int j = 0; j < backColors.GetLength(1); j++)
                    {
                        this[i, j] = color;
                    }
                }
            }

            public void ResetAll() => SetAll(Color.Empty);

            public void SetRow(int rowIndex, Color color)
            {
                for (int j = 0; j < backColors.GetLength(1); j++)
                {
                    this[rowIndex, j] = color;
                }
            }

            public void SetColumn(int colIndex, Color color)
            {
                for (int i = 0; i < backColors.GetLength(0); i++)
                {
                    this[i, colIndex] = color;
                }
            }
        }
        #endregion
    }
}
