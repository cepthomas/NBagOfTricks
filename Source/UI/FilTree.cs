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
                //Console.WriteLine($"IsExpanded:{e.Node.IsExpanded} IsSelected:{e.Node.IsSelected}");

                if(e.Node.FirstNode == null)
                {
                    // No subnodes therefore plain file.
                    PopulateFiles(e.Node);
                }
                else
                {
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
                    bool show = true;

                    // Is it in our tagged files?
                    if(TaggedPaths.ContainsKey(file.FullName))
                    {
                        var match = TaggedPaths.Where(p => _activeFilters.Contains(p.Value));
                        show = match.Count() > 0;
                    }

                    if(show)
                    {
                        string stags = TaggedPaths.ContainsKey(file.FullName) ? string.Join(" ", TaggedPaths[file.FullName]) : "";
                        var item = new ListViewItem(new[] { file.Name, (file.Length / 1024).ToString(), stags })
                        {
                            Tag = file.FullName
                        };
                        lvFiles.Items.Add(item);
                    }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuFiles_Opening(object sender, CancelEventArgs e)
        {
            menuFiles.Items.Clear();
            menuFiles.Items.Add("Edit Tags", null, Recent_Click);

            void Recent_Click(object csender, EventArgs ce)
            {
                ToolStripMenuItem item = csender as ToolStripMenuItem;
                string fn = lvFiles.SelectedItems[0].Tag.ToString();

                switch (csender.ToString())
                {
                    case "Edit Tags":
                        var tags = _taggedPaths.ContainsKey(fn) ? _taggedPaths[fn] : new HashSet<string>();
                        {
                            xxx
                        }


                        break;
                }
                //string fn = sender.ToString();
                //OpenFile(fn);
            }
        }


        #endregion

        #region Filters
        /// <summary>
        /// Add filter buttons for each tag type.
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

            lblActiveFilters.Text = string.Join("  ", _activeFilters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ActiveFilters_Click(object sender, EventArgs e)
        {
            OptionsEditor oped = new OptionsEditor()
            {
                Title = "Edit All Filters",
                Values = AllTags
            };

            if (oped.ShowDialog() == DialogResult.OK)
            {
                AllTags = oped.Values;
                PopulateFilters();
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
            foreach (var kv in paths)
            {
                // Check for valid path.
                if (Directory.Exists(kv.Key) || File.Exists(kv.Key))
                {
                    // TODOC Check for path is off one of the roots - ask user what to do.
                    HashSet<string> h = new HashSet<string>();
                    _taggedPaths.Add(kv.Key, h);

                    // Check for valid tags.
                    foreach (string tag in kv.Value.SplitByToken(" "))
                    {
                        if(!AllTags.ContainsKey(tag))
                        {
                            AllTags[tag] = false;
                        }
                    }
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
