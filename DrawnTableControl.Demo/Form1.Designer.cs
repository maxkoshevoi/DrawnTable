
namespace DrawnTableControl.Demo
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbDrawnTable = new DrawnTableControl.PBDrawnTable();
            this.chHideEmptyColumns = new System.Windows.Forms.CheckBox();
            this.pViewDays = new System.Windows.Forms.FlowLayoutPanel();
            this.chViewDaysMd = new System.Windows.Forms.CheckBox();
            this.chViewDaysTu = new System.Windows.Forms.CheckBox();
            this.chViewDaysWd = new System.Windows.Forms.CheckBox();
            this.chViewDaysTh = new System.Windows.Forms.CheckBox();
            this.chViewDaysFr = new System.Windows.Forms.CheckBox();
            this.chViewDaysSa = new System.Windows.Forms.CheckBox();
            this.chViewDaysSu = new System.Windows.Forms.CheckBox();
            this.gLayout = new System.Windows.Forms.GroupBox();
            this.cbLTGroupBy = new System.Windows.Forms.ComboBox();
            this.rbLocationTime = new System.Windows.Forms.RadioButton();
            this.rbDayList = new System.Windows.Forms.RadioButton();
            this.rbDayTime = new System.Windows.Forms.RadioButton();
            this.gWeekDays = new System.Windows.Forms.GroupBox();
            this.bPrint = new System.Windows.Forms.Button();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.chColorPast = new System.Windows.Forms.CheckBox();
            this.chColorWeekends = new System.Windows.Forms.CheckBox();
            this.chDragDrop = new System.Windows.Forms.CheckBox();
            this.chAllowCreateNewCell = new System.Windows.Forms.CheckBox();
            this.numEventsCount = new System.Windows.Forms.NumericUpDown();
            this.bGenerateEvents = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbDrawnTable)).BeginInit();
            this.pViewDays.SuspendLayout();
            this.gLayout.SuspendLayout();
            this.gWeekDays.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEventsCount)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbDrawnTable
            // 
            this.pbDrawnTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbDrawnTable.Image = null;
            this.pbDrawnTable.Location = new System.Drawing.Point(220, 12);
            this.pbDrawnTable.Name = "pbDrawnTable";
            this.pbDrawnTable.Size = new System.Drawing.Size(891, 585);
            this.pbDrawnTable.TabIndex = 0;
            this.pbDrawnTable.TabStop = false;
            // 
            // chHideEmptyColumns
            // 
            this.chHideEmptyColumns.AutoSize = true;
            this.chHideEmptyColumns.Location = new System.Drawing.Point(10, 431);
            this.chHideEmptyColumns.Name = "chHideEmptyColumns";
            this.chHideEmptyColumns.Size = new System.Drawing.Size(137, 19);
            this.chHideEmptyColumns.TabIndex = 3;
            this.chHideEmptyColumns.Text = "Hide empty columns";
            this.chHideEmptyColumns.UseVisualStyleBackColor = true;
            this.chHideEmptyColumns.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // pViewDays
            // 
            this.pViewDays.Controls.Add(this.chViewDaysMd);
            this.pViewDays.Controls.Add(this.chViewDaysTu);
            this.pViewDays.Controls.Add(this.chViewDaysWd);
            this.pViewDays.Controls.Add(this.chViewDaysTh);
            this.pViewDays.Controls.Add(this.chViewDaysFr);
            this.pViewDays.Controls.Add(this.chViewDaysSa);
            this.pViewDays.Controls.Add(this.chViewDaysSu);
            this.pViewDays.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pViewDays.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pViewDays.Location = new System.Drawing.Point(3, 19);
            this.pViewDays.Name = "pViewDays";
            this.pViewDays.Size = new System.Drawing.Size(195, 177);
            this.pViewDays.TabIndex = 4;
            // 
            // chViewDaysMd
            // 
            this.chViewDaysMd.AutoSize = true;
            this.chViewDaysMd.Checked = true;
            this.chViewDaysMd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chViewDaysMd.Location = new System.Drawing.Point(3, 3);
            this.chViewDaysMd.Name = "chViewDaysMd";
            this.chViewDaysMd.Size = new System.Drawing.Size(70, 19);
            this.chViewDaysMd.TabIndex = 0;
            this.chViewDaysMd.Tag = "1";
            this.chViewDaysMd.Text = "Monday";
            this.chViewDaysMd.UseVisualStyleBackColor = true;
            this.chViewDaysMd.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chViewDaysTu
            // 
            this.chViewDaysTu.AutoSize = true;
            this.chViewDaysTu.Checked = true;
            this.chViewDaysTu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chViewDaysTu.Location = new System.Drawing.Point(3, 28);
            this.chViewDaysTu.Name = "chViewDaysTu";
            this.chViewDaysTu.Size = new System.Drawing.Size(69, 19);
            this.chViewDaysTu.TabIndex = 1;
            this.chViewDaysTu.Tag = "2";
            this.chViewDaysTu.Text = "Tuesday";
            this.chViewDaysTu.UseVisualStyleBackColor = true;
            this.chViewDaysTu.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chViewDaysWd
            // 
            this.chViewDaysWd.AutoSize = true;
            this.chViewDaysWd.Checked = true;
            this.chViewDaysWd.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chViewDaysWd.Location = new System.Drawing.Point(3, 53);
            this.chViewDaysWd.Name = "chViewDaysWd";
            this.chViewDaysWd.Size = new System.Drawing.Size(87, 19);
            this.chViewDaysWd.TabIndex = 2;
            this.chViewDaysWd.Tag = "3";
            this.chViewDaysWd.Text = "Wednesday";
            this.chViewDaysWd.UseVisualStyleBackColor = true;
            this.chViewDaysWd.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chViewDaysTh
            // 
            this.chViewDaysTh.AutoSize = true;
            this.chViewDaysTh.Checked = true;
            this.chViewDaysTh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chViewDaysTh.Location = new System.Drawing.Point(3, 78);
            this.chViewDaysTh.Name = "chViewDaysTh";
            this.chViewDaysTh.Size = new System.Drawing.Size(74, 19);
            this.chViewDaysTh.TabIndex = 3;
            this.chViewDaysTh.Tag = "4";
            this.chViewDaysTh.Text = "Thursday";
            this.chViewDaysTh.UseVisualStyleBackColor = true;
            this.chViewDaysTh.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chViewDaysFr
            // 
            this.chViewDaysFr.AutoSize = true;
            this.chViewDaysFr.Checked = true;
            this.chViewDaysFr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chViewDaysFr.Location = new System.Drawing.Point(3, 103);
            this.chViewDaysFr.Name = "chViewDaysFr";
            this.chViewDaysFr.Size = new System.Drawing.Size(58, 19);
            this.chViewDaysFr.TabIndex = 4;
            this.chViewDaysFr.Tag = "5";
            this.chViewDaysFr.Text = "Friday";
            this.chViewDaysFr.UseVisualStyleBackColor = true;
            this.chViewDaysFr.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chViewDaysSa
            // 
            this.chViewDaysSa.AutoSize = true;
            this.chViewDaysSa.Checked = true;
            this.chViewDaysSa.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chViewDaysSa.Location = new System.Drawing.Point(3, 128);
            this.chViewDaysSa.Name = "chViewDaysSa";
            this.chViewDaysSa.Size = new System.Drawing.Size(72, 19);
            this.chViewDaysSa.TabIndex = 5;
            this.chViewDaysSa.Tag = "6";
            this.chViewDaysSa.Text = "Saturday";
            this.chViewDaysSa.UseVisualStyleBackColor = true;
            this.chViewDaysSa.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chViewDaysSu
            // 
            this.chViewDaysSu.AutoSize = true;
            this.chViewDaysSu.Checked = true;
            this.chViewDaysSu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chViewDaysSu.Location = new System.Drawing.Point(3, 153);
            this.chViewDaysSu.Name = "chViewDaysSu";
            this.chViewDaysSu.Size = new System.Drawing.Size(65, 19);
            this.chViewDaysSu.TabIndex = 6;
            this.chViewDaysSu.Tag = "0";
            this.chViewDaysSu.Text = "Sunday";
            this.chViewDaysSu.UseVisualStyleBackColor = true;
            this.chViewDaysSu.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // gLayout
            // 
            this.gLayout.Controls.Add(this.cbLTGroupBy);
            this.gLayout.Controls.Add(this.rbLocationTime);
            this.gLayout.Controls.Add(this.rbDayList);
            this.gLayout.Controls.Add(this.rbDayTime);
            this.gLayout.Location = new System.Drawing.Point(10, 71);
            this.gLayout.Name = "gLayout";
            this.gLayout.Size = new System.Drawing.Size(201, 99);
            this.gLayout.TabIndex = 5;
            this.gLayout.TabStop = false;
            this.gLayout.Text = "Layout";
            // 
            // cbLTGroupBy
            // 
            this.cbLTGroupBy.AutoCompleteCustomSource.AddRange(new string[] {
            "Day",
            "Location"});
            this.cbLTGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLTGroupBy.FormattingEnabled = true;
            this.cbLTGroupBy.Items.AddRange(new object[] {
            "Day",
            "Location"});
            this.cbLTGroupBy.Location = new System.Drawing.Point(120, 68);
            this.cbLTGroupBy.Name = "cbLTGroupBy";
            this.cbLTGroupBy.Size = new System.Drawing.Size(75, 23);
            this.cbLTGroupBy.TabIndex = 10;
            this.cbLTGroupBy.SelectedIndexChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // rbLocationTime
            // 
            this.rbLocationTime.AutoSize = true;
            this.rbLocationTime.Location = new System.Drawing.Point(6, 72);
            this.rbLocationTime.Name = "rbLocationTime";
            this.rbLocationTime.Size = new System.Drawing.Size(108, 19);
            this.rbLocationTime.TabIndex = 9;
            this.rbLocationTime.Text = "Location - Time";
            this.rbLocationTime.UseVisualStyleBackColor = true;
            this.rbLocationTime.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // rbDayList
            // 
            this.rbDayList.AutoSize = true;
            this.rbDayList.Location = new System.Drawing.Point(6, 47);
            this.rbDayList.Name = "rbDayList";
            this.rbDayList.Size = new System.Drawing.Size(74, 19);
            this.rbDayList.TabIndex = 8;
            this.rbDayList.Text = "Day - List";
            this.rbDayList.UseVisualStyleBackColor = true;
            this.rbDayList.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // rbDayTime
            // 
            this.rbDayTime.AutoSize = true;
            this.rbDayTime.Checked = true;
            this.rbDayTime.Location = new System.Drawing.Point(6, 22);
            this.rbDayTime.Name = "rbDayTime";
            this.rbDayTime.Size = new System.Drawing.Size(82, 19);
            this.rbDayTime.TabIndex = 7;
            this.rbDayTime.TabStop = true;
            this.rbDayTime.Text = "Day - Time";
            this.rbDayTime.UseVisualStyleBackColor = true;
            this.rbDayTime.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // gWeekDays
            // 
            this.gWeekDays.Controls.Add(this.pViewDays);
            this.gWeekDays.Location = new System.Drawing.Point(10, 176);
            this.gWeekDays.Name = "gWeekDays";
            this.gWeekDays.Size = new System.Drawing.Size(201, 199);
            this.gWeekDays.TabIndex = 6;
            this.gWeekDays.TabStop = false;
            this.gWeekDays.Text = "Week days to display";
            // 
            // bPrint
            // 
            this.bPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bPrint.Location = new System.Drawing.Point(12, 574);
            this.bPrint.Name = "bPrint";
            this.bPrint.Size = new System.Drawing.Size(196, 23);
            this.bPrint.TabIndex = 7;
            this.bPrint.Text = "Print";
            this.bPrint.UseVisualStyleBackColor = true;
            this.bPrint.Click += new System.EventHandler(this.bPrint_Click);
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(7, 463);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 74);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Style";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.chColorPast);
            this.flowLayoutPanel1.Controls.Add(this.chColorWeekends);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(189, 52);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // chColorPast
            // 
            this.chColorPast.AutoSize = true;
            this.chColorPast.Location = new System.Drawing.Point(3, 3);
            this.chColorPast.Name = "chColorPast";
            this.chColorPast.Size = new System.Drawing.Size(80, 19);
            this.chColorPast.TabIndex = 2;
            this.chColorPast.Text = "Color past";
            this.chColorPast.UseVisualStyleBackColor = true;
            this.chColorPast.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chColorWeekends
            // 
            this.chColorWeekends.AutoSize = true;
            this.chColorWeekends.Checked = true;
            this.chColorWeekends.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chColorWeekends.Location = new System.Drawing.Point(3, 28);
            this.chColorWeekends.Name = "chColorWeekends";
            this.chColorWeekends.Size = new System.Drawing.Size(110, 19);
            this.chColorWeekends.TabIndex = 1;
            this.chColorWeekends.Text = "Color weekends";
            this.chColorWeekends.UseVisualStyleBackColor = true;
            this.chColorWeekends.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chDragDrop
            // 
            this.chDragDrop.AutoSize = true;
            this.chDragDrop.Checked = true;
            this.chDragDrop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chDragDrop.Location = new System.Drawing.Point(10, 381);
            this.chDragDrop.Name = "chDragDrop";
            this.chDragDrop.Size = new System.Drawing.Size(141, 19);
            this.chDragDrop.TabIndex = 9;
            this.chDragDrop.Text = "Enable Drag and Drop";
            this.chDragDrop.UseVisualStyleBackColor = true;
            this.chDragDrop.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // chAllowCreateNewCell
            // 
            this.chAllowCreateNewCell.AutoSize = true;
            this.chAllowCreateNewCell.Checked = true;
            this.chAllowCreateNewCell.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chAllowCreateNewCell.Location = new System.Drawing.Point(10, 406);
            this.chAllowCreateNewCell.Name = "chAllowCreateNewCell";
            this.chAllowCreateNewCell.Size = new System.Drawing.Size(142, 19);
            this.chAllowCreateNewCell.TabIndex = 10;
            this.chAllowCreateNewCell.Text = "Allow create new cells";
            this.chAllowCreateNewCell.UseVisualStyleBackColor = true;
            this.chAllowCreateNewCell.CheckedChanged += new System.EventHandler(this.Setting_Changed);
            // 
            // numEventsCount
            // 
            this.numEventsCount.Location = new System.Drawing.Point(6, 22);
            this.numEventsCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numEventsCount.Name = "numEventsCount";
            this.numEventsCount.Size = new System.Drawing.Size(107, 23);
            this.numEventsCount.TabIndex = 11;
            this.numEventsCount.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // bGenerateEvents
            // 
            this.bGenerateEvents.Location = new System.Drawing.Point(119, 20);
            this.bGenerateEvents.Name = "bGenerateEvents";
            this.bGenerateEvents.Size = new System.Drawing.Size(75, 25);
            this.bGenerateEvents.TabIndex = 13;
            this.bGenerateEvents.Text = "Generate";
            this.bGenerateEvents.UseVisualStyleBackColor = true;
            this.bGenerateEvents.Click += new System.EventHandler(this.bGenerateEvents_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.bGenerateEvents);
            this.groupBox2.Controls.Add(this.numEventsCount);
            this.groupBox2.Location = new System.Drawing.Point(10, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 53);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Generate events";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1123, 609);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.chAllowCreateNewCell);
            this.Controls.Add(this.chDragDrop);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bPrint);
            this.Controls.Add(this.gWeekDays);
            this.Controls.Add(this.gLayout);
            this.Controls.Add(this.chHideEmptyColumns);
            this.Controls.Add(this.pbDrawnTable);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DrawnTableControl.Demo";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbDrawnTable)).EndInit();
            this.pViewDays.ResumeLayout(false);
            this.pViewDays.PerformLayout();
            this.gLayout.ResumeLayout(false);
            this.gLayout.PerformLayout();
            this.gWeekDays.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numEventsCount)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DrawnTableControl.PBDrawnTable pbDrawnTable;
        private System.Windows.Forms.CheckBox chHideEmptyColumns;
        private System.Windows.Forms.FlowLayoutPanel pViewDays;
        private System.Windows.Forms.CheckBox chViewDaysMd;
        private System.Windows.Forms.CheckBox chViewDaysTu;
        private System.Windows.Forms.CheckBox chViewDaysWd;
        private System.Windows.Forms.CheckBox chViewDaysTh;
        private System.Windows.Forms.CheckBox chViewDaysFr;
        private System.Windows.Forms.CheckBox chViewDaysSa;
        private System.Windows.Forms.CheckBox chViewDaysSu;
        private System.Windows.Forms.GroupBox gLayout;
        private System.Windows.Forms.GroupBox gWeekDays;
        private System.Windows.Forms.RadioButton rbLocationTime;
        private System.Windows.Forms.RadioButton rbDayTime;
        private System.Windows.Forms.RadioButton rbDayList;
        private System.Windows.Forms.ComboBox cbLTGroupBy;
        private System.Windows.Forms.Button bPrint;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chColorWeekends;
        private System.Windows.Forms.CheckBox chDragDrop;
        private System.Windows.Forms.CheckBox chAllowCreateNewCell;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox chColorPast;
        private System.Windows.Forms.NumericUpDown numEventsCount;
        private System.Windows.Forms.Button bGenerateEvents;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

