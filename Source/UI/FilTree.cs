using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace NBagOfTricks.UI
{
    /// <summary>
    /// Tree control with tags and filters.
    /// </summary>
    public partial class FilTree : UserControl
    {
        #region Fields
        /// <summary>
        /// Key is file path, value is space separated associated tags.
        /// </summary>
        Dictionary<string, string> _taggedFiles = new Dictionary<string, string>();

        /// <summary>
        /// Key is dir path, value is space separated associated tags.
        /// </summary>
        Dictionary<string, string> _taggedDirs = new Dictionary<string, string>();

        /// <summary>
        /// All possible tags.
        /// </summary>
        HashSet<string> _allTags = new HashSet<string>();

        /// <summary>
        /// Manage cosmetics.
        /// </summary>
        TreeNode _lastSelectedNode = null;
        #endregion

        #region Properties - client sets these before calling Init().
        /// <summary>
        /// Key is path to file or directory, value is space separated associated tags.
        /// </summary>
        public List<(string path, string tags)> TaggedPaths { get; set; } = new List<(string, string)>(); //TODOC get packs up everything so client can save.

        /// <summary>
        /// All possible tags.
        /// </summary>
        public List<string> AllTags { get; set; } = new List<string>();

        /// <summary>
        /// Base path(s) for the tree.
        /// </summary>
        public List<string> RootPaths { get; set; } = new List<string>();

        /// <summary>
        /// Show only these file types.
        /// </summary>
        public List<string> FilterExts { get; set; } = new List<string>();

        /// <summary>
        /// Generate event for single or double click.
        /// </summary>
        public bool DoubleClickSelect { get; set; } = false;
        #endregion

        #region Events
        /// <summary>
        /// User has selected a file.
        /// </summary>
        public event EventHandler<string> FileSelectedEvent = null;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FilTree()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FilTree_Load(object sender, EventArgs e)
        {
            treeView.HideSelection = false;
            treeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            // Process properties into our internal structure.
            _allTags.Clear();
            AllTags.ForEach(t => _allTags.Add(t));

            _taggedDirs.Clear();
            _taggedFiles.Clear();
            foreach ((string path, string tags) in TaggedPaths)
            {
                // TODOC Check for valid tags. Remove or add to all tags?
                if (Directory.Exists(path))
                {
                    _taggedDirs.Add(path, tags);
                }
                else if (File.Exists(path))
                {
                    _taggedFiles.Add(path, tags);
                }
                else
                {
                    throw new FileNotFoundException($"Invalid path: {path}");
                }
            }

            // Show what we have.
            PopulateTreeView();
            if(treeView.Nodes.Count > 0)
            {
                treeView.SelectedNode = treeView.Nodes[0];
                PopulateNode(treeView.Nodes[0]);
            }
            else
            {
                throw new DirectoryNotFoundException($"No root directories");
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        void PopulateTreeView()
        {
            treeView.Nodes.Clear();

            foreach (string path in RootPaths)
            {
                TreeNode rootNode;

                DirectoryInfo info = new DirectoryInfo(path);
                if (info.Exists)
                {
                    rootNode = new TreeNode(info.Name)
                    {
                        Tag = info
                    };

                    ShowDirectories(info.GetDirectories(), rootNode);
                    treeView.Nodes.Add(rootNode);
                }
                else
                {
                    throw new DirectoryNotFoundException($"Invalid root directory: {path}");
                }
            }

            // Open them up a bit.
            foreach (TreeNode n in treeView.Nodes)
            {
                n.Expand();
            }
        }

        /// <summary>
        /// Recursively drill down through the directory structure and populate the tree.
        /// </summary>
        /// <param name="dirs"></param>
        /// <param name="parentNode"></param>
        void ShowDirectories(DirectoryInfo[] dirs, TreeNode parentNode)
        {
            foreach (DirectoryInfo dir in dirs)
            {
                TreeNode subDirNode = new TreeNode(dir.Name, 0, 0)
                {
                    Tag = dir,
                    ImageKey = "folder"
                };

                // Go a little lower now.
                DirectoryInfo[] subDirs = dir.GetDirectories();
                ShowDirectories(subDirs, subDirNode);
                parentNode.Nodes.Add(subDirNode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilTree_Resize(object sender, EventArgs e)
        {
            lvFiles.Columns[0].Width = lvFiles.Width / 2;
            lvFiles.Columns[1].Width = -2;
        }

        #region Tree Selection
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PopulateNode(e.Node);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void PopulateNode(TreeNode node)
        {
            TreeNode clickedNode = node;

            lvFiles.Items.Clear();
            var nodeDirInfo = clickedNode.Tag as DirectoryInfo;

            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                if (FilterExts.Contains(Path.GetExtension(file.Name)))
                {
                    var item = new ListViewItem(new[] { file.Name, "TODO tags" });
                    item.Tag = file.FullName;
                    lvFiles.Items.Add(item);
                }
            }
        }
        #endregion

        #region File List Selection
        /// <summary>
        /// Single click file selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListFiles_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !DoubleClickSelect && FileSelectedEvent != null)
            {
                FileSelectedEvent.Invoke(this, lvFiles.SelectedItems[0].Tag.ToString());
            }
        }

        /// <summary>
        /// Double click file selection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ListFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && DoubleClickSelect && FileSelectedEvent != null)
            {
                FileSelectedEvent.Invoke(this, lvFiles.SelectedItems[0].Tag.ToString());
            }
        }
        #endregion

        #region Context Menus
        private void Cms_Opening(object sender, CancelEventArgs e)
        {
            cms.Items.Add(new ToolStripMenuItem("Select All"));
            cms.Items.Add(new ToolStripMenuItem("Clear All"));

            // TODOC context menus:

            // Tree context menu:
            // - Same as above for dirs.
            // - expand/compress all or 1/2/3/...

            // Files context menu:
            // - list of all tags with checkboxes indicating tags for this file. show inherited from dir.
            // - add tag
            // - delete tag (need to remove from all files)
            // - edit tag? maybe



                //       cms.Items.Clear();

                // cms.ShowImageMargin = false;
                // cms.Items.Add(new ToolStripMenuItem("Select All"));
                // cms.Items.Add(new ToolStripMenuItem("Clear All"));
                // cms.ItemClicked += new ToolStripItemClickedEventHandler(cms_ItemClicked);

                // lvFiles.ContextMenuStrip = cms;
                // lvFiles.ContextMenuStrip.Opening += new CancelEventHandler(cms_MenuOpening);


                // //string text, Image image, EventHandler onClick

                // ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem("11111");
                // toolStripMenuItem1.Checked = true;
                // toolStripMenuItem1.CheckState = CheckState.Indeterminate;
                // cms.Items.Add(toolStripMenuItem1);

                // ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem("222222");
                // cms.Items.Add(toolStripMenuItem2);

                // ToolStripSeparator toolStripSeparator1 = new ToolStripSeparator();
                // cms.Items.Add(toolStripSeparator1);

                // ToolStripComboBox toolStripComboBox1 = new ToolStripComboBox();
                // toolStripComboBox1.Items.Add("c1");
                // toolStripComboBox1.Items.Add("c2");
                // toolStripComboBox1.Items.Add("c3");
                // cms.Items.Add(toolStripComboBox1);

                // ToolStripTextBox toolStripTextBox1 = new ToolStripTextBox();
                // toolStripTextBox1.Font = new Font("Segoe UI", 9F);
                // toolStripTextBox1.Text = "Hello!";
                // cms.Items.Add(toolStripTextBox1);

        }

        void Cms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // Set/clear the assay selections based on menu selection.
            switch (e.ClickedItem.ToString())
            {
                case "Select All":
                    //SelectedAssays.Clear();
                    //SelectedAssays.AddRange(AllAssays);
                    //PopulateList();
                    break;

                case "Clear All":
                    //SelectedAssays.Clear();
                    //PopulateList();
                    break;
            }
        }
        #endregion

        #region Misc
        /// <summary>
        /// Ensure tree selection is always visible. Kludgy...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Select new node
            e.Node.BackColor = SystemColors.Highlight;
            e.Node.ForeColor = SystemColors.HighlightText;
            if (_lastSelectedNode != null)
            {
                // Deselect old node
                _lastSelectedNode.BackColor = SystemColors.Window;
                _lastSelectedNode.ForeColor = SystemColors.WindowText;
            }
            _lastSelectedNode = e.Node;
        }

        /// <summary>
        /// See above.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true;
        }
        #endregion
    }
}
