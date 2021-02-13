using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using DrawnTableControl.Enums;
using DrawnTableControl.EventArguments;
using DrawnTableControl.HeaderHelpers;
using DrawnTableControl.Models;

namespace DrawnTableControl.Demo
{
    public partial class Form1 : Form
    {
        private readonly HeaderCreator headers = new();

        public Form1()
        {
            InitializeComponent();
            cbLTGroupBy.SelectedIndex = 0;
        }

        enum TStyle { None, DayTime, LocationTime, DayList }

        private void Setting_Changed(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void UpdateTable()
        {
            List<Event> events = new();
            DateTime dayStart = DateTime.Now;
            DateTime dayEnd = dayStart.AddDays(6);
            TStyle style = GetStyle();

            List<DrawnTableHeader> Rows = null, Cols = null;
            List<DayOfWeek> specificDaysOfWeek = pViewDays.Controls.OfType<CheckBox>()
                .Where(day => day.Checked)
                .Select(day => (DayOfWeek)int.Parse(day.Tag?.ToString()))
                .ToList();
            switch (style)
            {
                case TStyle.DayTime:
                case TStyle.LocationTime:
                {
                    Func<DateTime, bool> colFilter = GetHeaderDayFilter(specificDaysOfWeek, events);
                    Cols = headers.Day.GenerateHeaders(dayStart, dayEnd, colFilter, "dd, dddd").ToList();
                    Rows = headers.Time.GenerateHeaders(TimeSpan.FromHours(8), TimeSpan.FromHours(18), TimeSpan.FromHours(1), 4).ToList();
                    break;
                }
                case TStyle.DayList:
                {
                    Func<DateTime, bool> colFilter = GetHeaderDayFilter(specificDaysOfWeek, events);
                    Cols = headers.Day.GenerateHeaders(dayStart, dayEnd, colFilter, "dd.MM, dddd").ToList();
                    int maxEventsPerDay = !events.Any() ? 0 : events.GroupBy(e => e.Date).Max(g => g.Count());
                    headers.Custom1.Clear();
                    for (int i = 1; i <= maxEventsPerDay + 1; i++)
                    {
                        headers.Custom1.Add(i.ToString());
                    }
                    Rows = headers.Custom1.Headers;
                    break;
                }
            }


            pbDrawnTable.Table.SuspendRedrawing();
            InitTable(Rows, Cols);

            foreach (var e in events)
            {
                DrawnTableCell cell = CreateCell(e, headers, style);
                if (cell is null)
                {
                    continue;
                }

                pbDrawnTable.Table.Cells.Add(cell);
            }

            pbDrawnTable.Table.ResumeRedrawing();
        }

        private Func<DateTime, bool> GetHeaderDayFilter(List<DayOfWeek> specificDaysOfWeek, List<Event> events)
        {
            Expression<Func<DateTime, bool>> headerFilter = (day) => true;
            if (specificDaysOfWeek.Count > 0)
            {
                var prefix = headerFilter.Compile();
                headerFilter = (day) => prefix(day) && specificDaysOfWeek.Contains(day.DayOfWeek);
            }
            if (chHideEmptyColumns.Checked)
            {
                var prefix = headerFilter.Compile();
                headerFilter = (day) => prefix(day) && events.Exists(e => e.Date == day);
            }
            return headerFilter.Compile();
        }

        void InitTable(List<DrawnTableHeader> Rows, List<DrawnTableHeader> Cols)
        {
            pbDrawnTable.Table.Create(Rows, Cols);
            pbDrawnTable.Table.CellCreating += Table_CellCreating;
            pbDrawnTable.Table.CellWithValueClick += Table_CellWithValueClick;
            pbDrawnTable.Table.CellOverlapPlaceholderClick += Table_CellOverlapPlaceholderClick;
            pbDrawnTable.Table.CellDragDropFinished += Table_CellDragDropFinished;
            pbDrawnTable.Table.CellCopied += Table_CellCopied;
            pbDrawnTable.Table.CellPasted += Table_CellPasted;
            pbDrawnTable.Table.AllowCreateNewCells = true;
            pbDrawnTable.Table.AllowDragDrop = true;
            pbDrawnTable.Table.IfCellsOverlap = DrawnTableOverlapOptions.ReplaceWithCounter;
            pbDrawnTable.Table.CellCopyMode = DrawnTableCopyMode.CtrlAndDrag;
        }

        private void Table_CellPasted(object sender, CellPastedEventArgs e)
        {
            Event ev = (Event)pbDrawnTable.Table.Cells[e.Location].Value;
            OpenEventEdit(ev.ID, pbDrawnTable.Table.Cells[e.Location], e.CopiedFrom.Location);
        }

        private void Table_CellCopied(object sender, CellCopiedEventArgs e)
        {
            (e.CopiedCell.Value as Event).ID = -(e.CopiedCell.Value as Event).ID;
        }

        private void Table_CellDragDropFinished(object sender, CellMovedEventArgs e)
        {
            object v = pbDrawnTable.Table.Cells[e.Location].Value;
            if (v is Event ev)
            {
                OpenEventEdit(ev.ID, pbDrawnTable.Table.Cells[e.Location], e.OldLocation);
            }
        }

        private void Table_CellWithValueClick(object sender, CellClickEventArgs e)
        {
            DrawnTableCell cell = pbDrawnTable.Table.Cells[e.Location];
            if (e.Button == MouseButtons.Left)
            {
                if (cell.Value is not Event ev)
                {
                    ev = new Event(0, cell.Value.ToString());
                    cell.Value = ev;
                    cell.ResetStyle();
                }
                OpenEventEdit(ev.ID, cell);
            }
            else
            {
                pbDrawnTable.Table.Cells.Remove(cell);
            }
        }

        private void Table_CellOverlapPlaceholderClick(object sender, CellOverlapEventArgs e)
        {
            List<Event> events = e.OverlappingCells.Select(oc => oc.Value as Event).ToList();
            MessageBox.Show($"{events.Count} events are overlaping", "Overlapped cell clicked");
        }

        private void Table_CellCreating(object sender, CellChangedEventArgs e)
        {
            string value = "+"; // Release mouse to create new cell
            pbDrawnTable.Table.Cells.RemoveByValue(value);

            DrawnTableCell cell = pbDrawnTable.Table.Cells[e.Location];
            cell.Brush = new SolidBrush(Color.FromArgb(125, pbDrawnTable.Table.Cells[e.Location].Brush.Color));
            cell.Alignment = StringAlignment.Center;
            cell.LineAlignment = StringAlignment.Center;
            cell.Value = value;
        }

        void OpenEventEdit(int eid, DrawnTableCell cell = null, CellLocation? movedFrom = null)
        {

        }

        private DrawnTableCell CreateCell(Event e, HeaderCreator headers, TStyle style)
        {
            DrawnTableCell cell;
            switch (style)
            {
                case TStyle.DayTime:
                {
                    int col = headers.Day.GetIndexByValue(e.Date);
                    double row = headers.Time.GetIndexByValue(e.Start);
                    double rowEnd = headers.Time.GetIndexByValue(e.End);
                    AdjustCell(ref row, ref rowEnd);
                    cell = new DrawnTableCell(new CellLocation((int)row, col), e, (int)rowEnd - (int)row);
                    break;
                }
                case TStyle.DayList:
                {
                    int col = headers.Day.GetIndexByValue(e.Date);
                    DrawnTableHeader header = headers.Custom1.Headers.First(h => (int)(h.Tag ?? -1) != col);
                    header.Tag = col;
                    int row = int.Parse(header.Text) - 1;
                    cell = new DrawnTableCell(new CellLocation(row, col), e);
                    break;
                }
                case TStyle.LocationTime:
                {
                    if (e.RoomId is null)
                    {
                        return null;
                    }
                    int col = GetRoomTimeColumnIndex(e.Date, e.RoomId.Value);
                    double rol = headers.Time.GetIndexByValue(e.Start);
                    double rowEnd = headers.Time.GetIndexByValue(e.End);
                    if (Math.Floor(rol) == Math.Ceiling(rowEnd))
                    {
                        if (Math.Round(rol) == Math.Round(rowEnd))
                        {
                            rol = Math.Ceiling(rol);
                            rowEnd = Math.Floor(rowEnd);
                        }
                        else
                        {
                            rol = Math.Round(rol);
                            rowEnd = Math.Round(rowEnd);
                        }
                    }
                    else
                    {
                        rol = Math.Floor(rol);
                        rowEnd = Math.Ceiling(rowEnd);
                    }
                    cell = new DrawnTableCell(new CellLocation((int)rol, col), e, (int)rowEnd - (int)rol);
                    break;
                }
                default:
                    return null;
            }

            cell.Brush = new SolidBrush(e.Color);
            cell.Enabled = e.Enabled;
            return cell;
        }

        private void AdjustCell(TimeSpan start, TimeSpan end, out int timeFrom, out int timeTo)
        {
            double _timeFrom = headers.Time.GetIndexByValue(start);
            double _timeTo = headers.Time.GetIndexByValue(end);
            _timeFrom = Math.Max(0, _timeFrom);
            _timeTo = Math.Min(pbDrawnTable.Table.RowCount(), _timeTo);
            AdjustCell(ref _timeFrom, ref _timeTo);

            timeFrom = (int)_timeFrom;
            timeTo = (int)_timeTo;
        }

        private static void AdjustCell(ref double start, ref double end)
        {
            if (Math.Floor(start) == Math.Ceiling(end))
            {
                if (Math.Round(start) == Math.Round(end))
                {
                    start = Math.Ceiling(start);
                    end = Math.Floor(end);
                }
                else
                {
                    start = Math.Round(start);
                    end = Math.Round(end);
                }
            }
            else
            {
                start = Math.Floor(start);
                end = Math.Ceiling(end);
            }
        }

        private int GetRoomTimeColumnIndex(DateTime day, int roomId)
        {
            int col;
            if (cbLTGroupBy.SelectedIndex == 0)
            {
                col = HeaderCreator.GetRealIndex(headers.Day.LastGeneratedHeaders, headers.Day.GetIndexByValue(day), headers.Custom1.GetIndexByHeaderTag(roomId));
            }
            else
            {
                col = HeaderCreator.GetRealIndex(headers.Custom1.Headers, headers.Custom1.GetIndexByHeaderTag(roomId), headers.Day.GetIndexByValue(day));
            }

            return col;
        }

        TStyle GetStyle()
        {
            if (rbDayTime.Checked)
            {
                return TStyle.DayTime;
            }
            else if (rbDayList.Checked)
            {
                return TStyle.DayList;
            }
            else if (rbLocationTime.Checked)
            {
                return TStyle.LocationTime;
            }
            return TStyle.None;
        }

        private void bPrint_Click(object sender, EventArgs e)
        {
            PrintDialog printDialog = new()
            {
                UseEXDialog = true,
                Document = printDocument1
            };

            DialogResult dialogResult = printDialog.ShowDialog();
            if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
            {
                return;
            }

            printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Rectangle area = e.PageBounds;
            area.Inflate(-25, -25);
            using Graphics g = e.Graphics;

            // Print footer
            Font footerFont = Font;
            SolidBrush footerColor = new(Color.Gray);

            string date = DateTime.Now.ToShortDateString();
            SizeF dSize = g.MeasureString(date, footerFont, area.Width);
            g.DrawString(date, footerFont, footerColor, area.Right - dSize.Width, area.Bottom - dSize.Height);
            area.Height -= (int)(dSize.Height + 15);

            // Print header
            Font headerFont = new Font("Segoe UI", 14.25F);
            SolidBrush headerColor = new(Color.DimGray);

            string header = "DrawnTable.Demo";
            SizeF hSize = g.MeasureString(header, headerFont, area.Width);
            g.DrawString(header, headerFont, headerColor, area.X + (area.Width - hSize.Width) / 2, area.Y);
            area.Y += (int)(hSize.Height + 20);
            area.Height -= (int)(hSize.Height + 20);

            // Print table
            pbDrawnTable.Table.Printing.PrintPaint(g, area);
        }
    }
}
