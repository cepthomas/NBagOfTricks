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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.vkbd = new NBagOfTricks.UI.VirtualKeyboard();
            this.cpuMeter1 = new NBagOfTricks.UI.CpuMeter();
            this.slider2 = new NBagOfTricks.UI.Slider();
            this.pan1 = new NBagOfTricks.UI.Pan();
            this.txtInfo = new NBagOfTricks.UI.TextViewer();
            this.meter1 = new NBagOfTricks.UI.Meter();
            this.pot1 = new NBagOfTricks.UI.Pot();
            this.slider1 = new NBagOfTricks.UI.Slider();
            this.btnUT = new System.Windows.Forms.Button();
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
            this.splitContainer1.Panel2.Controls.Add(this.btnUT);
            this.splitContainer1.Panel2.Controls.Add(this.cpuMeter1);
            this.splitContainer1.Panel2.Controls.Add(this.slider2);
            this.splitContainer1.Panel2.Controls.Add(this.pan1);
            this.splitContainer1.Panel2.Controls.Add(this.txtInfo);
            this.splitContainer1.Panel2.Controls.Add(this.meter1);
            this.splitContainer1.Panel2.Controls.Add(this.pot1);
            this.splitContainer1.Panel2.Controls.Add(this.slider1);
            this.splitContainer1.Size = new System.Drawing.Size(1256, 601);
            this.splitContainer1.SplitterDistance = 119;
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
            this.vkbd.Size = new System.Drawing.Size(1256, 119);
            this.vkbd.TabIndex = 0;
            this.vkbd.KeyboardEvent += new System.EventHandler<NBagOfTricks.UI.VirtualKeyboard.KeyboardEventArgs>(this.vkbd_KeyboardEvent);
            // 
            // cpuMeter1
            // 
            this.cpuMeter1.BackColor = System.Drawing.Color.Gainsboro;
            this.cpuMeter1.ControlColor = System.Drawing.Color.Orange;
            this.cpuMeter1.Label = "cpu";
            this.cpuMeter1.Location = new System.Drawing.Point(74, 285);
            this.cpuMeter1.Name = "cpuMeter1";
            this.cpuMeter1.Size = new System.Drawing.Size(221, 90);
            this.cpuMeter1.TabIndex = 7;
            // 
            // slider2
            // 
            this.slider2.BackColor = System.Drawing.Color.Gainsboro;
            this.slider2.ControlColor = System.Drawing.Color.SlateBlue;
            this.slider2.DecPlaces = 1;
            this.slider2.Label = "Vertical";
            this.slider2.Location = new System.Drawing.Point(520, 28);
            this.slider2.Margin = new System.Windows.Forms.Padding(4);
            this.slider2.Maximum = 10D;
            this.slider2.Minimum = 0D;
            this.slider2.Name = "slider2";
            this.slider2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.slider2.ResetValue = 0D;
            this.slider2.Size = new System.Drawing.Size(71, 193);
            this.slider2.TabIndex = 6;
            this.slider2.Value = 5.4D;
            // 
            // pan1
            // 
            this.pan1.BackColor = System.Drawing.Color.Gainsboro;
            this.pan1.ControlColor = System.Drawing.Color.Crimson;
            this.pan1.Location = new System.Drawing.Point(39, 149);
            this.pan1.Margin = new System.Windows.Forms.Padding(4);
            this.pan1.Name = "pan1";
            this.pan1.Size = new System.Drawing.Size(200, 69);
            this.pan1.TabIndex = 5;
            this.pan1.Value = 0D;
            // 
            // txtInfo
            // 
            this.txtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInfo.Location = new System.Drawing.Point(664, 11);
            this.txtInfo.Margin = new System.Windows.Forms.Padding(4);
            this.txtInfo.MaxText = 5000;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(559, 453);
            this.txtInfo.TabIndex = 4;
            // 
            // meter1
            // 
            this.meter1.BackColor = System.Drawing.Color.Gainsboro;
            this.meter1.ControlColor = System.Drawing.Color.Orange;
            this.meter1.Label = "";
            this.meter1.Location = new System.Drawing.Point(281, 149);
            this.meter1.Margin = new System.Windows.Forms.Padding(4);
            this.meter1.Maximum = 100D;
            this.meter1.MeterType = NBagOfTricks.UI.MeterType.Linear;
            this.meter1.Minimum = 0D;
            this.meter1.Name = "meter1";
            this.meter1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meter1.Size = new System.Drawing.Size(180, 73);
            this.meter1.TabIndex = 3;
            // 
            // pot1
            // 
            this.pot1.BackColor = System.Drawing.Color.Gainsboro;
            this.pot1.ControlColor = System.Drawing.Color.Green;
            this.pot1.DecPlaces = 2;
            this.pot1.ForeColor = System.Drawing.Color.Black;
            this.pot1.Label = "???";
            this.pot1.Location = new System.Drawing.Point(119, 28);
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
            this.slider1.ControlColor = System.Drawing.Color.Orange;
            this.slider1.DecPlaces = 2;
            this.slider1.Label = "Horizontal";
            this.slider1.Location = new System.Drawing.Point(252, 28);
            this.slider1.Margin = new System.Windows.Forms.Padding(4);
            this.slider1.Maximum = 1D;
            this.slider1.Minimum = 0D;
            this.slider1.Name = "slider1";
            this.slider1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.slider1.ResetValue = 0D;
            this.slider1.Size = new System.Drawing.Size(153, 73);
            this.slider1.TabIndex = 2;
            this.slider1.Value = 0.3D;
            // 
            // btnUT
            // 
            this.btnUT.BackColor = System.Drawing.Color.HotPink;
            this.btnUT.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUT.Location = new System.Drawing.Point(22, 28);
            this.btnUT.Name = "btnUT";
            this.btnUT.Size = new System.Drawing.Size(75, 60);
            this.btnUT.TabIndex = 8;
            this.btnUT.Text = "UT";
            this.btnUT.UseVisualStyleBackColor = false;
            this.btnUT.Click += new System.EventHandler(this.btnUT_Click);
            // 
            // TestHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1256, 601);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TestHost";
            this.Text = "TestHost";
            this.Load += new System.EventHandler(this.TestHost_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
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
    }
}