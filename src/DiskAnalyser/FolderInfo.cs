using System.Collections.Generic;

namespace DiskAnalyser
{
    public class FolderInfo
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public long TotalSize { get; set; }

        public List<FolderInfo> Children { get; }

        public FolderInfo Parent { get; set; }

        public FolderInfo()
        {
            Children = new List<FolderInfo>();
        }
    }
}