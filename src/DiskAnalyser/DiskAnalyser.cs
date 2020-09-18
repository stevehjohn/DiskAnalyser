using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiskAnalyser
{
    public class DiskAnalyser
    {
        public FolderInfo Root { get; private set; }

        public string[] GetDrives()
        {
            return DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).Select(d => d.Name).ToArray();
        }

        public void Analyse(string drive)
        {
            Root = new FolderInfo { Name = $"\\\\?\\{drive}" };

            AnalyseNode(Root);
        }

        private static void AnalyseNode(FolderInfo node)
        {
            var directoryInfo = new DirectoryInfo(node.Name);

            try
            {
                node.Size = directoryInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Sum(f => f.Length);
            }
            catch
            {
                //
            }

            IEnumerable<DirectoryInfo> folders;

            try
            {
                folders = directoryInfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            foreach (var folder in folders)
            {
                if ((folder.Attributes & FileAttributes.ReparsePoint) == 0)
                {
                    var child = new FolderInfo { Name = folder.FullName };

                    node.Children.Add(child);

                    AnalyseNode(child);
                }
            }
        }
    }
}