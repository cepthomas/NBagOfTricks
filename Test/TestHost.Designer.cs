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
            this.slider2 = new NBagOfTricks.UI.Slider();
            this.pan1 = new NBagOfTricks.UI.Pan();
            this.txtInfo = new NBagOfTricks.UI.TextViewer();
            this.meter1 = new NBagOfTricks.UI.Meter();
            this.pot1 = new NBagOfTricks.UI.Pot();
            this.slider1 = new NBagOfTricks.UI.Slider();
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
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.vkbd);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.slider2);
            this.splitContainer1.Panel2.Controls.Add(this.pan1);
            this.splitContainer1.Panel2.Controls.Add(this.txtInfo);
            this.splitContainer1.Panel2.Controls.Add(this.meter1);
            this.splitContainer1.Panel2.Controls.Add(this.pot1);
            this.splitContainer1.Panel2.Controls.Add(this.slider1);
            this.splitContainer1.Size = new System.Drawing.Size(864, 488);
            this.splitContainer1.SplitterDistance = 114;
            this.splitContainer1.TabIndex = 4;
            // 
            // vkbd
            // 
            this.vkbd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vkbd.KeyHeight = 100;
            this.vkbd.KeySize = 10;
            this.vkbd.Location = new System.Drawing.Point(0, 0);
            this.vkbd.Name = "vkbd";
            this.vkbd.Size = new System.Drawing.Size(864, 114);
            this.vkbd.TabIndex = 0;
            this.vkbd.KeyboardEvent += new System.EventHandler<NBagOfTricks.UI.VirtualKeyboard.KeyboardEventArgs>(this.vkbd_KeyboardEvent);
            // 
            // slider2
            // 
            this.slider2.BackColor = System.Drawing.Color.Gainsboro;
            this.slider2.ControlColor = System.Drawing.Color.SlateBlue;
            this.slider2.DecPlaces = 1;
            this.slider2.Label = "Vertical";
            this.slider2.Location = new System.Drawing.Point(390, 23);
            this.slider2.Maximum = 10D;
            this.slider2.Minimum = 0D;
            this.slider2.Name = "slider2";
            this.slider2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.slider2.ResetValue = 0D;
            this.slider2.Size = new System.Drawing.Size(53, 157);
            this.slider2.TabIndex = 6;
            this.slider2.Value = 5D;
            // 
            // pan1
            // 
            this.pan1.BackColor = System.Drawing.Color.Gainsboro;
            this.pan1.ControlColor = System.Drawing.Color.Crimson;
            this.pan1.Location = new System.Drawing.Point(29, 121);
            this.pan1.Name = "pan1";
            this.pan1.Size = new System.Drawing.Size(150, 56);
            this.pan1.TabIndex = 5;
            this.pan1.Value = 0D;
            // 
            // txtInfo
            // 
            this.txtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInfo.Location = new System.Drawing.Point(498, 9);
            this.txtInfo.MaxText = 5000;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(341, 352);
            this.txtInfo.TabIndex = 4;
            // 
            // meter1
            // 
            this.meter1.BackColor = System.Drawing.Color.Gainsboro;
            this.meter1.ControlColor = System.Drawing.Color.Orange;
            this.meter1.Label = "";
            this.meter1.Location = new System.Drawing.Point(211, 121);
            this.meter1.Maximum = 100D;
            this.meter1.MeterType = NBagOfTricks.UI.MeterType.Linear;
            this.meter1.Minimum = 0D;
            this.meter1.Name = "meter1";
            this.meter1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meter1.Size = new System.Drawing.Size(135, 59);
            this.meter1.TabIndex = 3;
            // 
            // pot1
            // 
            this.pot1.BackColor = System.Drawing.Color.Gainsboro;
            this.pot1.ControlColor = System.Drawing.Color.Green;
            this.pot1.DecPlaces = 2;
            this.pot1.ForeColor = System.Drawing.Color.Black;
            this.pot1.Label = "???";
            this.pot1.Location = new System.Drawing.Point(89, 23);
            this.pot1.Maximum = 1D;
            this.pot1.Minimum = 0D;
            this.pot1.Name = "pot1";
            this.pot1.Size = new System.Drawing.Size(61, 59);
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
            this.slider1.Location = new System.Drawing.Point(189, 23);
            this.slider1.Maximum = 1D;
            this.slider1.Minimum = 0D;
            this.slider1.Name = "slider1";
            this.slider1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.slider1.ResetValue = 0D;
            this.slider1.Size = new System.Drawing.Size(115, 59);
            this.slider1.TabIndex = 2;
            this.slider1.Value = 0.3D;
            // 
            // TestHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 488);
            this.Controls.Add(this.splitContainer1);
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
    }
}