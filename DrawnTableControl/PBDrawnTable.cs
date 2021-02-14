using DrawnTableControl.Enums;
using DrawnTableControl.EventArguments;
using DrawnTableControl.Models;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DrawnTableControl.DrawnTable;

namespace DrawnTableControl
{
    [DesignerCategory("code")]
    public class PBDrawnTable : PictureBox
    {
        public DrawnTable Table { get; }

        public PBDrawnTable()
        {
            Table = new DrawnTable(this);
            toolTip = new ToolTip();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Table.IsEnabled)
            {
                Table.Resize();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (Table.IsEnabled)
            {
                if (toolTip.Tag != null && !IsInArea((RectangleF)toolTip.Tag, PointToClient(Cursor.Position)))
                {
                    toolTip.Tag = null;
                    toolTip.Hide(this);
                }
            }
            base.OnMouseLeave(e);
        }

        #region Mouse
        // Всплывающая подсказка
        readonly ToolTip toolTip;
        // Необходимо для реализации DradAndDrop
        internal bool? mouseDown = false;
        Point startDrag;
        DrawnTableCell interactWith;
        Bitmap dragBackground;
        CellLocation? dragCoord;
        bool allowDrop;
        InteractAction action;
        // Копирование ячейки перетаскиванием
        bool copyMode_DrugDrop;
        DrawnTableCell copySource;

        enum InteractAction { None, Hover, Click, Drag, CellCreating }

