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
            this.vkbd = new NBagOfTricks.UI.VirtualKeyboard();
            this.pot1 = new NBagOfTricks.UI.Pot();
            this.slider1 = new NBagOfTricks.UI.Slider();
            this.meter1 = new NBagOfTricks.UI.Meter();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtInfo = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // vkbd
            // 
            this.vkbd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vkbd.Location = new System.Drawing.Point(0, 0);
            this.vkbd.Name = "vkbd";
            this.vkbd.Size = new System.Drawing.Size(864, 118);
            this.vkbd.TabIndex = 0;
            this.vkbd.KeyboardEvent += new System.EventHandler<NBagOfTricks.UI.VirtualKeyboard.KeyboardEventArgs>(this.vkbd_KeyboardEvent);
            // 
            // pot1
            // 
            this.pot1.ControlColor = System.Drawing.Color.Black;
            this.pot1.DecPlaces = 1;
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
            this.slider1.ControlColor = System.Drawing.Color.Orange;
            this.slider1.DecPlaces = 1;
            this.slider1.Label = "";
            this.slider1.Location = new System.Drawing.Point(189, 23);
            this.slider1.Maximum = 1D;
            this.slider1.Minimum = 0D;
            this.slider1.Name = "slider1";
            this.slider1.ResetValue = 0D;
            this.slider1.Size = new System.Drawing.Size(115, 59);
            this.slider1.TabIndex = 2;
            this.slider1.Value = 0D;
            // 
            // meter1
            // 
            this.meter1.ControlColor = System.Drawing.Color.Orange;
            this.meter1.Label = "";
            this.meter1.Location = new System.Drawing.Point(324, 23);
            this.meter1.Maximum = 100D;
            this.meter1.MeterType = NBagOfTricks.UI.MeterType.Linear;
            this.meter1.Minimum = 0D;
            this.meter1.Name = "meter1";
            this.meter1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meter1.Size = new System.Drawing.Size(135, 59);
            this.meter1.TabIndex = 3;
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
            this.splitContainer1.Panel2.Controls.Add(this.txtInfo);
            this.splitContainer1.Panel2.Controls.Add(this.meter1);
            this.splitContainer1.Panel2.Controls.Add(this.pot1);
            this.splitContainer1.Panel2.Controls.Add(this.slider1);
            this.splitContainer1.Size = new System.Drawing.Size(864, 236);
            this.splitContainer1.SplitterDistance = 118;
            this.splitContainer1.TabIndex = 4;
            // 
            // txtInfo
            // 
            this.txtInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInfo.Location = new System.Drawing.Point(498, 9);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Size = new System.Drawing.Size(341, 96);
            this.txtInfo.TabIndex = 4;
            this.txtInfo.Text = "";
            // 
            // TestHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 236);
            this.Controls.Add(this.splitContainer1);
            this.Name = "TestHost";
            this.Text = "TestHost";
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
        private System.Windows.Forms.RichTextBox txtInfo;
    }
}