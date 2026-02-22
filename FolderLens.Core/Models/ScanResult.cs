using System;
using System.Collections.Generic;

namespace FolderLens.Core.Models
{
    public class ScanResult
    {
        public string RootPath { get; set; } = string.Empty;
        public FolderNode RootNode { get; set; } = new();
        public TimeSpan ScanDuration { get; set; }
        public DateTime ScanTime { get; set; }
        public long TotalSizeBytes => RootNode.Size;
        public long TotalFilesCount { get; set; }
        public long TotalFoldersCount { get; set; }

        public List<Exception> ScanErrors { get; set; } = new();
    }
}
