using DrawnTableControl.Enums;
using DrawnTableControl.EventArguments;
using DrawnTableControl.Models;
using DrawnTableControl.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace DrawnTableControl
{
    public class DrawnTable
    {
        #region Variables
        /// <summary>
        /// Maximum redrawing frequency of the element. 60 = 60 times per second
        /// </summary>
        public int RefreshRate { get; private set; } = 60;
        public bool ShowToolTip { get; set; } = true;
        private bool allowDragDrop = false;
        public bool AllowDragDrop
        {
            get => allowDragDrop;
            set
            {
                if (allowDragDrop == value) return;

                if (CellCopyMode == DrawnTableCopyMode.CtrlAndDrag && value == true)
                {
                    throw new InvalidOperationException($"Selected {nameof(CellCopyMode)} option require CellDragDrop. Change {nameof(CellCopyMode)} to something else first.");
                }
                allowDragDrop = value;
            }
        }
        public bool AllowCreateNewCells { get; set; } = false;
        public DrawnTableOverlapOptions ifCellsOverlap = DrawnTableOverlapOptions.ThrowError;
        /// <summary>
        /// Attention! This option allow cell to be overlaped by another cell been added via AddCell method only.
        /// If option "Hide" is chosen, once cells overlaped each other they got removed from table and basicly became inpossible to interact with.
        /// If option "Merge" is chosen, once cells overlaped each other their values need to be merged manualy by CellsMerging event.
        /// </summary>
        public DrawnTableOverlapOptions IfCellsOverlap
        {
            get => ifCellsOverlap;
            set
            {
                if (ifCellsOverlap == value) return;

                ifCellsOverlap = value;
                if (ifCellsOverlap == DrawnTableOverlapOptions.ThrowError || ifCellsOverlap == DrawnTableOverlapOptions.Merge)
                {
                    foreach (var cell in Cells)
                    {
                        if (cell is DrawnTableCellsOverlap)
                        {
                            if (ifCellsOverlap == DrawnTableOverlapOptions.ThrowError)
                            {
                                throw new InvalidOperationException("There are still overlapping cells");
                            }
                            else if (ifCellsOverlap == DrawnTableOverlapOptions.Merge)
                            {
                                throw new InvalidOperationException("Hidden cells cannot be merged =(");
                            }
                        }
                    }
                }
            }
        }
        private DrawnTableCopyMode cellCopyMode = DrawnTableCopyMode.None;
        public DrawnTableCopyMode CellCopyMode
        {
            get => cellCopyMode;
            set
            {
                if (cellCopyMode == value) return;

                if (!AllowDragDrop && value == DrawnTableCopyMode.CtrlAndDrag)
                {
                    throw new InvalidOperationException($"Enable {nameof(AllowDragDrop)} option first");
                }
                cellCopyMode = value;
            }
        }

        // Table information
        public bool IsEnabled { get; private set; } = false;
        public RectangleF CellsArea { get; private set; }
        public RectangleF TableArea { get; private set; }
        public float ColumnWidth { get; private set; }
        public float RowHeight { get; private set; }
        private List<DrawnTableHeader> RowHeaders, ColumnHeaders;
        public int RowCount() => CountHeaders(RowHeaders);
        public int ColumnCount() => CountHeaders(ColumnHeaders);
        private Color borderColor = Color.Gainsboro;
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                if (borderColor == value) return;
                borderColor = value;
                Resize(TableArea);
            }
        }

        // Cells information
        public DrawnTableCells Cells { get; private set; }
        //public int CellMargin { get; set; } = 2;

        internal Bitmap table { get; private set; }
        PBDrawnTable Owner;
        internal DeferredExecution dRedrawEx;
        internal bool isRedrawingSuspended = false;

        public PrintingManager Printing { get; private set; }
        #endregion

        #region Events
        public event EventHandler<CellMovedEventArgs> CellDragOver;
        public event EventHandler<CellMovedEventArgs> CellDragDropFinished;
        public event EventHandler<CellClickEventArgs> CellWithValueClick;
        public event EventHandler<CellChangedEventArgs> CellCreated;
        public event EventHandler<CellChangedEventArgs> CellCreating;
        public event EventHandler<CellOverlapEventArgs> CellOverlapPlaceholderClick;
        public event EventHandler<CellsMergingEventArgs> CellsMerging;
        public event EventHandler<CellCopiedEventArgs> CellCopied;
        public event EventHandler<CellPastedEventArgs> CellPasted;

        internal virtual void OnCellDragOver(CellMovedEventArgs e)
        {
            CellDragOver?.Invoke(this, e);
        }
        internal virtual void OnCellDragDropFinished(CellMovedEventArgs e)
        {
            CellDragDropFinished?.Invoke(this, e);
        }
        internal virtual void OnCellWithValueClick(CellClickEventArgs e)
        {
            CellWithValueClick?.Invoke(this, e);
        }
        internal virtual void OnCellCreating(CellChangedEventArgs e)
        {
            CellCreating?.Invoke(this, e);
        }
        internal virtual void OnCellCreated(CellChangedEventArgs e)
        {
            CellCreated?.Invoke(this, e);
        }
        internal virtual void OnCellOverlapPlaceholderClick(CellOverlapEventArgs e)
        {
            CellOverlapPlaceholderClick?.Invoke(this, e);
        }
        internal virtual void OnCellsMerging(CellsMergingEventArgs e)
        {
            CellsMerging?.Invoke(this, e);
        }
        internal virtual void OnCellCopied(CellCopiedEventArgs e)
        {
            CellCopied?.Invoke(this, e);
        }
        internal virtual void OnCellPasted(CellPastedEventArgs e)
        {
            CellPasted?.Invoke(this, e);
        }
        #endregion

        internal DrawnTable(PBDrawnTable owner)
        {
            Owner = owner;
            Cells = new DrawnTableCells(this);
            RowHeaders = new List<DrawnTableHeader>();
            ColumnHeaders = new List<DrawnTableHeader>();
            dRedrawEx = new DeferredExecution(1000 / RefreshRate); //, Owner);
            Printing = new PrintingManager(this);
        }

        #region Table Manage
        public void Create(List<DrawnTableHeader> rows, List<DrawnTableHeader> cols)
        {
            if (IsEnabled) Remove();

            RowHeaders = rows;
            ColumnHeaders = cols;
            Cells = new DrawnTableCells(this);

            TableArea = GetDefaultArea();
            table = RedrawBackground();

            IsEnabled = true;
            Redraw();
        }

        public void Remove()
        {
            IsEnabled = false;
            Cells = null;
            table = null;
            Owner.mouseDown = false;

            if (!isRedrawingSuspended)
            {
                Owner.Image?.Dispose();
                Owner.Image = null;
            }

            CellDragOver = null;
            CellDragDropFinished = null;
            CellWithValueClick = null;
            CellCreated = null;
            CellCreating = null;
            CellOverlapPlaceholderClick = null;
            CellsMerging = null;
            CellCopied = null;
            CellPasted = null;
        }

        private RectangleF GetDefaultArea()
        {
            return new RectangleF(0, 0, Owner.Width, Owner.Height);
        }

        internal void Resize() => Resize(GetDefaultArea());
        internal void Resize(RectangleF newTableArea)
        {
            dRedrawEx.Execute(() => ActualRedraw(newTableArea));
        }

        public Font GetDefaultFont()
        {
            Control c = Owner;
            while (c.Font == null)
            {
                if (c.Parent == null)
                {
                    return c.FindForm().Font;
                }
                c = c.Parent;
            }
            return c.Font;
        }
        #endregion

        #region Draw
        private Graphics GetGraphics() => GetGraphics(Owner.Image);

        internal Graphics GetGraphics(Image image)
        {
            Graphics graphics = Graphics.FromImage(image);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            return graphics;
        }

        public void Redraw()
        {
            dRedrawEx.Execute(() => ActualRedraw());
        }

        void ActualRedraw(RectangleF area = default(RectangleF))
        {
            if (isRedrawingSuspended) return;

            if (area == default(RectangleF))
            {
                area = TableArea;
            }

            Bitmap img = new Bitmap(Math.Max(5, Owner.Width), Math.Max(5, Owner.Height));
            Graphics g = GetGraphics(img);

            ActualRedraw(g, area);

            g.Dispose();
            Owner.Image?.Dispose();
            Owner.Image = img;
        }

        internal void ActualRedraw(Graphics g, RectangleF area)
        {
            if (area.Width < 5 || area.Height < 5 || !IsEnabled) return;

            // Drawing background
            bool isMoved = false;
            if (table == null || TableArea != area)
            {
                isMoved = true;
                if (table == null || TableArea.Size != area.Size)
                {
                    TableArea = area;
                    table = RedrawBackground();
                }
                else
                {
                    UpdateCellArea();
                }
            }
            g.DrawImage(table, TableArea);

            // Coping cells and back colors to draw them in the separate thread
            Dictionary<DrawnTableCell, RectangleF> currCells = null;
            Owner.Invoke(new Action(() =>
            {
                currCells = Cells.Select(c => (DrawnTableCell)c.Clone(true)).ToDictionary(x => x, i => RectangleF.Empty);
            }));

            // Drawing background colors
            DrawBackColors(g);

            // Drawing cells
            foreach (var item in currCells.Keys.ToList())
            {
                currCells[item] = DrawCell(item, g);
            }

            // Updating cells area if needed
            if (isMoved && currCells.Count > 0)
            {
                Owner.Invoke(new Action(() =>
                {
                    foreach (var cell in currCells.Keys)
                    {
                        if (Cells.Contains(cell))
                        {
                            Cells.UpdateArea(cell, currCells[cell]);
                        }
                    }
                }));
            }
        }

        private void UpdateCellArea()
        {
            CellsArea = new RectangleF(new PointF(CellsArea.X + TableArea.X, CellsArea.Y + TableArea.Y), CellsArea.Size);
        }

        internal void DrawBackColors(Graphics g)
        {
            for (int r = 0; r < RowCount(); r++)
            {
                for (int c = 0; c < ColumnCount(); c++)
                {
                    Color color;
                    do
                    {
                        try
                        {
                            color = Cells.BackColors[r, c];
                            break;
                        }
                        catch (NullReferenceException) { } // I don`t know how, but it happens
                    } while (true);
                    if (color == Color.Empty) continue;

                    g.FillRectangle(new SolidBrush(color), GetCellArea(r, c));
                }
            }
        }

        private Bitmap RedrawBackground()
        {
            Bitmap img = new Bitmap((int)TableArea.Width, (int)TableArea.Height);
            Graphics g = Graphics.FromImage(img);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            DrawTable(g, TableArea.Size, ColumnHeaders, RowHeaders, GetDefaultFont());
            g.Dispose();
            UpdateCellArea();
            return img;
        }

        private void DrawTable(Graphics g, SizeF size, IEnumerable<DrawnTableHeader> cols, IEnumerable<DrawnTableHeader> rows, Font font)
        {
            int div = 1;
            float colHeaderH = cols.Max(c => TextRenderer.MeasureText(c.Text, font).Height);
            float rowHeaderW = rows.Max(r => TextRenderer.MeasureText(r.Text, font).Width);
            colHeaderH *= 1.5f;
            rowHeaderW *= 1.15f;
            int colCount = CountHeaders(cols);
            int rowCount = CountHeaders(rows);

            bool hasSubcols = cols.Any(c => c.Subheaders.Count != 0);
            bool hasSubrows = rows.Any(r => r.Subheaders.Count != 0);
            float colSubheaderH = 0;
            float rowSubheaderW = 0;
            if (hasSubcols)
            {
                colSubheaderH = cols.Where(c => c.Subheaders.Count > 0).Max(c => c.Subheaders.Max(sh => TextRenderer.MeasureText(sh.Text, font).Height));
            }
            if (hasSubrows)
            {
                rowSubheaderW = rows.Where(r => r.Subheaders.Count > 0).Max(r => r.Subheaders.Max(sh => TextRenderer.MeasureText(sh.Text, font).Width));
            }

            float colW = (size.Width - rowHeaderW - rowSubheaderW - colCount * div) / colCount;
            float rowH = (size.Height - colHeaderH - colSubheaderH - rowCount * div) / rowCount;

            float x = rowHeaderW + rowSubheaderW;
            float y = colHeaderH + colSubheaderH;
            foreach (DrawnTableHeader col in cols)
            {
                bool hasSubheaders = col.Subheaders.Count != 0;
                float subheaderX = x;

                g.DrawLine(new Pen(BorderColor), new PointF(x, 0), new PointF(x, hasSubheaders ? colHeaderH : size.Height));
                x++;
                float W = (colW + 1) * (hasSubheaders ? col.Subheaders.Sum(sh => sh.Span) : col.Span) - 1;
                if (!string.IsNullOrEmpty(col.Text))
                {
                    g.DrawString(col.Text, font, new SolidBrush(col.ForeColor), new RectangleF(x, 0, W, colHeaderH + (hasSubheaders ? 0 : colSubheaderH)), col.format);
                }
                x += W;

                if (hasSubheaders)
                {
                    g.DrawLine(new Pen(BorderColor), new PointF(subheaderX, colHeaderH), new PointF(x, colHeaderH));
                    foreach (DrawnTableSubheader subcol in col.Subheaders)
                    {
                        g.DrawLine(new Pen(BorderColor), new PointF(subheaderX, colHeaderH), new PointF(subheaderX, size.Height));
                        subheaderX++;
                        W = (colW + 1) * subcol.Span - 1;
                        if (!string.IsNullOrEmpty(subcol.Text))
                        {
                            g.DrawString(subcol.Text, font, new SolidBrush(subcol.ForeColor), new RectangleF(subheaderX, colHeaderH, W, colSubheaderH), subcol.format);
                        }
                        subheaderX += W;
                    }
                }
            }
            foreach (DrawnTableHeader row in rows)
            {
                bool hasSubheaders = row.Subheaders.Count != 0;
                float subheaderY = y;
                
                g.DrawLine(new Pen(BorderColor), new PointF(0, y), new PointF(hasSubheaders ? rowHeaderW : size.Width, y));
                y++;
                float H = (rowH + 1) * (hasSubheaders ? row.Subheaders.Sum(sh => sh.Span) : row.Span) - 1;
                if (!string.IsNullOrEmpty(row.Text))
                {
                    g.DrawString(row.Text, font, new SolidBrush(row.ForeColor), new RectangleF(0, y, rowHeaderW + (hasSubheaders ? 0 : rowSubheaderW), H), row.format);
                }
                y += H;

                if (hasSubheaders)
                {
                    g.DrawLine(new Pen(BorderColor), new PointF(rowHeaderW, subheaderY), new PointF(rowHeaderW, y));
                    foreach (DrawnTableSubheader subrow in row.Subheaders)
                    {
                        g.DrawLine(new Pen(BorderColor), new PointF(rowHeaderW, subheaderY), new PointF(size.Width, subheaderY));
                        subheaderY++;
                        H = (rowH + 1) * subrow.Span - 1;
                        if (!string.IsNullOrEmpty(subrow.Text))
                        {
                            g.DrawString(subrow.Text, font, new SolidBrush(subrow.ForeColor), new RectangleF(rowHeaderW, subheaderY, rowSubheaderW, H), subrow.format);
                        }
                        subheaderY += H;
                    }
                }
            }

            x = rowHeaderW + rowSubheaderW;
            y = colHeaderH + colSubheaderH;
            CellsArea = new RectangleF(x, y, size.Width - x, size.Height - y);
            ColumnWidth = colW;
            RowHeight = rowH;
        }

        internal RectangleF DrawCell(DrawnTableCell cell, Graphics g = null)
        {
            RectangleF cellArea = GetCellArea(cell.Location);
            if (cell.Rowspan != 1)
            {
                float endCellB = GetCellArea(cell.Location.Row + cell.Rowspan - 1, cell.Location.Column).Bottom;
                cellArea.Height += endCellB - cellArea.Bottom;
            }

            cellArea.X += cell.Margin;
            cellArea.Y += cell.Margin;
            cellArea.Height -= cell.Margin * 2;
            cellArea.Width -= cell.Margin * 2;
            RectangleF res = DrawCell(cell.ToString(), cell.Font, cell.Brush, cellArea, new StringFormat() { Alignment = cell.Alignment, LineAlignment = cell.LineAlignment }, g);

            return res;
        }
        internal RectangleF DrawCell(string text, Font font, Brush brush, RectangleF area, StringFormat format = null, Graphics g = null)
        {
            int padding = 2;

            RectangleF res = area;
            if (g != null)
            {
                g.FillRectangle(brush, area);
            }
            if (format == null)
            {
                format = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            }
            format.Trimming = StringTrimming.Character;

            area.X += padding;
            area.Y += padding;
            area.Height -= padding * 2;
            area.Width -= padding * 2;

            if (g != null && area.Width > 0 && area.Height > 0)
            {
                g.DrawString(text, font, Brushes.White, area, format);
            }
            return res;
        }

        internal RectangleF? GetCellArea(PointF location)
        {
            float x = location.X - (location.X - CellsArea.X) % (ColumnWidth + 1);
            float y = location.Y - (location.Y - CellsArea.Y) % (RowHeight + 1);
            if (IsInArea(CellsArea, new PointF(x, y)))
            {
                return new RectangleF(x, y, ColumnWidth, RowHeight);
            }
            else
            {
                return null;
            }
        }

        internal RectangleF GetCellArea(CellLocation cellLocation) => GetCellArea(cellLocation.Row, cellLocation.Column);
        internal RectangleF GetCellArea(int row, int col)
        {
            RectangleF area = new RectangleF(CellsArea.X + col * (ColumnWidth + 1) + 1, CellsArea.Y + row * (RowHeight + 1) + 1, ColumnWidth, RowHeight);

            // Correcting Width or/and Height by 1 if there is no line right from the column or/and under the row 
            foreach (DrawnTableHeader colH in ColumnHeaders)
            {
                col -= colH.Span;
                if (col < 0)
                {
                    col++;
                    if (col == 0)
                    {
                        area.Width = (int)area.Width;
                    }
                    else
                    {
                        area.Width++;
                    }
                    break;
                }
            }

            foreach (DrawnTableHeader rowH in RowHeaders)
            {
                row -= rowH.Span;
                if (row < 0)
                {
                    row++;
                    if (row == 0)
                    {
                        area.Height = (int)area.Height;
                    }
                    else
                    {
                        area.Height++;
                    }
                    break;
                }
            }

            return area;
        }

        internal CellLocation? GetCellLocation(PointF location, bool nullIfOutOfRange = true)
        {
            int col = (int)Math.Floor((location.X - CellsArea.X) / (ColumnWidth + 1));
            int row = (int)Math.Floor((location.Y - CellsArea.Y) / (RowHeight + 1));
            if (nullIfOutOfRange && !IsInArea(CellsArea, GetCellArea(row, col).Location))
            {
                //row = col = -1;
                return null;
            }
            return new CellLocation(row, col);
        }

        public static bool IsInArea(RectangleF area, PointF point) => (point.X >= area.X && point.Y >= area.Y && point.X <= area.Right && point.Y <= area.Bottom);

        private int CountHeaders(IEnumerable<DrawnTableHeader> headers)
        {
            if (headers == null)
            {
                return 0;
            }

            int headerCount = 0;
            foreach (DrawnTableHeader header in headers)
            {
                if (header.Subheaders.Count == 0)
                {
                    headerCount += header.Span;
                }
                else
                {
                    headerCount += header.Subheaders.Sum(sh => sh.Span);
                }
            }
            return headerCount;
        }

        public void SuspendRedrawing()
        {
            isRedrawingSuspended = true;
        }

        public void ResumeRedrawing()
        {
            isRedrawingSuspended = false;
            if (IsEnabled)
            {
                Redraw();
            }
            else
            {
                Owner.Image?.Dispose();
                Owner.Image = null;
            }
        }
        #endregion
    }
}
