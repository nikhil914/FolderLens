using System;
using System.Threading;
using System.Threading.Tasks;
using FolderLens.Core.Models;

namespace FolderLens.Core.Scanners
{
    public interface IScanEngine
    {
        Task<ScanResult> ScanAsync(string rootPath, IProgress<string>? progress = null, CancellationToken cancellationToken = default);
    }
}
