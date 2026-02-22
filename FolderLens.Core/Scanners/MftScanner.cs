using System;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using FolderLens.Core.Models;

namespace FolderLens.Core.Scanners
{
    public class MftScanner : IScanEngine
    {
        private readonly StandardScanner _fallbackScanner;

        public MftScanner()
        {
            _fallbackScanner = new StandardScanner();
        }

        public async Task<ScanResult> ScanAsync(string rootPath, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
        {
            var driveInfo = new DriveInfo(Path.GetPathRoot(rootPath) ?? rootPath);
            
            // Check if NTFS and Admin privileges are available for MFT read
            if (driveInfo.DriveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase) && IsAdministrator())
            {
                try
                {
                    // TODO: Implement direct NTFS MFT parsing via DeviceIoControl and FSCTL_ENUM_USN_DATA
                    // For now, fall back to standard scanner to ensure application stability 
                    // until native MFT parsing algorithm is fully validated.
                    return await _fallbackScanner.ScanAsync(rootPath, progress, cancellationToken);
                }
                catch (Exception)
                {
                    // Fallback on error
                    return await _fallbackScanner.ScanAsync(rootPath, progress, cancellationToken);
                }
            }
            else
            {
                // Fallback to standard enumeration for non-NTFS or non-admin
                return await _fallbackScanner.ScanAsync(rootPath, progress, cancellationToken);
            }
        }

        private bool IsAdministrator()
        {
            try
            {
                using var identity = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }
    }
}
