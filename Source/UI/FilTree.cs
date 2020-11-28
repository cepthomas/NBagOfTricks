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

// TODO1 Expand when select node?

namespace NBagOfTricks.UI
{
    /// <summary>
    /// Tree control with tags and filters.
    /// </summary>
    public partial class FilTree : UserControl
    {
        #region Fields
        /// <summary>Key is file or dir path, value is associated tags.</summary>
        Dictionary<string, HashSet<string>> _taggedPaths = new Dictionary<string, HashSet<string>>();

        /// <summary>All possible tags.</summary>
        HashSet<string> _allTags = new HashSet<string>();

        /// <summary>Filter by these tags.</summary>
        HashSet<string> _activeFilters = new HashSet<string>();

        /// <summary>Manage cosmetics.</summary>
        TreeNode _lastSelectedNode = null;
        #endregion

        #region Properties
        /// <summary>Key is path to file or directory, value is space separated associated tags.</summary>
        public List<(string path, string tags)> TaggedPaths
        {
            get
            {
                List<(string path, string tags)> paths = new List<(string path, string tags)>();
                _taggedPaths.ForEach(kv => { paths.Add((kv.Key, string.Join(" ", kv.Value.ToArray()))); });
                return paths;
            }
            set
            {
                foreach ((string path, string tags) in value)
                {
                    // Check for valid path.
                    if (Directory.Exists(path) || File.Exists(path))
                    {
                        // TODO Check for path is off one of the roots - ask user what to do.

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
            }
        }

        /// <summary>All possible tags.</summary>
        public List<string> AllTags
        {
            get
            {
                return _allTags.ToList();
            }
            set
            {
                _allTags.Clear();
                value.ForEach(t => _allTags.Add(t));
            }
        }

        /// <summary>Base path(s) for the tree.</summary>
        public List<string> RootPaths { get; set; } = new List<string>();

        /// <summary>Show only these file types.</summary>
        public List<string> FilterExts { get; set; } = new List<string>();

        /// <summary>Generate event for single or double click.</summary>
        public bool DoubleClickSelect { get; set; } = false;
        #endregion

        #region Events
        /// <summary>User has selected a file.</summary>
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
                if (FilterExts.Contains(Path.GetExtension(file.Name).ToLower()))
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
        /// Manage menus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cms_Opening(object sender, CancelEventArgs e)
        {
            // TODO context menus:
            // Files context menu:
            // - list of all tags with checkboxes indicating tags for this file. show inherited from dir.
            // - add tag
            // - delete tag (need to remove from all files)
            // - edit tag? maybe
            // Tree context menu:
            // - Same as above for dirs.
            // - expand/compress all or 1/2/3/...

            cms.Items.Clear();
            var vvv = new ToolStripMenuItem("Select All", null, MenuItem_Click);
            cms.Items.Add(new ToolStripMenuItem("Select All", null, MenuItem_Click));
            cms.Items.Add(new ToolStripMenuItem("Clear All", null, MenuItem_Click));

            ToolStripMenuItem checkMarginOnly = new ToolStripMenuItem("Check Margin", null, MenuItem_Click)
            {
                Checked = true,
                CheckOnClick = true,
                CheckState = CheckState.Indeterminate
            };

            cms.Items.Add(checkMarginOnly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, EventArgs e)
        {
            var mi = sender as ToolStripMenuItem;

            switch (mi.Text)
            {
                case "Select All":
                    //PopulateList();
                    break;

                case "Clear All":
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
