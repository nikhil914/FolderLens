using System;

namespace FolderLens.Core.Models
{
    public class FileEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime LastAccessed { get; set; }
        
        public FolderNode? Parent { get; set; }
    }
}
