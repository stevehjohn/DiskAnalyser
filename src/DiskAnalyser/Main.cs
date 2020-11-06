using System;
using System.ComponentModel;
using System.Windows.Forms;

// ReSharper disable LocalizableElement

namespace DiskAnalyser
{
    public partial class Main : Form
    {
        private readonly DiskAnalyser _diskAnalyser;

        private readonly BackgroundWorker _backgroundWorker;

        private TreeNode _analysisNode;

        public Main()
        {
            InitializeComponent();

            _diskAnalyser = new DiskAnalyser();

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;

            InitialiseApp();
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            statusLabel.Text = string.Empty;

            progressBar.Visible = false;

            _analysisNode.Tag = _diskAnalyser.Root;

            foreach (var child in _diskAnalyser.Root.Children)
            {
                var newNode = _analysisNode.Nodes.Add(GetDirectoryName(child.Name));

                newNode.Tag = child;
                newNode.ImageKey = "folder.ico";
                newNode.SelectedImageKey = "folder.ico";

                if (child.Children.Count > 0)
                {
                    var dummyNode = newNode.Nodes.Add("Working...");

                    dummyNode.ImageKey = "Spinner.ico";
                    dummyNode.SelectedImageKey = "Spinner.ico";
                }

                mainFlow.Controls.Add(new ProgressBar
                                      {
                                          Minimum = 0,
                                          Maximum = 1000000,
                                          Text = "arse", // child.TotalSize.ToString(),
                                          Value = (int) (1000000.0d / _diskAnalyser.DriveSize * child.TotalSize),
                                          Height = 16,
                                          Padding = new Padding(0, 0, 0, 0),
                                          Margin = new Padding(1, 1, 1, 1),
                                          Width = 300
                                      });
            }
        }

        private string GetDirectoryName(string fullName)
        {
            return fullName.Substring(fullName.LastIndexOf('\\') + 1);
        }

        private void InitialiseApp()
        {
            ListDrivesInTreeView();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ListDrivesInTreeView()
        {
            mainTree.Nodes.Clear();

            var drives = _diskAnalyser.GetDrives();

            foreach (var drive in drives)
            {
                var node = mainTree.Nodes.Add(drive);

                node.ImageKey = "hdd.ico";
            }
        }

        private void AnalyseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Analysing...";

            progressBar.Visible = true;

            _analysisNode = mainTree.SelectedNode;

            _backgroundWorker.RunWorkerAsync(mainTree.SelectedNode.Text);
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _diskAnalyser.Analyse((string) e.Argument);
        }

        private void mainTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (! (e.Node.Tag is FolderInfo node))
            {
                return;
            }

            e.Node.Nodes.Clear();

            foreach (var folderInfo in node.Children)
            {
                var child = e.Node.Nodes.Add(GetDirectoryName(folderInfo.Name));

                child.Tag = folderInfo;
                child.ImageKey = "folder.ico";
                child.SelectedImageKey = "folder.ico";

                if (folderInfo.Children.Count > 0)
                {
                    var dummyNode = child.Nodes.Add("Working...");

                    dummyNode.ImageKey = "Spinner.ico";
                    dummyNode.SelectedImageKey = "Spinner.ico";
                }
            }
        }

        private void mainTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            e.Node.Nodes.Clear();

            var dummyNode = e.Node.Nodes.Add("Working...");

            dummyNode.ImageKey = "Spinner.ico";
            dummyNode.SelectedImageKey = "Spinner.ico";
        }
    }
}