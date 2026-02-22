using System;
using System.Collections.Generic;

namespace FolderLens.Core.Models
{
    public class FolderNode
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public long Size { get; set; }
        public double PercentageOfParent { get; set; }
        
        public int FileCount { get; set; }
        public int SubfolderCount { get; set; }
        public DateTime LastModified { get; set; }
        
        public FolderNode? Parent { get; set; }
        public List<FolderNode> Subfolders { get; set; } = new();
        public List<FileEntry> Files { get; set; } = new();

        public void AddSubfolder(FolderNode folder)
        {
            folder.Parent = this;
            Subfolders.Add(folder);
        }

        public void AddFile(FileEntry file)
        {
            file.Parent = this;
            Files.Add(file);
        }
    }
}
