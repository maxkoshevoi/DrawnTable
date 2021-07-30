using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;
using DrawnTableControl.Demo.Models;
using DrawnTableControl.Enums;
using DrawnTableControl.EventArguments;
using DrawnTableControl.HeaderHelpers;
using DrawnTableControl.Models;

namespace DrawnTableControl.Demo
{
    public partial class Form1 : Form
    {
        private readonly HeaderCreator headers = new();

        private readonly List<Event> events = new();
        private readonly IReadOnlyList<Location> locations = new Location[]
        {
            new(1, "Room1", Color.PaleVioletRed),
            new(2, "Room2", Color.LightSeaGreen),
            new(3, "Room3", Color.ForestGreen)
        };
        private readonly  DateTime dayStart;
        private readonly  DateTime dayEnd;

        private readonly Color weekendColor = Color.FromArgb(75, Color.LightSeaGreen);
        private readonly Color pastColor = Color.FromArgb(25, Color.Gray);

        private enum TStyle 
        { 
            None, 
            DayTime, 
            LocationTime,
            DayList 
        }

        public Form1()
        {
            InitializeComponent();

            dayStart = headers.Day.GetWeeksMonday(DateTime.Now);
            dayEnd = dayStart.AddDays(6);

            cbLTGroupBy.SelectedIndex = 1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Font defaultFont = pbDrawnTable.Table.GetDefaultFont();
            pbDrawnTable.Font = new Font(defaultFont.FontFamily, 10);

            GenerateEvents();
        }

        private void bGenerateEvents_Click(object sender, EventArgs e)
        {
            GenerateEvents();
        }

        private void Setting_Changed(object sender, EventArgs e)
        {
            UpdateTable();
        }

        private void InitTable(List<DrawnTableHeader> Rows, List<DrawnTableHeader> Cols)
        {
            pbDrawnTable.Table.Create(Rows, Cols);
            pbDrawnTable.Table.CellCreating += Table_CellCreating;
            pbDrawnTable.Table.CellCreated += Table_CellCreated;
            pbDrawnTable.Table.CellWithValueClick += Table_CellWithValueClick;
            pbDrawnTable.Table.CellOverlapPlaceholderClick += Table_CellOverlapPlaceholderClick;
            pbDrawnTable.Table.CellDragDropFinished += Table_CellDragDropFinished;
            pbDrawnTable.Table.CellCopied += Table_CellCopied;
            pbDrawnTable.Table.CellPasted += Table_CellPasted;
            pbDrawnTable.Table.AllowCreateNewCells = chAllowCreateNewCell.Checked;
            pbDrawnTable.Table.IfCellsOverlap = DrawnTableOverlapOptions.ReplaceWithCounter;
            if (chDragDrop.Checked)
            {
                pbDrawnTable.Table.AllowDragDrop = true;
                pbDrawnTable.Table.CellCopyMode = DrawnTableCopyMode.CtrlAndDrag;
            }
            else
            {
                pbDrawnTable.Table.CellCopyMode = DrawnTableCopyMode.None;
                pbDrawnTable.Table.AllowDragDrop = false;
            }
        }

