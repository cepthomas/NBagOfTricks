namespace NBagOfTricks.UI
{
    partial class FilTree
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeView = new System.Windows.Forms.TreeView();
            this.menuFiles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lvFiles = new System.Windows.Forms.ListView();
            this.dvcolFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dvcolSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dvcolTags = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblActiveFilters = new System.Windows.Forms.ToolStripLabel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView.ContextMenuStrip = this.menuFiles;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(257, 587);
            this.treeView.TabIndex = 0;
            this.treeView.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.TreeView_DrawNode);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView_NodeMouseClick);
            // 
            // menuFiles
            // 
            this.menuFiles.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuFiles.Name = "menuTree";
            this.menuFiles.Size = new System.Drawing.Size(61, 4);
            this.menuFiles.Opening += new System.ComponentModel.CancelEventHandler(this.MenuFiles_Opening);
            // 
            // lvFiles
            // 
            this.lvFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.dvcolFile,
            this.dvcolSize,
            this.dvcolTags});
            this.lvFiles.ContextMenuStrip = this.menuFiles;
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.HideSelection = false;
            this.lvFiles.Location = new System.Drawing.Point(0, 0);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(327, 587);
            this.lvFiles.TabIndex = 1;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            this.lvFiles.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ListFiles_MouseClick);
            this.lvFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ListFiles_MouseDoubleClick);
            // 
            // dvcolFile
            // 
            this.dvcolFile.Text = "File";
            this.dvcolFile.Width = 377;
            // 
            // dvcolSize
            // 
            this.dvcolSize.Text = "Size (kb)";
            this.dvcolSize.Width = 70;
            // 
            // dvcolTags
            // 
            this.dvcolTags.Text = "Tags";
            this.dvcolTags.Width = 365;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblActiveFilters});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(588, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lblActiveFilters
            // 
            this.lblActiveFilters.Name = "lblActiveFilters";
            this.lblActiveFilters.Size = new System.Drawing.Size(93, 22);
            this.lblActiveFilters.Text = "Active Filters";
            this.lblActiveFilters.Click += new System.EventHandler(this.ActiveFilters_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 25);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lvFiles);
            this.splitContainer2.Size = new System.Drawing.Size(588, 587);
            this.splitContainer2.SplitterDistance = 257;
            this.splitContainer2.TabIndex = 4;
            // 
            // FilTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FilTree";
            this.Size = new System.Drawing.Size(588, 612);
            this.Load += new System.EventHandler(this.FilTree_Load);
            this.Resize += new System.EventHandler(this.FilTree_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader dvcolFile;
        private System.Windows.Forms.ColumnHeader dvcolTags;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ColumnHeader dvcolSize;
        private System.Windows.Forms.ContextMenuStrip menuFiles;
        private System.Windows.Forms.ToolStripLabel lblActiveFilters;
    }
}
