using System;
using System.Collections.Generic;
using System.Linq;
using FolderLens.Core.Models;

namespace FolderLens.Core.Analyzers
{
    public class FileCategoryResult
    {
        public string Category { get; set; } = string.Empty;
        public long TotalSize { get; set; }
        public int FileCount { get; set; }
        public double PercentageOfTotal { get; set; }
        public long AvgFileSize => FileCount > 0 ? TotalSize / FileCount : 0;
        public List<FileEntry> Files { get; set; } = new();
    }

    public static class FileTypeClassifier
    {
        private static readonly Dictionary<string, string> ExtensionMap = new(StringComparer.OrdinalIgnoreCase)
        {
            // Images
            { ".jpg", "Images" }, { ".jpeg", "Images" }, { ".png", "Images" }, { ".gif", "Images" },
            { ".webp", "Images" }, { ".raw", "Images" }, { ".psd", "Images" }, { ".ai", "Images" },
            { ".svg", "Images" }, { ".bmp", "Images" }, { ".tiff", "Images" },
            // Videos
            { ".mp4", "Videos" }, { ".mkv", "Videos" }, { ".avi", "Videos" }, { ".mov", "Videos" },
            { ".wmv", "Videos" }, { ".flv", "Videos" }, { ".webm", "Videos" },
            // Audio
            { ".mp3", "Audio" }, { ".flac", "Audio" }, { ".wav", "Audio" }, { ".aac", "Audio" },
            { ".ogg", "Audio" }, { ".m4a", "Audio" },
            // Documents
            { ".pdf", "Documents" }, { ".docx", "Documents" }, { ".doc", "Documents" },
            { ".xlsx", "Documents" }, { ".xls", "Documents" }, { ".pptx", "Documents" },
            { ".ppt", "Documents" }, { ".txt", "Documents" }, { ".rtf", "Documents" },
            { ".csv", "Documents" }, { ".md", "Documents" },
            // Archives
            { ".zip", "Archives" }, { ".rar", "Archives" }, { ".7z", "Archives" },
            { ".tar", "Archives" }, { ".gz", "Archives" }, { ".bz2", "Archives" },
            // Executables
            { ".exe", "Executables & Installers" }, { ".msi", "Executables & Installers" },
            { ".apk", "Executables & Installers" }, { ".dmg", "Executables & Installers" },
            { ".bat", "Executables & Installers" }, { ".cmd", "Executables & Installers" },
            // Developer
            { ".kt", "Developer Files" }, { ".java", "Developer Files" }, { ".py", "Developer Files" },
            { ".js", "Developer Files" }, { ".ts", "Developer Files" }, { ".cs", "Developer Files" },
            { ".cpp", "Developer Files" }, { ".c", "Developer Files" }, { ".h", "Developer Files" },
            { ".json", "Developer Files" }, { ".xml", "Developer Files" }, { ".yaml", "Developer Files" },
            { ".yml", "Developer Files" }, { ".gradle", "Developer Files" }, { ".html", "Developer Files" },
            { ".css", "Developer Files" }, { ".sln", "Developer Files" }, { ".csproj", "Developer Files" },
            // System
            { ".dll", "System Files" }, { ".sys", "System Files" }, { ".ini", "System Files" },
            // Databases
            { ".db", "Databases" }, { ".sqlite", "Databases" }, { ".mdf", "Databases" }, { ".bak", "Databases" },
            // Temp & Cache
            { ".tmp", "Temp & Cache" }, { ".cache", "Temp & Cache" }, { ".log", "Temp & Cache" }
        };

        public static List<FileCategoryResult> Classify(ScanResult scanResult)
        {
            if (scanResult?.RootNode == null) return new List<FileCategoryResult>();

            var results = new Dictionary<string, FileCategoryResult>();
            var allFiles = new List<FileEntry>();
            CollectAllFiles(scanResult.RootNode, allFiles);

            foreach (var file in allFiles)
            {
                string category = "Other / Unknown";
                if (!string.IsNullOrEmpty(file.Extension) && ExtensionMap.TryGetValue(file.Extension, out var mappedCategory))
                {
                    category = mappedCategory;
                }
                else if (file.Name.StartsWith("~") || file.Extension == ".bak")
                {
                    category = "Temp & Cache";
                }

                if (!results.TryGetValue(category, out var result))
                {
                    result = new FileCategoryResult { Category = category };
                    results[category] = result;
                }

                result.Files.Add(file);
                result.TotalSize += file.Size;
                result.FileCount++;
            }

            long totalSize = allFiles.Sum(f => f.Size);
            
            var list = results.Values.OrderByDescending(r => r.TotalSize).ToList();
            foreach (var r in list)
            {
                if (totalSize > 0)
                {
                    r.PercentageOfTotal = (double)r.TotalSize / totalSize * 100;
                }
                r.Files = r.Files.OrderByDescending(f => f.Size).ToList();
            }

            return list;
        }

        private static void CollectAllFiles(FolderNode node, List<FileEntry> files)
        {
            files.AddRange(node.Files);
            foreach (var sub in node.Subfolders)
            {
                CollectAllFiles(sub, files);
            }
        }
    }
}
