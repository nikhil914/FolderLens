using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FolderLens.Core.Models;

namespace FolderLens.Core.Analyzers
{
    public class DuplicateGroup
    {
        public string Hash { get; set; } = string.Empty;
        public long Size { get; set; }
        public List<FileEntry> Files { get; set; } = new();
        public long WastedSpace => Size * Math.Max(0, Files.Count - 1);
    }

    public class SpaceSaverResult
    {
        public List<DuplicateGroup> Duplicates { get; set; } = new();
        public List<FileEntry> LargeFiles { get; set; } = new();
        public List<FileEntry> OldFiles { get; set; } = new();
        public List<FileEntry> TempJunkFiles { get; set; } = new();
        public List<FolderNode> DeveloperJunkFolders { get; set; } = new();
        public List<FolderNode> EmptyFolders { get; set; } = new();
    }

    public static class SpaceSaverAnalyzer
    {
        private static readonly HashSet<string> DevJunkNames = new(StringComparer.OrdinalIgnoreCase) 
        { "node_modules", ".gradle", "build", ".dart_tool", "__pycache__", ".cache", "bin", "obj", ".vs", "Pods" };

        public static async Task<SpaceSaverResult> AnalyzeAsync(ScanResult scan, int oldDaysThreshold = 90)
        {
            var result = new SpaceSaverResult();
            if (scan?.RootNode == null) return result;

            var allFiles = new List<FileEntry>();
            var allFolders = new List<FolderNode>();
            CollectNodes(scan.RootNode, allFiles, allFolders);

            // Large Files (Top 50)
            result.LargeFiles = allFiles.OrderByDescending(f => f.Size).Take(50).ToList();

            // Old Files
            var oldDate = DateTime.Now.AddDays(-oldDaysThreshold);
            result.OldFiles = allFiles.Where(f => f.LastAccessed < oldDate && f.Size > 10 * 1024 * 1024).OrderByDescending(f => f.Size).ToList();

            // Temp Junk
            result.TempJunkFiles = allFiles.Where(f => 
                f.Extension == ".tmp" || f.Extension == ".cache" || f.Extension == ".bak" || f.Name.StartsWith("~")
            ).OrderByDescending(f => f.Size).ToList();

            // Developer Junk Config
            result.DeveloperJunkFolders = allFolders.Where(f => DevJunkNames.Contains(f.Name)).OrderByDescending(f => f.Size).ToList();

            // Empty Folders
            result.EmptyFolders = allFolders.Where(f => f.FileCount == 0 && f.SubfolderCount == 0).ToList();

            // Duplicates (Parallel MD5)
            // Group by size first for fast pre-filtering
            var sizeGroups = allFiles.GroupBy(f => f.Size).Where(g => g.Count() > 1 && g.Key > 0).ToList();
            var duplicateGroups = new ConcurrentBag<DuplicateGroup>();

            await Task.Run(() =>
            {
                Parallel.ForEach(sizeGroups, sizeGroup =>
                {
                    var hashGroups = new Dictionary<string, List<FileEntry>>();
                    using var md5 = MD5.Create();
                    var buffer = new byte[1024 * 1024]; // 1MB buffer

                    foreach (var file in sizeGroup)
                    {
                        try
                        {
                            using var stream = File.OpenRead(file.Path);
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            var hashBytes = md5.ComputeHash(buffer, 0, bytesRead);
                            var hashStr = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                            if (!hashGroups.TryGetValue(hashStr, out var list))
                            {
                                list = new List<FileEntry>();
                                hashGroups[hashStr] = list;
                            }
                            list.Add(file);
                        }
                        catch { /* locked file */ }
                    }

                    foreach (var hg in hashGroups.Where(kvp => kvp.Value.Count > 1))
                    {
                        duplicateGroups.Add(new DuplicateGroup
                        {
                            Hash = hg.Key,
                            Size = sizeGroup.Key,
                            Files = hg.Value.OrderByDescending(f => f.LastAccessed).ToList()
                        });
                    }
                });
            });

            result.Duplicates = duplicateGroups.OrderByDescending(g => g.WastedSpace).ToList();

            return result;
        }

        private static void CollectNodes(FolderNode node, List<FileEntry> files, List<FolderNode> folders)
        {
            files.AddRange(node.Files);
            folders.Add(node);
            foreach (var sub in node.Subfolders)
            {
                CollectNodes(sub, files, folders);
            }
        }
    }
}
