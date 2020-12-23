namespace NBagOfTricks.Test
{
    partial class TestHost
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.vkbd = new NBagOfTricks.UI.VirtualKeyboard();
            this.chkRunBars = new System.Windows.Forms.CheckBox();
            this.barBar = new NBagOfTricks.UI.BarBar();
            this.clickGrid1 = new NBagOfTricks.UI.ClickGrid();
            this.waveViewer = new NBagOfTricks.UI.WaveViewer();
            this.timeBar = new NBagOfTricks.UI.TimeBar();
            this.chkCpu = new System.Windows.Forms.CheckBox();
            this.ftree = new NBagOfTricks.UI.FilTree();
            this.meter3 = new NBagOfTricks.UI.Meter();
            this.meter2 = new NBagOfTricks.UI.Meter();
            this.btnUT = new System.Windows.Forms.Button();
            this.cpuMeter1 = new NBagOfTricks.UI.CpuMeter();
            this.slider2 = new NBagOfTricks.UI.Slider();
            this.pan1 = new NBagOfTricks.UI.Pan();
            this.txtInfo = new NBagOfTricks.UI.TextViewer();
            this.meter1 = new NBagOfTricks.UI.Meter();
            this.pot1 = new NBagOfTricks.UI.Pot();
            this.slider1 = new NBagOfTricks.UI.Slider();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.vkbd);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chkRunBars);
            this.splitContainer1.Panel2.Controls.Add(this.barBar);
            this.splitContainer1.Panel2.Controls.Add(this.clickGrid1);
            this.splitContainer1.Panel2.Controls.Add(this.waveViewer);
            this.splitContainer1.Panel2.Controls.Add(this.timeBar);
            this.splitContainer1.Panel2.Controls.Add(this.chkCpu);
            this.splitContainer1.Panel2.Controls.Add(this.ftree);
            this.splitContainer1.Panel2.Controls.Add(this.meter3);
            this.splitContainer1.Panel2.Controls.Add(this.meter2);
            this.splitContainer1.Panel2.Controls.Add(this.btnUT);
            this.splitContainer1.Panel2.Controls.Add(this.cpuMeter1);
            this.splitContainer1.Panel2.Controls.Add(this.slider2);
            this.splitContainer1.Panel2.Controls.Add(this.pan1);
            this.splitContainer1.Panel2.Controls.Add(this.txtInfo);
            this.splitContainer1.Panel2.Controls.Add(this.meter1);
            this.splitContainer1.Panel2.Controls.Add(this.pot1);
            this.splitContainer1.Panel2.Controls.Add(this.slider1);
            this.splitContainer1.Size = new System.Drawing.Size(1436, 720);
            this.splitContainer1.SplitterDistance = 142;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 4;
            // 
            // vkbd
            // 
            this.vkbd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vkbd.Location = new System.Drawing.Point(0, 0);
            this.vkbd.Margin = new System.Windows.Forms.Padding(5);
            this.vkbd.Name = "vkbd";
            this.vkbd.ShowNoteNames = false;
            this.vkbd.Size = new System.Drawing.Size(1436, 142);
            this.vkbd.TabIndex = 0;
            this.vkbd.KeyboardEvent += new System.EventHandler<NBagOfTricks.UI.VirtualKeyboard.KeyboardEventArgs>(this.Vkbd_KeyboardEvent);
            // 
            // chkRunBars
            // 
            this.chkRunBars.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkRunBars.AutoSize = true;
            this.chkRunBars.Checked = true;
            this.chkRunBars.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRunBars.Location = new System.Drawing.Point(840, 174);
            this.chkRunBars.Name = "chkRunBars";
            this.chkRunBars.Size = new System.Drawing.Size(77, 27);
            this.chkRunBars.TabIndex = 16;
            this.chkRunBars.Text = "Run Bars";
            this.chkRunBars.UseVisualStyleBackColor = true;
            // 
            // barBar
            // 
            this.barBar.BeatsPerBar = 4;
            this.barBar.FontLarge = new System.Drawing.Font("Microsoft Sans Serif", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.barBar.FontSmall = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.barBar.Location = new System.Drawing.Point(943, 242);
            this.barBar.MarkerColor = System.Drawing.Color.Black;
            this.barBar.Name = "barBar";
            this.barBar.ProgressColor = System.Drawing.Color.White;
            this.barBar.Size = new System.Drawing.Size(353, 51);
            this.barBar.Snap = NBagOfTricks.UI.BarBar.SnapType.Tick;
            this.barBar.TabIndex = 15;
            this.barBar.TicksPerBeat = 8;
            // 
            // clickGrid1
            // 
            this.clickGrid1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.clickGrid1.Location = new System.Drawing.Point(546, 225);
            this.clickGrid1.Name = "clickGrid1";
            this.clickGrid1.Size = new System.Drawing.Size(371, 106);
            this.clickGrid1.TabIndex = 1;
            // 
            // waveViewer
            // 
            this.waveViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewer.DrawColor = System.Drawing.Color.Orange;
            this.waveViewer.Location = new System.Drawing.Point(546, 174);
            this.waveViewer.Name = "waveViewer";
            this.waveViewer.Size = new System.Drawing.Size(254, 45);
            this.waveViewer.TabIndex = 14;
            // 
            // timeBar
            // 
            this.timeBar.FontLarge = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeBar.FontSmall = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeBar.Location = new System.Drawing.Point(943, 174);
            this.timeBar.MarkerColor = System.Drawing.Color.Black;
            this.timeBar.Name = "timeBar";
            this.timeBar.ProgressColor = System.Drawing.Color.Orange;
            this.timeBar.Size = new System.Drawing.Size(353, 52);
            this.timeBar.SnapMsec = 0;
            this.timeBar.TabIndex = 13;
            // 
            // chkCpu
            // 
            this.chkCpu.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkCpu.AutoSize = true;
            this.chkCpu.Location = new System.Drawing.Point(633, 27);
            this.chkCpu.Name = "chkCpu";
            this.chkCpu.Size = new System.Drawing.Size(41, 27);
            this.chkCpu.TabIndex = 12;
            this.chkCpu.Text = "cpu";
            this.chkCpu.UseVisualStyleBackColor = true;
            this.chkCpu.CheckedChanged += new System.EventHandler(this.ChkCpu_CheckedChanged);
            // 
            // ftree
            // 
            this.ftree.DoubleClickSelect = false;
            this.ftree.Location = new System.Drawing.Point(12, 25);
            this.ftree.Name = "ftree";
            this.ftree.Size = new System.Drawing.Size(484, 529);
            this.ftree.TabIndex = 11;
            this.ftree.FileSelectedEvent += new System.EventHandler<string>(this.FilTree_FileSelectedEvent);
            // 
            // meter3
            // 
            this.meter3.BackColor = System.Drawing.Color.Gainsboro;
            this.meter3.DrawColor = System.Drawing.Color.Violet;
            this.meter3.Label = "meter dots";
            this.meter3.Location = new System.Drawing.Point(875, 25);
            this.meter3.Margin = new System.Windows.Forms.Padding(4);
            this.meter3.Maximum = 10D;
            this.meter3.MeterType = NBagOfTricks.UI.MeterType.ContinuousDots;
            this.meter3.Minimum = -10D;
            this.meter3.Name = "meter3";
            this.meter3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meter3.Size = new System.Drawing.Size(180, 48);
            this.meter3.TabIndex = 10;
            // 
            // meter2
            // 
            this.meter2.BackColor = System.Drawing.Color.Gainsboro;
            this.meter2.DrawColor = System.Drawing.Color.Azure;
            this.meter2.Label = "meter log";
            this.meter2.Location = new System.Drawing.Point(875, 108);
            this.meter2.Margin = new System.Windows.Forms.Padding(4);
            this.meter2.Maximum = 3D;
            this.meter2.MeterType = NBagOfTricks.UI.MeterType.Log;
            this.meter2.Minimum = -60D;
            this.meter2.Name = "meter2";
            this.meter2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meter2.Size = new System.Drawing.Size(180, 48);
            this.meter2.TabIndex = 9;
            // 
            // btnUT
            // 
            this.btnUT.BackColor = System.Drawing.Color.HotPink;
            this.btnUT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUT.Location = new System.Drawing.Point(546, 27);
            this.btnUT.Name = "btnUT";
            this.btnUT.Size = new System.Drawing.Size(58, 46);
            this.btnUT.TabIndex = 8;
            this.btnUT.Text = "UT";
            this.btnUT.UseVisualStyleBackColor = false;
            this.btnUT.Click += new System.EventHandler(this.UT_Click);
            // 
            // cpuMeter1
            // 
            this.cpuMeter1.BackColor = System.Drawing.Color.Gainsboro;
            this.cpuMeter1.DrawColor = System.Drawing.Color.DarkOrange;
            this.cpuMeter1.Enable = false;
            this.cpuMeter1.Label = "cpu";
            this.cpuMeter1.Location = new System.Drawing.Point(680, 25);
            this.cpuMeter1.Name = "cpuMeter1";
            this.cpuMeter1.Size = new System.Drawing.Size(175, 56);
            this.cpuMeter1.TabIndex = 7;
            // 
            // slider2
            // 
            this.slider2.BackColor = System.Drawing.Color.Gainsboro;
            this.slider2.DecPlaces = 1;
            this.slider2.DrawColor = System.Drawing.Color.SlateBlue;
            this.slider2.Label = "Vertical";
            this.slider2.Location = new System.Drawing.Point(1254, 25);
            this.slider2.Margin = new System.Windows.Forms.Padding(4);
            this.slider2.Maximum = 10D;
            this.slider2.Minimum = 0D;
            this.slider2.Name = "slider2";
            this.slider2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.slider2.ResetValue = 0D;
            this.slider2.Size = new System.Drawing.Size(42, 131);
            this.slider2.TabIndex = 6;
            this.slider2.Value = 5.4D;
            // 
            // pan1
            // 
            this.pan1.BackColor = System.Drawing.Color.Gainsboro;
            this.pan1.DrawColor = System.Drawing.Color.Crimson;
            this.pan1.Location = new System.Drawing.Point(692, 108);
            this.pan1.Margin = new System.Windows.Forms.Padding(4);
            this.pan1.Name = "pan1";
            this.pan1.Size = new System.Drawing.Size(146, 48);
            this.pan1.TabIndex = 5;
            this.pan1.Value = 0D;
            // 
            // txtInfo
            // 
            this.txtInfo.Location = new System.Drawing.Point(546, 338);
            this.txtInfo.Margin = new System.Windows.Forms.Padding(4);
            this.txtInfo.MaxText = 5000;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(754, 215);
            this.txtInfo.TabIndex = 4;
            this.txtInfo.WordWrap = true;
            // 
            // meter1
            // 
            this.meter1.BackColor = System.Drawing.Color.Gainsboro;
            this.meter1.DrawColor = System.Drawing.Color.Orange;
            this.meter1.Label = "meter xyz";
            this.meter1.Location = new System.Drawing.Point(1081, 108);
            this.meter1.Margin = new System.Windows.Forms.Padding(4);
            this.meter1.Maximum = 100D;
            this.meter1.MeterType = NBagOfTricks.UI.MeterType.Linear;
            this.meter1.Minimum = 0D;
            this.meter1.Name = "meter1";
            this.meter1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meter1.Size = new System.Drawing.Size(153, 48);
            this.meter1.TabIndex = 3;
            // 
            // pot1
            // 
            this.pot1.BackColor = System.Drawing.Color.Gainsboro;
            this.pot1.DecPlaces = 2;
            this.pot1.DrawColor = System.Drawing.Color.Green;
            this.pot1.ForeColor = System.Drawing.Color.Black;
            this.pot1.Label = "p99";
            this.pot1.Location = new System.Drawing.Point(546, 83);
            this.pot1.Margin = new System.Windows.Forms.Padding(5);
            this.pot1.Maximum = 1D;
            this.pot1.Minimum = 0D;
            this.pot1.Name = "pot1";
            this.pot1.Size = new System.Drawing.Size(81, 73);
            this.pot1.TabIndex = 1;
            this.pot1.Taper = NBagOfTricks.UI.Taper.Linear;
            this.pot1.Value = 0.5D;
            // 
            // slider1
            // 
            this.slider1.BackColor = System.Drawing.Color.Gainsboro;
            this.slider1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.slider1.DecPlaces = 2;
            this.slider1.DrawColor = System.Drawing.Color.Orange;
            this.slider1.Label = "Horizontal";
            this.slider1.Location = new System.Drawing.Point(1081, 25);
            this.slider1.Margin = new System.Windows.Forms.Padding(4);
            this.slider1.Maximum = 1D;
            this.slider1.Minimum = 0D;
            this.slider1.Name = "slider1";
            this.slider1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.slider1.ResetValue = 0D;
            this.slider1.Size = new System.Drawing.Size(153, 48);
            this.slider1.TabIndex = 2;
            this.slider1.Value = 0.3D;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // TestHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1436, 720);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TestHost";
            this.Text = "TestHost";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestHost_FormClosing);
            this.Load += new System.EventHandler(this.TestHost_Load);
            this.Shown += new System.EventHandler(this.TestHost_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private UI.VirtualKeyboard vkbd;
        private UI.Pot pot1;
        private UI.Slider slider1;
        private UI.Meter meter1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private UI.TextViewer txtInfo;
        private UI.Pan pan1;
        private UI.Slider slider2;
        private UI.CpuMeter cpuMeter1;
        private System.Windows.Forms.Button btnUT;
        private UI.Meter meter2;
        private UI.Meter meter3;
        private UI.FilTree ftree;
        private System.Windows.Forms.CheckBox chkCpu;
        private UI.TimeBar timeBar;
        private System.Windows.Forms.Timer timer1;
        private UI.WaveViewer waveViewer;
        private UI.ClickGrid clickGrid1;
        private UI.BarBar barBar;
        private System.Windows.Forms.CheckBox chkRunBars;
    }
}