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
    /// 
    /// </summary>
    public partial class TreeViewEx : UserControl
    {
        #region Fields
        /// <summary>
        /// Base path(s) for the tree.
        /// </summary>
        List<string> _rootPaths = new List<string>();

        /// <summary>
        /// Show only these file types.
        /// </summary>
        List<string> _filterExts = new List<string>();
        #endregion

        #region Properties
        /// <summary>
        /// Key is file path, value is space separated associated tags.
        /// </summary>
        public Dictionary<string, string> TaggedFiles { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Key is dir path, value is space separated associated tags.
        /// </summary>
        public Dictionary<string, string> TaggedDirs { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// All possible tags - client supplies and persists.
        /// </summary>
        public HashSet<string> AllTags = new HashSet<string>();

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
        public TreeViewEx()
        {
            InitializeComponent();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TreeViewEx_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPaths">Where the files be.</param>
        /// <param name="filterExts">Oly these kinds.</param>
        public void Init(List<string> rootPaths, List<string> filterExts)
        {
            _rootPaths = rootPaths;
            _filterExts = filterExts;
            PopulateTreeView();
            if(treeView.Nodes.Count > 0)
            {
                treeView.SelectedNode = treeView.Nodes[0];
            }
            else
            {

            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        void PopulateTreeView()
        {
            treeView.Nodes.Clear();

            foreach (string path in _rootPaths)
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
        private void TreeViewEx_Resize(object sender, EventArgs e)
        {
            lvFiles.Columns[0].Width = lvFiles.Width / 2;
            lvFiles.Columns[1].Width = -2;
        }

        #region Tree Selection

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode clickedNode = e.Node;

                lvFiles.Items.Clear();
                var nodeDirInfo = clickedNode.Tag as DirectoryInfo;

                foreach (FileInfo file in nodeDirInfo.GetFiles())
                {
                    if (_filterExts.Contains(Path.GetExtension(file.Name)))
                    {
                        var item = new ListViewItem(new[] { file.Name, "TODO tags" });
                        item.Tag = file.FullName;
                        lvFiles.Items.Add(item);
                    }
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


        #region Context Menu

        private void cms_Opening(object sender, CancelEventArgs e)
        {
            cms.Items.Add(new ToolStripMenuItem("Select All"));
            cms.Items.Add(new ToolStripMenuItem("Clear All"));

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

        void cms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
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
    }
}
