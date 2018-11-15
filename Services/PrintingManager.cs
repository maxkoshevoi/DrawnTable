﻿using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace DrawnTableControl.Services
{
    public class PrintingManager : IDisposable
    {
        private DrawnTable table;
        private PrintDocument printDocument;
        public PrintDocument PrintDocument
        {
            set
            {
                printDocument = value;
            }
            get
            {
                if (printDocument == null)
                {
                    printDocument = new PrintDocument();
                    printDocument.DefaultPageSettings.Landscape = true;
                    printDocument.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                }
                return printDocument;
            }
        }

        internal PrintingManager(DrawnTable table)
        {
            this.table = table ?? throw new ArgumentNullException(nameof(table));
        }

        /// <summary>
		/// Draws chart on the printer graphics.
		/// </summary>
        /// <param name="graphics">Printer graphics.</param>
		/// <param name="area">Position to draw in the graphics.</param>
        public void PrintPaint(Graphics graphics, Rectangle area)
        {
            RectangleF lastArea = table.TableArea;
            table.ActualRedraw(graphics, area);
            table.Resize(lastArea);
        }

        /// <summary>
		/// Shows Page Setup dialog.
		/// </summary>
		public void PageSetup()
        {
            PageSetupDialog pageSetupDialog = new PageSetupDialog
            {
                Document = PrintDocument
            };
            pageSetupDialog.ShowDialog();
        }

        /// <summary>
		/// Print preview the table.
		/// </summary>
		public void PrintPreview()
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog
            {
                Document = PrintDocument
            };
            printPreviewDialog.ShowDialog();
        }

        /// <summary>
		/// Prints chart.
		/// </summary>
		/// <param name="showPrintDialog">Indicates if printing dialog should be shown.</param>
		public void Print(bool showPrintDialog)
        {
            // Show Print dialog
            if (showPrintDialog)
            {
                // Create and show Print dialog
                PrintDialog printDialog = new PrintDialog
                {
                    UseEXDialog = true,
                    Document = PrintDocument
                };
                DialogResult dialogResult = printDialog.ShowDialog();

                // Do not proceed with printing if OK button was not pressed
                if (dialogResult != DialogResult.OK && dialogResult != DialogResult.Yes)
                {
                    return;
                }
            }

            // Print table
            PrintDocument.Print();
        }

        /// <summary>
		/// Handles PrintPage event of the document.
		/// </summary>
		/// <param name="sender">Sender object.</param>
		/// <param name="ev">Event parameters.</param>
		private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (table.IsEnabled)
            {
                try
                {
                    Rectangle position = ev.MarginBounds;

                    // Draw chart on the printer graphisc
                    PrintPaint(ev.Graphics, position);
                }
                finally
                {
                }
            }
        }

        public void Dispose()
        {
            printDocument?.Dispose();
        }
    }
}
