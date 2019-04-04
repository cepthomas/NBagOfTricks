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
            this.virtualKeyboard1 = new NBagOfTricks.UI.VirtualKeyboard();
            this.pot1 = new NBagOfTricks.UI.Pot();
            this.slider1 = new NBagOfTricks.UI.Slider();
            this.meter1 = new NBagOfTricks.UI.Meter();
            this.SuspendLayout();
            // 
            // virtualKeyboard1
            // 
            this.virtualKeyboard1.Location = new System.Drawing.Point(28, 21);
            this.virtualKeyboard1.Name = "virtualKeyboard1";
            this.virtualKeyboard1.Size = new System.Drawing.Size(773, 173);
            this.virtualKeyboard1.TabIndex = 0;
            // 
            // pot1
            // 
            this.pot1.ControlColor = System.Drawing.Color.Black;
            this.pot1.DecPlaces = 1;
            this.pot1.Label = "???";
            this.pot1.Location = new System.Drawing.Point(28, 229);
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
            this.slider1.Location = new System.Drawing.Point(128, 229);
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
            this.meter1.Location = new System.Drawing.Point(263, 229);
            this.meter1.Maximum = 100D;
            this.meter1.MeterType = NBagOfTricks.UI.MeterType.Linear;
            this.meter1.Minimum = 0D;
            this.meter1.Name = "meter1";
            this.meter1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meter1.Size = new System.Drawing.Size(135, 59);
            this.meter1.TabIndex = 3;
            // 
            // TestHost
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 374);
            this.Controls.Add(this.meter1);
            this.Controls.Add(this.slider1);
            this.Controls.Add(this.pot1);
            this.Controls.Add(this.virtualKeyboard1);
            this.Name = "TestHost";
            this.Text = "TestHost";
            this.ResumeLayout(false);

        }

        #endregion

        private UI.VirtualKeyboard virtualKeyboard1;
        private UI.Pot pot1;
        private UI.Slider slider1;
        private UI.Meter meter1;
    }
}