        private void UpdateTable()
        {
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
                    Rows = headers.Time.GenerateHeaders(TimeSpan.FromHours(8), TimeSpan.FromHours(18), TimeSpan.FromHours(1), 4).ToList();
                    if (style == TStyle.DayTime)
                    {
                        Cols = headers.Day.GenerateHeaders(dayStart, dayEnd, colFilter, "dd, dddd").ToList();
                    }
                    else
                    {
                        headers.Custom1.Clear();
                        headers.Custom1.AddRange(locations.Select(l => (l.Name, (object)l.Id)));

                        if (headers.Custom1.Count > 0)
                        {
                            IEnumerable<DrawnTableHeader> dayHeaders = headers.Day.GenerateHeaders(dayStart, dayEnd, colFilter, "dd, dddd");
                            if (cbLTGroupBy.SelectedIndex == 0)
                            {
                                Cols = dayHeaders.ToList();
                                Cols.ForEach(c => c.Subheaders.AddRange(headers.Custom1.Headers));
                            }
                            else
                            {
                                Cols = headers.Custom1.Headers;
                                Cols.ForEach(c => c.Subheaders.AddRange(dayHeaders));
                            }
                        }
                    }
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

            if (chColorWeekends.Checked)
            {
                ColorDateRange(dayStart, dayEnd, dayEnd.AddDays(-1), dayEnd, weekendColor);
            }
            if (chColorPast.Checked)
            {
                ColorPast(dayStart, dayEnd, pastColor);
            }

            foreach (var e in events.OrderBy(e => e.Date).ThenBy(e => e.Start))
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

        #region Table events
        private void Table_CellPasted(object sender, CellPastedEventArgs e)
        {
            UpdateEventBasedOnTableLocation(pbDrawnTable.Table.Cells[e.Location], e.CopiedFrom.Location);
        }

        private void Table_CellCopied(object sender, CellCopiedEventArgs e)
        {
            Event ev = (Event)e.CopiedCell.Value;
            Event copy = (Event)ev.Clone();
            events.Add(copy);

            e.CopiedCell.Value = copy;
        }

        private void Table_CellDragDropFinished(object sender, CellMovedEventArgs e)
        {
            UpdateEventBasedOnTableLocation(pbDrawnTable.Table.Cells[e.Location], e.OldLocation);
        }

        private void Table_CellWithValueClick(object sender, CellClickEventArgs e)
        {
            DrawnTableCell cell = pbDrawnTable.Table.Cells[e.CellLocation];
            if (e.Button == MouseButtons.Left)
            {
                MessageBox.Show(cell.Value.ToString(), "Event clicked");
            }
            else
            {
                pbDrawnTable.Table.Cells.Remove(cell);
                events.Remove((Event)cell.Value);
            }
        }

        private void Table_CellOverlapPlaceholderClick(object sender, CellOverlapEventArgs e)
        {
            List<Event> events = e.OverlappingCells.Select(oc => (Event)oc.Value).ToList();
            MessageBox.Show($"{events.Count} events are overlapping", "Overlapped cell clicked");
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

        private void Table_CellCreated(object sender, CellChangedEventArgs e)
        {
            Event ev = new();
            events.Add(ev);

            DrawnTableCell cell = pbDrawnTable.Table.Cells[e.Location];
            cell.Value = ev;
            cell.ResetStyle();
            UpdateEventBasedOnTableLocation(cell);
        }

        private void UpdateEventBasedOnTableLocation(DrawnTableCell cell, CellLocation? movedFrom = null)
        {
            Event ev = (Event)cell.Value;

            // Obtaining event information based on its location in the table
            TimeSpan newStart, newEnd;
            newEnd = newStart = TimeSpan.FromTicks(-1);
            switch (GetStyle())
            {
                case TStyle.DayTime:
                    ev.Date = headers.Day[cell.Location.Column];
                    newStart = headers.Time[cell.Location.Row];
                    newEnd = headers.Time[cell.Location.Row + cell.Rowspan];
                    break;
                case TStyle.LocationTime:
                    newStart = headers.Time[cell.Location.Row];
                    newEnd = headers.Time[cell.Location.Row + cell.Rowspan];
                    int lid;
                    if (cbLTGroupBy.SelectedIndex == 0)
                    {
                        var (Header, Subheader) = headers.GetHeadersByRealIndex(headers.Day.LastGeneratedHeaders, cell.Location.Column);
                        ev.Date = (DateTime)Header.Tag;
                        lid = (int)Subheader.Tag;
                    }
                    else
                    {
                        var (Header, Subheader) = headers.GetHeadersByRealIndex(headers.Custom1.Headers, cell.Location.Column);
                        lid = (int)Header.Tag;
                        ev.Date = (DateTime)Subheader.Tag;
                    }
                    ev.Location = locations.SingleOrDefault(l => l.Id == lid);
                    break;
                case TStyle.DayList:
                    ev.Date = headers.Day[cell.Location.Column];
                    break;
                default:
                    throw new NotSupportedException("Something wrong");
            }

            if (newStart.Ticks > 0)
            {
                if (movedFrom == null)
                {
                    ev.Start = newStart;
                    ev.End = newEnd;
                }
                else if (cell.Location.Row != movedFrom.Value.Row)
                {
                    TimeSpan duration = ev.End - ev.Start;
                    ev.Start = newStart;
                    ev.End = ev.Start + duration;
                }
            }

            cell.Value = ev;
            if (ev.Color != null)
            {
                cell.Brush = new SolidBrush(ev.Color.Value);
            }
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
                    (row, rowEnd) = AdjustCell(row, rowEnd);
                    cell = new DrawnTableCell(new CellLocation((int)row, col), e, (int)rowEnd - (int)row);
                    break;
                }
                case TStyle.DayList:
                {
                    int row = -1;
                    int col = headers.Day.GetIndexByValue(e.Date);
                    if (col >= 0)
                    {
                        DrawnTableHeader header = headers.Custom1.Headers.First(h => (int)(h.Tag ?? -1) != col);
                        header.Tag = col;
                        row = int.Parse(header.Text) - 1;
                    }
                    cell = new DrawnTableCell(new CellLocation(row, col), e);
                    break;
                }
                case TStyle.LocationTime:
                {
                    if (e.Location is null)
                    {
                        return null;
                    }
                    int col = GetLocationTimeColumnIndex(e.Date, e.Location.Id);
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

            if (cell.Location.Column < 0 || cell.Location.Column >= pbDrawnTable.Table.ColumnCount()
                || cell.Location.Row < 0 || cell.Location.Row >= pbDrawnTable.Table.RowCount())
            {
                return null;
            }

            if (e.Color != null)
            {
                cell.Brush = e.Enabled ? new SolidBrush(e.Color.Value) : new SolidBrush(Color.FromArgb(100, e.Color.Value));
            }
            cell.Enabled = e.Enabled;
            return cell;
        }
        #endregion

        #region Cell location utils
        private (int start, int end) AdjustCell(TimeSpan start, TimeSpan end)
        {
            double timeFrom = headers.Time.GetIndexByValue(start);
            double timeTo = headers.Time.GetIndexByValue(end);
            timeFrom = Math.Max(0, timeFrom);
            timeTo = Math.Min(pbDrawnTable.Table.RowCount(), timeTo);
            
            return AdjustCell(timeFrom, timeTo);
        }

        private static (int start, int end) AdjustCell(double start, double end)
        {
            if (Math.Floor(start) == Math.Ceiling(end))
            {
                if (Math.Round(start) == Math.Round(end))
                {
                    return ((int)Math.Ceiling(start), (int)Math.Floor(end));
                }

                return ((int)Math.Round(start), (int)Math.Round(end));
            }

            return ((int)Math.Floor(start), (int)Math.Ceiling(end));
        }

        private int GetLocationTimeColumnIndex(DateTime day, int locationId)
        {
            int col;
            if (cbLTGroupBy.SelectedIndex == 0)
            {
                col = headers.GetRealIndex(headers.Day.LastGeneratedHeaders, headers.Day.GetIndexByValue(day), headers.Custom1.GetIndexByHeaderTag(locationId));
            }
            else
            {
                col = headers.GetRealIndex(headers.Custom1.Headers, headers.Custom1.GetIndexByHeaderTag(locationId), headers.Day.GetIndexByValue(day));
            }

            return col;
        }
        #endregion

        #region Color utils
        private void ColorEvent(DateTime day, TimeSpan start, TimeSpan end, int? locationId, Color color)
        {
            TStyle style = GetStyle();

            // Getting row
            var (timeFrom, timeTo) = AdjustCell(start, end);
            if (timeFrom >= timeTo) return;

            // Getting column
            int col = style switch
            {
                TStyle.DayTime or TStyle.DayList => headers.Day.GetIndexByValue(day),
                TStyle.LocationTime when locationId.HasValue => GetLocationTimeColumnIndex(day, locationId.Value),
                _ => -1
            };
            if (col < 0) return;

            // Setting background color
            for (int i = timeFrom; i < timeTo; i++)
            {
                pbDrawnTable.Table.Cells.BackColors[i, col] = color;
            }
        }

        private void ColorPast(DateTime dayStart, DateTime dayEnd, Color pastColor)
        {
            TStyle style = GetStyle();

            if (dayStart > DateTime.Today
                || pbDrawnTable.Table.RowCount() == 0
                || pbDrawnTable.Table.ColumnCount() == 0)
            {
                return;
            }

            if (dayEnd < DateTime.Today)
            {
                pbDrawnTable.Table.Cells.BackColors.SetAll(pastColor);
                return;
            }

            int locationCount = 1;
            if (style == TStyle.LocationTime && cbLTGroupBy.SelectedIndex == 0)
            {
                locationCount = headers.Custom1.Headers.Count;
            }

            int to = pbDrawnTable.Table.ColumnCount() - 1;
            int timeTo = pbDrawnTable.Table.RowCount() - 1;

            to = Math.Max(0, headers.Day.GetNearestIndexByDay(DateTime.Today, d => DateTime.Today >= d) * locationCount) - 1;
            if (style == TStyle.DayTime || style == TStyle.DayList || (style == TStyle.LocationTime && cbLTGroupBy.SelectedIndex == 0))
            {
                for (int i = 0; i <= to; i++)
                {
                    pbDrawnTable.Table.Cells.BackColors.SetColumn(i, pastColor);
                }

                if (style == TStyle.DayTime || style == TStyle.LocationTime)
                {
                    for (int r = 0; r <= timeTo; r++)
                    {
                        TimeSpan time = headers.Time[r + 1];
                        if (DateTime.Now.TimeOfDay < time)
                        {
                            break;
                        }
                        for (int i = 1; i <= locationCount; i++)
                        {
                            pbDrawnTable.Table.Cells.BackColors[r, to + i] = pastColor;
                        }
                    }
                }
            }
            else if (style == TStyle.LocationTime && cbLTGroupBy.SelectedIndex == 1)
            {
                int dayCount = headers.Day.LastGeneratedHeaders.Count;

                for (int locationIndex = 0; locationIndex < headers.Custom1.Count; locationIndex++)
                {
                    for (int dayIndex = 0; dayIndex <= to; dayIndex++)
                    {
                        pbDrawnTable.Table.Cells.BackColors.SetColumn(locationIndex * dayCount + dayIndex, pastColor);
                    }

                    for (int r = 0; r <= timeTo; r++)
                    {
                        TimeSpan time = headers.Time[r + 1];
                        if (DateTime.Now.TimeOfDay < time)
                        {
                            break;
                        }
                        pbDrawnTable.Table.Cells.BackColors[r, locationIndex * dayCount + to + 1] = pastColor;
                    }
                }
            }
        }

        private void ColorDateRange(DateTime dayStart, DateTime dayEnd, DateTime rangeStart, DateTime rangeEnd, Color color)
        {
            if (pbDrawnTable.Table.RowCount() == 0 || pbDrawnTable.Table.ColumnCount() == 0)
            {
                return;
            }

            TStyle style = GetStyle();

            int locationCount = 1;
            if (style == TStyle.LocationTime && cbLTGroupBy.SelectedIndex == 0)
            {
                locationCount = headers.Custom1.Headers.Count;
            }
            int columnCount = pbDrawnTable.Table.ColumnCount();

            int from = 0, to = columnCount - 1;
            if (dayStart < rangeStart)
            {
                from = headers.Day.GetNearestIndexByDay(rangeStart, d => rangeStart <= d) * locationCount;
            }
            if (dayEnd > rangeEnd)
            {
                to = headers.Day.GetNearestIndexByDay(rangeEnd, d => rangeEnd >= d) * locationCount + (locationCount - 1);
            }

            if (from < 0 || to < 0)
            {
                return;
            }

            if (from == 0 && to == columnCount - 1)
            {
                pbDrawnTable.Table.Cells.BackColors.SetAll(color);
                return;
            }

            if (style == TStyle.LocationTime && cbLTGroupBy.SelectedIndex == 1)
            {
                int dayCount = headers.Day.LastGeneratedHeaders.Count;

                if (to == columnCount - 1)
                {
                    to = dayCount - 1;
                }

                for (int locationIndex = 0; locationIndex < headers.Custom1.Count; locationIndex++)
                {
                    for (int dayIndex = from; dayIndex <= to; dayIndex++)
                    {
                        pbDrawnTable.Table.Cells.BackColors.SetColumn(locationIndex * dayCount + dayIndex, color);
                    }
                }
            }
            else
            {
                for (int i = from; i <= to; i++)
                {
                    pbDrawnTable.Table.Cells.BackColors.SetColumn(i, color);
                }
            }
        }
        #endregion

        #region Printing
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
            Font headerFont = new("Segoe UI", 14.25F);
            SolidBrush headerColor = new(Color.DimGray);

            string header = "DrawnTable.Demo";
            SizeF hSize = g.MeasureString(header, headerFont, area.Width);
            g.DrawString(header, headerFont, headerColor, area.X + (area.Width - hSize.Width) / 2, area.Y);
            area.Y += (int)(hSize.Height + 20);
            area.Height -= (int)(hSize.Height + 20);

            // Print table
            pbDrawnTable.Table.Printing.PrintPaint(g, area);
        }
        #endregion

        private TStyle GetStyle()
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

        private void GenerateEvents()
        {
            events.Clear();
            events.AddRange(GenerateEvents(TimeSpan.FromHours(8), TimeSpan.FromHours(18)).Take((int)numEventsCount.Value));
            UpdateTable();
        }

        private IEnumerable<Event> GenerateEvents(TimeSpan timeFrom, TimeSpan timeTo)
        {
            TimeSpan durationFrom = TimeSpan.FromMinutes(30), durationTo = TimeSpan.FromHours(2);

            Random r = new();
            int dayRange = (dayEnd - dayStart).Days;
            int startRange = (int)(timeTo - timeFrom).TotalMinutes;
            int durationRange = (int)(durationTo - durationFrom).TotalMinutes;
            int locationRange = locations.Count;

            while (true)
            {
                TimeSpan start = timeFrom.Add(TimeSpan.FromMinutes(r.Next(startRange)));
                Event ev = new()
                {
                    Date = dayStart.AddDays(r.Next(dayRange)).Date,
                    Start = start,
                    End = start.Add(durationFrom.Add(TimeSpan.FromMinutes(r.Next(durationRange)))),
                    Enabled = r.Next(100) < 70
                };

                int locationIndex = r.Next(locationRange + 1);
                if (locationIndex != locationRange)
                {
                    ev.Location = locations[locationIndex];
                }

                yield return ev;
            }
        }
    }
}
