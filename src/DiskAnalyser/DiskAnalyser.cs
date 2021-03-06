﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiskAnalyser
{
    public class DiskAnalyser
    {
        public FolderInfo Root { get; private set; }

        public long DriveSize { get; private set; }

        public string[] GetDrives()
        {
            return DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).Select(d => d.Name).ToArray();
        }

        public void Analyse(string drive)
        {
            Root = new FolderInfo { Name = $"\\\\?\\{drive}" };

            DriveSize = DriveInfo.GetDrives().First(di => di.Name == drive).TotalSize;

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
                folders = directoryInfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).ToList();
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            //if (! folders.Any())
            //{
            //    var parent = node.Parent;

            //    var child = node;

            //    while (parent != null)
            //    {
            //        parent.TotalSize += child.TotalSize;

            //        parent = parent.Parent;

            //        child = parent;
            //    }
            //}

            foreach (var folder in folders)
            {
                if ((folder.Attributes & FileAttributes.ReparsePoint) == 0)
                {
                    var child = new FolderInfo
                                {
                                    Name = folder.FullName,
                                    Parent = node
                                };

                    node.Children.Add(child);

                    AnalyseNode(child);
                }
            }
        }
    }
}