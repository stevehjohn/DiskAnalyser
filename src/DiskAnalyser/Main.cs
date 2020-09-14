using System;
using System.Diagnostics;
using System.Windows.Forms;
// ReSharper disable LocalizableElement

namespace DiskAnalyser
{
    public partial class Main : Form
    {
        private readonly DiskAnalyser _diskAnalyser;

        public Main()
        {
            InitializeComponent();

            _diskAnalyser = new DiskAnalyser();

            InitialiseApp();
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
            _diskAnalyser.Analyse(mainTree.SelectedNode.Text);
        }
    }
}
