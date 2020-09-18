using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
// ReSharper disable LocalizableElement

namespace DiskAnalyser
{
    public partial class Main : Form
    {
        private readonly DiskAnalyser _diskAnalyser;

        private readonly BackgroundWorker _backgroundWorker;

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

            _backgroundWorker.RunWorkerAsync(mainTree.SelectedNode.Text);
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _diskAnalyser.Analyse((string) e.Argument);
        }
    }
}