        private Point CursorPosition() => PointToClient(Cursor.Position);

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Table.IsEnabled && mouseDown != null)
            {
                copyMode_DrugDrop = false;
                copySource = null;
                action = InteractAction.None;
                allowDrop = true;
                dragCoord = Table.GetCellLocation(e.Location);
                interactWith = Table.Cells[e.Location];
                if (interactWith != null && interactWith.Enabled)
                {
                    if (interactWith is DrawnTableCell && Table.CellCopyMode == DrawnTableCopyMode.CtrlAndDrag && ModifierKeys.HasFlag(Keys.Control))
                    {
                        copySource = interactWith;
                        DrawnTableCell clone = (DrawnTableCell)interactWith.Clone();
                        Table.OnCellCopied(new CellCopiedEventArgs(clone));
                        interactWith = clone;
                        copyMode_DrugDrop = true;
                    }

                    action = InteractAction.Click;
                    dragCoord = interactWith.Location;
                    if (Table.AllowDragDrop) // || copyMode_DrugDrop
                    {
                        Table.dRedrawEx.Join();
                        Table.dRedrawEx.Pause();

                        lock (Table.table)
                        {
                            dragBackground = (Bitmap)Table.table.Clone();
                        }
                        using Graphics g = GetGraphics(dragBackground);

                        Table.DrawBackColors(g);

                        // Drawing cells
                        foreach (DrawnTableCell item in Table.Cells)
                        {
                            if (item == interactWith && !copyMode_DrugDrop) continue;
                            Table.DrawCell(item, g);
                        }

                        Table.dRedrawEx.Resume();
                    }
                }
                else if (Table.AllowCreateNewCells)
                {
                    CellLocation? cLoc = Table.GetCellLocation(e.Location);
                    if (cLoc != null && Table.Cells[(CellLocation)cLoc] == null)
                    {
                        DrawnTableCell cell = new(cLoc.Value);
                        Table.Cells.Add(cell, false);
                        CellChangedEventArgs args = new(cLoc.Value);
                        Table.OnCellCreating(args);
                        if (!args.Handled)
                        {
                            action = InteractAction.CellCreating;
                            interactWith = Table.Cells[cLoc.Value];
                            Table.Redraw();
                        }
                        else
                        {
                            Table.Cells.Remove(cLoc.Value);
                        }
                    }
                }
                startDrag = e.Location;
                mouseDown = true;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (Table.IsEnabled && mouseDown != null)
            {
                mouseDown = null;
                Table.dRedrawEx.Join();
                if (action == InteractAction.CellCreating)
                {
                    CellChangedEventArgs args = new(interactWith.Location);
                    Table.OnCellCreated(args);
                    if (args.Handled)
                    {
                        Table.Cells.Remove(interactWith.Location);
                    }
                }
                else if (interactWith != null && interactWith.Enabled)
                {
                    bool sameTable = interactWith.Table == Table;
                    // Whether the MouseDown cell matches the MouseUp one
                    bool sameLocation = dragCoord != null && sameTable && interactWith.Location == dragCoord.Value;
                    bool sameCell = interactWith == Table.Cells[e.Location];

                    // DragDrop
                    if (action == InteractAction.Drag && allowDrop)
                    {
                        if (dragCoord != null && !sameLocation)
                        {
                            CellLocation? oldLocation = null;
                            if (sameTable)
                            {
                                oldLocation = interactWith.Location;
                            }
                            interactWith.Location = dragCoord.Value;
                            if (!sameTable)
                            {
                                Table.Cells.Add(interactWith);
                            }

                            CellChangedEventArgs args;

                            if (copyMode_DrugDrop)
                            {
                                args = new CellPastedEventArgs(copySource, dragCoord.Value);
                                Table.OnCellPasted((CellPastedEventArgs)args);
                            }
                            else
                            {
                                args = new CellMovedEventArgs(dragCoord.Value, oldLocation);
                                Table.OnCellDragDropFinished((CellMovedEventArgs)args);
                            }

                            if (args.Handled)
                            {
                                if (sameTable && !copyMode_DrugDrop)
                                {
                                    interactWith.Location = oldLocation.Value;
                                }
                                else
                                {
                                    Table.Cells.Remove(interactWith.Location);
                                }
                            }
                        }
                    }

                    // Click
                    if (action == InteractAction.Click && sameCell)
                    {
                        if (interactWith is DrawnTableCellsOverlap)
                        {
                            Table.OnCellOverlapPlaceholderClick(new CellOverlapEventArgs(e, interactWith.Location, (interactWith as DrawnTableCellsOverlap).Value));
                        }
                        else
                        {
                            Table.OnCellWithValueClick(new CellClickEventArgs(e, interactWith.Location));
                        }
                    }
                }
                Table.Redraw();
                interactWith = null;
                action = InteractAction.None;
                dragBackground?.Dispose();
                mouseDown = false;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Table.IsEnabled && mouseDown != null)
            {
                Point pos = e.Location;
                DrawnTableCell cellUnder = Table.Cells[pos];

                if (mouseDown == false)
                {
                    if (cellUnder != null && cellUnder.Enabled)
                    {
                        Cursor = Cursors.Hand;
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                    }
                    if (toolTip.Tag != null && !IsInArea((RectangleF)toolTip.Tag, pos))
                    {
                        toolTip.Tag = null;
                        toolTip.Hide(this);
                    }
                    else if (Table.ShowToolTip && toolTip.Tag == null && cellUnder != null)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            string tipText = cellUnder.ToString();
                            if (cellUnder is DrawnTableCellsOverlap cellUnderOverlap)
                            {
                                tipText = "";
                                foreach (DrawnTableCell cell in cellUnderOverlap.Value)
                                {
                                    tipText += cell + "\r\n\r\n";
                                }
                            }
                            if (string.IsNullOrEmpty(tipText)) return;
                            Thread.Sleep(500);
                            try
                            {
                                if (mouseDown == true || toolTip.Tag != null) return;
                                Invoke((MethodInvoker)delegate
                                {
                                    Point posNew = CursorPosition();
                                    if (cellUnder != Table.Cells[posNew]) return;
                                    RectangleF lPos = cellUnder.Area;
                                    toolTip.Tag = lPos;
                                    toolTip.Show(tipText, this, new Point((int)lPos.Left + 5, (int)lPos.Bottom + 5), int.MaxValue);
                                });
                            }
                            catch { }
                        });
                    }
                }
                else
                {
                    toolTip.Tag = null;
                    toolTip.Hide(this);
                }

                if (mouseDown == true)
                {
                    if (action == InteractAction.CellCreating)
                    {
                        CellLocation? cLoc = Table.GetCellLocation(e.Location, false);
                        if (cLoc != null && cLoc.Value.Row != interactWith.Location.Row + interactWith.Rowspan - 1)
                        {
                            if (cLoc.Value.Row >= interactWith.Location.Row)
                            {
                                int newRowspan = cLoc.Value.Row - interactWith.Location.Row + 1;
                                for (int i = 1; i < newRowspan; i++)
                                {
                                    DrawnTableCell tmp = Table.Cells[interactWith.Location.Row + i, interactWith.Location.Column];
                                    if (interactWith.Location.Row + i >= Table.RowCount() || (tmp != null && tmp != interactWith))
                                    {
                                        newRowspan = i;
                                        break;
                                    }
                                }
                                interactWith.Rowspan = newRowspan;
                            }
                            else
                            {
                                interactWith.Rowspan = 1;
                            }
                        }
                    }
                    else if (interactWith != null && interactWith.Enabled)
                    {
                        Cursor = Cursors.Hand;

                        if (interactWith is DrawnTableCellsOverlap)
                        {
                            allowDrop = false;
                        }
                        else if (Table.AllowDragDrop)
                        {
                            bool isMovedFarEnoughToStartDragDrop = Math.Pow(startDrag.X - e.X, 2) + Math.Pow(startDrag.Y - e.Y, 2) > 3;
                            if (action == InteractAction.Drag || isMovedFarEnoughToStartDragDrop)
                            {
                                action = InteractAction.Drag;
                                Table.dRedrawEx.Execute(() => RedrawCellDrag(e));
                            }
                        }
                    }
                }
                else
                {
                    action = InteractAction.Hover;
                }
            }

            base.OnMouseMove(e);
        }

        private void RedrawCellDrag(MouseEventArgs e)
        {
            bool isOk = true;
            Bitmap imgNow;
            lock (dragBackground)
            {
                imgNow = (Bitmap)dragBackground.Clone();
            }
            using Graphics g = Graphics.FromImage(imgNow);
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.CompositingQuality = CompositingQuality.HighSpeed;

            RectangleF area = Table.Cells.Contains(interactWith) ? interactWith.Area : Table.DrawCell(interactWith);
            RectangleF rect = new(e.X - (startDrag.X - area.X), e.Y - (startDrag.Y - area.Y), area.Width, area.Height);
            CellLocation? cLoc = Table.GetCellLocation(new PointF(rect.X + rect.Width / 2, (int)(rect.Y + Table.RowHeight / 2)));
            if (cLoc == null)
            {
                isOk = false;
            }
            else
            {
                RectangleF cellA = Table.GetCellArea(cLoc.Value);
                if (interactWith.Rowspan != 1)
                {
                    float endCellB = Table.GetCellArea(cLoc.Value.Row + interactWith.Rowspan - 1, cLoc.Value.Column).Bottom;
                    cellA.Height += endCellB - cellA.Bottom;
                }
                if (interactWith.Table == Table && cLoc.Value == interactWith.Location)
                {
                    allowDrop = true;
                }
                else if (cLoc == dragCoord)
                {
                    isOk = allowDrop;
                }
                else
                {
                    if (IsInArea(Table.CellsArea, new PointF(cellA.X, cellA.Bottom)))
                    {
                        for (int i = 0; i < interactWith.Rowspan; i++)
                        {
                            DrawnTableCell tmp = Table.Cells[cLoc.Value.Row + i, cLoc.Value.Column];
                            if (tmp != null && tmp != interactWith)
                            {
                                isOk = false;
                                break;
                            }
                        }
                        if (isOk)
                        {
                            CellMovedEventArgs args = new(cLoc.Value, interactWith.Location);
                            Table.OnCellDragOver(args);
                            if (args.Handled)
                            {
                                isOk = false;
                            }
                        }
                    }
                    else
                    {
                        isOk = false;
                    }
                }
                Color color = Color.LightSeaGreen;
                if (!isOk)
                {
                    color = Color.PaleVioletRed;
                }
                g.FillRectangle(new SolidBrush(Color.FromArgb(75, color)), cellA);
            }
            dragCoord = cLoc;
            allowDrop = isOk;

            if (Table.isRedrawingSuspended)
            {
                return;
            }

            DrawCell(
                interactWith.ToString(),
                interactWith.Font,
                new SolidBrush(Color.FromArgb(150, interactWith.Brush.Color)),
                rect,
                new StringFormat() { Alignment = interactWith.Alignment, LineAlignment = interactWith.LineAlignment },
                g);

            Image = imgNow;
        }

        public new Image Image
        {
            get => base.Image;
            set
            {
                if (InvokeRequired)
                {
                    BeginInvoke((Action) delegate
                    {
#pragma warning disable CA2011 // Avoid infinite recursion
                        Image = value;
#pragma warning restore CA2011 // Avoid infinite recursion
                    });
                    return;
                }

                base.Image?.Dispose();
                base.Image = value;
            }
        }
        #endregion
    }
}
