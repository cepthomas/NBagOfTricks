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
using NBagOfTricks.Utils;

namespace NBagOfTricks.UI
{
    /// <summary>
    /// Tree control with tags and filters.
    /// </summary>
    public partial class FilTree : UserControl
    {
        #region Fields
        ///// <summary>
        ///// Key is file path, value is space separated associated tags.
        ///// </summary>
        //Dictionary<string, string> _taggedFiles = new Dictionary<string, string>();

        ///// <summary>
        ///// Key is dir path, value is space separated associated tags.
        ///// </summary>
        //Dictionary<string, string> _taggedDirs = new Dictionary<string, string>();

        /// <summary>
        /// Key is file or dir path, value is associated tags.
        /// </summary>
        Dictionary<string, HashSet<string>> _taggedPaths = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// All possible tags.
        /// </summary>
        HashSet<string> _allTags = new HashSet<string>();

        /// <summary>
        /// Filter by these tags.
        /// </summary>
        HashSet<string> _activeFilters = new HashSet<string>();

        /// <summary>
        /// Manage cosmetics.
        /// </summary>
        TreeNode _lastSelectedNode = null;
        #endregion

        #region Properties - client sets these before calling Init().
        /// <summary>
        /// Key is path to file or directory, value is space separated associated tags.
        /// </summary>
        public List<(string path, string tags)> TaggedPaths { get; set; } = new List<(string, string)>();

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
        /// Populate everything from the properties.
        /// </summary>
        public void Init()
        {
            // Process properties into our internal structure.
            _allTags.Clear();
            AllTags.ForEach(t => _allTags.Add(t));

            foreach ((string path, string tags) in TaggedPaths)
            {
                // Check for valid path.
                if (Directory.Exists(path) || File.Exists(path))
                {
                    // TODOC Check for path is off one of the roots - ask user what to do.


                    // Check for valid tags. If not, add to all tags.
                    HashSet<string> h = new HashSet<string>();
                    tags.SplitByToken(" ").ForEach(t => { _allTags.Add(t); h.Add(t); });
                    _taggedPaths.Add(path, h);
                }
                else
                {
                    throw new FileNotFoundException($"Invalid path: {path}");
                }
            }

            //_taggedDirs.Clear();
            //_taggedFiles.Clear();
            //foreach ((string path, string tags) in TaggedPaths)
            //{
            //    if (Directory.Exists(path))
            //    {
            //        _taggedDirs.Add(path, tags);
            //    }
            //    else if (File.Exists(path))
            //    {
            //        _taggedFiles.Add(path, tags);
            //    }
            //    else
            //    {
            //        throw new FileNotFoundException($"Invalid path: {path}");
            //    }
            //}

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

        /// <summary>
        /// Collect changes. TODOC kinda klunky.
        /// </summary>
        public void FlushChanges()
        {
            AllTags = _allTags.ToList();

            TaggedPaths.Clear();
            _taggedPaths.ForEach(kv => { TaggedPaths.Add((kv.Key, string.Join(" ", kv.Value.ToArray()))); });

            //Dictionary<string, string> _taggedFiles = new Dictionary<string, string>();
            //Dictionary<string, string> _taggedDirs = new Dictionary<string, string>();
            //HashSet<string> _allTags = new HashSet<string>();
        }
        #endregion

        #region Tree View
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
        #endregion

        #region File List
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cms_Opening(object sender, CancelEventArgs e)
        {



            cms.Items.Add(new ToolStripMenuItem("Select All"));
            cms.Items.Add(new ToolStripMenuItem("Clear All"));

            // TODOC context menus:

            // Files context menu:
            // - list of all tags with checkboxes indicating tags for this file. show inherited from dir.
            // - add tag
            // - delete tag (need to remove from all files)
            // - edit tag? maybe


            // Tree context menu:
            // - Same as above for dirs.
            // - expand/compress all or 1/2/3/...



            //cms.Items.Clear();

            // cms.ShowImageMargin = false;
            // cms.Items.Add(new ToolStripMenuItem("Select All"));
            // cms.Items.Add(new ToolStripMenuItem("Clear All"));
            // cms.ItemClicked += new ToolStripItemClickedEventHandler(cms_ItemClicked);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #region Filtering

        private void FilterByTags_DropDownOpening(object sender, EventArgs e)
        {
            btnFilterByTags.DropDownItems.Clear();

            btnFilterByTags.DropDownItems.Add(new ToolStripMenuItem("Select All", null, FilterByTags_ItemClicked));
            btnFilterByTags.DropDownItems.Add(new ToolStripMenuItem("Clear All", null, FilterByTags_ItemClicked));
            // cms.Items.Add(new ToolStripMenuItem("Clear All"));
            // cms.ItemClicked += new ToolStripItemClickedEventHandler(cms_ItemClicked);



            // cms.ShowImageMargin = false;
            // cms.Items.Add(new ToolStripMenuItem("Select All"));
            // cms.Items.Add(new ToolStripMenuItem("Clear All"));
            // cms.ItemClicked += new ToolStripItemClickedEventHandler(cms_ItemClicked);

            // ToolStripMenuItem toolStripMenuItem1 = new ToolStripMenuItem("11111");
            // toolStripMenuItem1.Checked = true;
            // toolStripMenuItem1.CheckState = CheckState.Indeterminate;
            // cms.Items.Add(toolStripMenuItem1);


        }

        private void FilterByTags_ItemClicked(object sender, EventArgs e)
        {

        }

        private void FilterByTags_DropDownClosed(object sender, EventArgs e)
        {
            // Need an active filters list

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
        #endregion

    }
}
