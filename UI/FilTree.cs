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
using NBagOfTricks;


namespace NBagOfTricks.UI
{
    /// <summary>
    /// Tree control with tags and filters.
    /// </summary>
    public partial class FilTree : UserControl
    {
        #region Fields
        /// <summary>Key is file or dir path, value is associated tags.</summary>
        Dictionary<string, List<string>> _taggedPaths = new Dictionary<string, List<string>>();

        /// <summary>Filter by these tags.</summary>
        HashSet<string> _activeFilters = new HashSet<string>();
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

        /// <summary>All possible tags and whether they are active.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Dictionary<string, bool> AllTags { get; set; } = new Dictionary<string, bool>();

        /// <summary>Base path(s) for the tree.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public List<string> RootDirs { get; set; } = new List<string>();

        /// <summary>Show only these file types.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public List<string> FilterExts { get; set; } = new List<string>();

        /// <summary>Generate event with single or double click.</summary>
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
            PopulateFilters();

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
                PopulateFiles(e.Node);
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
        /// Populate the file selector.
        /// </summary>
        /// <param name="node">Selected directory.</param>
        void PopulateFiles(TreeNode node)
        {
            TreeNode clickedNode = node;

            lvFiles.Items.Clear();
            var nodeDirInfo = clickedNode.Tag as DirectoryInfo;

            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                if (FilterExts.Contains(Path.GetExtension(file.Name).ToLower()))
                {
                    bool show = false;
                    List<string> stags = _taggedPaths.Where(p => p.Key == file.FullName).FirstOrDefault().Value;

                    // Filters on?
                    if (_activeFilters.Count > 0)
                    {
                        if(stags != null)
                        {
                            var match = stags.Where(p => _activeFilters.Contains(p));
                            show = match.Count() > 0;
                        }
                    }
                    else // no filters, show all.
                    {
                        show = true;
                    }

                    if (show)
                    {
                        var item = new ListViewItem(new[] { file.Name, (file.Length / 1024).ToString(), stags != null ? string.Join(" ", stags ) : ""})
                        {
                            Tag = file.FullName
                        };
                        lvFiles.Items.Add(item);
                    }
                }
            }
        }

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

        /// <summary>
        /// Populate the context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuFiles_Opening(object sender, CancelEventArgs e)
        {
            menuFiles.Items.Clear();
            menuFiles.Items.Add("Edit Tags", null, MenuFiles_Click);
        }

        /// <summary>
        /// Context menu handler.
        /// Select the tags for this file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuFiles_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            string fn = lvFiles.SelectedItems[0].Tag.ToString();

            switch (sender.ToString())
            {
                case "Edit Tags":
                    // Make a collection of all tags associated with this file.
                    var pathTags = _taggedPaths.ContainsKey(fn) ? _taggedPaths[fn] : new List<string>();
                    var options = new Dictionary<string, bool>();
                    AllTags.ForEach(kv => { options[kv.Key] = pathTags.Contains(kv.Key); });

                    OptionsEditor oped = new OptionsEditor()
                    {
                        Title = "Edit Tags",
                        AllowEdit = false, // select only
                        Values = options
                    };

                    if (oped.ShowDialog() == DialogResult.OK)
                    {
                        // Process the user selections.
                        if(oped.Values.Count == 0) // None.
                        {
                            // None selected.
                            _taggedPaths.Remove(fn);
                            lvFiles.SelectedItems[0].SubItems[2].Text = "";
                        }
                        else // One or more selected.
                        {
                            // Update the tags for this file selection.
                            List<string> tags = new List<string>();
                            foreach(var v in oped.Values)
                            {
                                if(v.Value)
                                {
                                    tags.Add(v.Key);
                                }
                            }
                            _taggedPaths[fn] = tags;
                            string stags = string.Join(" ", tags);
                            lvFiles.SelectedItems[0].SubItems[2].Text = stags;
                        }
                    }
                    //else never mind
                    break;
            }
        }
        #endregion

        #region Filters
        /// <summary>
        /// Update the clickable label showing active filters.
        /// </summary>
        void PopulateFilters()
        {
            _activeFilters.Clear();

            foreach (var tag in AllTags)
            {
                if (tag.Value)
                {
                    _activeFilters.Add(tag.Key);
                }
            }

            lblActiveFilters.Text = "Filters: " + (_activeFilters.Count == 0 ? "None" : string.Join(" | ", _activeFilters));
        }

        /// <summary>
        /// Edit the tag filters for this file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ActiveFilters_Click(object sender, EventArgs e)
        {
            OptionsEditor oped = new OptionsEditor()
            {
                Title = "Edit Active Filters",
                AllowEdit = true,
                Values = AllTags
            };

            if (oped.ShowDialog() == DialogResult.OK)
            {
                // Go through all files and update their tags collections.
                List<string> removed = new List<string>();
                AllTags.ForEach(kv => { if (!oped.Values.ContainsKey(kv.Key)) removed.Add(kv.Key); });
                _taggedPaths.ForEach(kv => kv.Value.RemoveAll(s => removed.Contains(s)));
                
                AllTags = oped.Values;
                PopulateFilters();

                PopulateFiles(treeView.SelectedNode);
            }
            //else never mind
        }
        #endregion

        #region Misc privates
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
        /// Property accessor helper.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, string> GetTaggedPaths()
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            _taggedPaths.ForEach(kv => { paths[kv.Key] = string.Join(" ", kv.Value.ToArray()); });
            return paths;
        }

        /// <summary>
        /// Property accessor helper.
        /// </summary>
        /// <param name="paths"></param>
        void SetTaggedPaths(Dictionary<string, string> paths)
        {
            _taggedPaths.Clear();

            foreach (var kv in paths)
            {
                // Check for valid path.
                if (Directory.Exists(kv.Key) || File.Exists(kv.Key))
                {
                    // Check for path is off one of the roots - ask user what to do?
                    List<string> h = new List<string>();
                    _taggedPaths.Add(kv.Key, h);

                    // Check for valid tags.
                    foreach (string tag in kv.Value.SplitByToken(" "))
                    {
                        if(AllTags.ContainsKey(tag))
                        {
                            _taggedPaths[kv.Key].Add(tag);
                        }
                    }
                }
                else
                {
                    //TODO? throw new FileNotFoundException($"Invalid path: {kv.Key}");
                }
            }
        }
        #endregion
    }
}
