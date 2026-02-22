using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FolderLens.Core.Models;

namespace FolderLens.Core.Scanners
{
    public class StandardScanner : IScanEngine
    {
        public async Task<ScanResult> ScanAsync(string rootPath, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
        {
            var result = new ScanResult
            {
                RootPath = rootPath,
                ScanTime = DateTime.Now
            };

            var startTime = DateTime.UtcNow;

            if (!Directory.Exists(rootPath))
            {
                result.ScanErrors.Add(new DirectoryNotFoundException($"Directory not found: {rootPath}"));
                return result;
            }

            result.RootNode = new FolderNode
            {
                Name = new DirectoryInfo(rootPath).Name,
                Path = rootPath
            };

            // Recursively scan in a background thread
            await Task.Run(() => ScanDirectory(result.RootNode, result, progress, cancellationToken), cancellationToken);

            CalculatePercentages(result.RootNode);

            result.ScanDuration = DateTime.UtcNow - startTime;
            return result;
        }

        private void ScanDirectory(FolderNode currentNode, ScanResult scanResult, IProgress<string>? progress, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            try
            {
                var dirInfo = new DirectoryInfo(currentNode.Path);
                currentNode.LastModified = dirInfo.LastWriteTime;
                
                // Files first
                var files = dirInfo.EnumerateFiles();
                foreach (var file in files)
                {
                    ct.ThrowIfCancellationRequested();
                    var fileEntry = new FileEntry
                    {
                        Name = file.Name,
                        Path = file.FullName,
                        Extension = file.Extension.ToLowerInvariant(),
                        Size = file.Length,
                        LastModified = file.LastWriteTime,
                        LastAccessed = file.LastAccessTime
                    };
                    
                    currentNode.AddFile(fileEntry);
                    currentNode.Size += fileEntry.Size;
                    currentNode.FileCount++;
                    
                    scanResult.TotalFilesCount++;
                }

                // Subdirectories
                var subDirs = dirInfo.EnumerateDirectories();
                foreach (var subDir in subDirs)
                {
                    ct.ThrowIfCancellationRequested();
                    progress?.Report(subDir.FullName);

                    var subNode = new FolderNode
                    {
                        Name = subDir.Name,
                        Path = subDir.FullName
                    };
                    
                    currentNode.AddSubfolder(subNode);
                    scanResult.TotalFoldersCount++;

                    // Recursive call
                    ScanDirectory(subNode, scanResult, progress, ct);

                    // Aggregate sizes up the tree
                    currentNode.Size += subNode.Size;
                    currentNode.FileCount += subNode.FileCount;
                    currentNode.SubfolderCount += (1 + subNode.SubfolderCount);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                scanResult.ScanErrors.Add(ex);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                scanResult.ScanErrors.Add(ex);
            }
        }

        private void CalculatePercentages(FolderNode node)
        {
            if (node.Parent != null && node.Parent.Size > 0)
            {
                node.PercentageOfParent = ((double)node.Size / node.Parent.Size) * 100;
            }
            else if (node.Parent == null)
            {
                node.PercentageOfParent = 100;
            }

            foreach (var child in node.Subfolders)
            {
                CalculatePercentages(child);
            }
        }
    }
}
