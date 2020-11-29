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
        /// <summary>Key is file or dir path, value is associated tags.</summary>
        Dictionary<string, HashSet<string>> _taggedPaths = new Dictionary<string, HashSet<string>>();

        /// <summary>All possible tags.</summary>
        HashSet<string> _allTags = new HashSet<string>();

        /// <summary>Filter by these tags.</summary>
        HashSet<string> _activeFilters = new HashSet<string>();

        ///// <summary>Manage cosmetics.</summary>
        //TreeNode _lastSelectedNode = null;
        #endregion

        #region Properties
        /// <summary>Key is path to file or directory, value is space separated associated tags.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Dictionary<string, string> TaggedPaths
        {
            get { return GetTaggedPaths(); }
            set { SetTaggedPaths(value); }
        }

        /// <summary>All possible tags.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public List<string> AllTags
        {
            get { return _allTags.ToList(); }
            set { _allTags.Clear(); value.ForEach(t => _allTags.Add(t)); }
        }

        /// <summary>Base path(s) for the tree.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public List<string> RootDirs { get; set; } = new List<string>();

        /// <summary>Show only these file types.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
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
                PopulateFiles(treeView.Nodes[0]);
            }
            else
            {
                throw new DirectoryNotFoundException($"No root directories");
            }
        }
        #endregion

        #region Tree View
        /// <summary>
        /// Drill down through dirs/files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //Console.WriteLine($"IsExpanded:{e.Node.IsExpanded} IsSelected:{e.Node.IsSelected}");

                if(e.Node.FirstNode == null)
                {
                    // No subnodes therefore plain file.
                    PopulateFiles(e.Node);
                }
                else
                {
                    // More to go.
                    // Click +:
                    // 1-IsExpanded:True IsSelected:False
                    // 2-IsExpanded:False IsSelected:False
                    // Click name:
                    // 1-IsExpanded:False IsSelected:False
                    // 2-IsExpanded:False IsSelected:True

                    //if (e.Node.IsExpanded == false)
                    //{
                    //    e.Node.Expand();
                    //}
                    //else
                    //{
                    //    e.Node.Collapse();
                    //}
                }
            }
        }

        /// <summary>
        /// Populate the file selector.
        /// </summary>
        /// <param name="node"></param>
        void PopulateFiles(TreeNode node)
        {
            TreeNode clickedNode = node;

            lvFiles.Items.Clear();
            var nodeDirInfo = clickedNode.Tag as DirectoryInfo;

            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                if (FilterExts.Contains(Path.GetExtension(file.Name).ToLower()))
                {
                    var item = new ListViewItem(new[] { file.Name, (file.Length / 1024).ToString(), ">>> tags" })
                    {
                        Tag = file.FullName
                    };
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

            foreach (string path in RootDirs)
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
        void ListFiles_MouseClick(object sender, MouseEventArgs e)
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
        void Cms_Opening(object sender, CancelEventArgs e)
        {
            // TODOC context menus:
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
        void MenuItem_Click(object sender, EventArgs e)
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

        #region Misc privates
        ///// <summary>
        ///// Ensure tree selection is always visible. Kludgy...
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        //{
        //    // Select new node
        //    e.Node.BackColor = SystemColors.Highlight;
        //    e.Node.ForeColor = SystemColors.HighlightText;
        //    if (_lastSelectedNode != null)
        //    {
        //        // Deselect old node
        //        _lastSelectedNode.BackColor = SystemColors.Window;
        //        _lastSelectedNode.ForeColor = SystemColors.WindowText;
        //    }
        //    _lastSelectedNode = e.Node;
        //}

        /// <summary>
        /// See above.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TreeView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FilTree_Resize(object sender, EventArgs e)
        {
            lvFiles.Columns[0].Width = lvFiles.Width / 2;
            lvFiles.Columns[1].Width = -2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetTaggedPaths()
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            _taggedPaths.ForEach(kv => { paths[kv.Key] = string.Join(" ", kv.Value.ToArray()); });
            return paths;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths"></param>
        void SetTaggedPaths(Dictionary<string, string> paths)
        {
            foreach (var kv in paths)
            {
                // Check for valid path.
                if (Directory.Exists(kv.Key) || File.Exists(kv.Key))
                {
                    // TODOC Check for path is off one of the roots - ask user what to do.
                    // TODOC Check for valid tags. If not, add to all tags? or remove?
                    HashSet<string> h = new HashSet<string>();
                    kv.Value.SplitByToken(" ").ForEach(t => { _allTags.Add(t); h.Add(t); });
                    _taggedPaths.Add(kv.Key, h);
                }
                else
                {
                    throw new FileNotFoundException($"Invalid path: {kv.Key}");
                }
            }
        }
        #endregion
    }
}
