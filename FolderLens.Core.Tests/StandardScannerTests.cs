using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FolderLens.Core.Scanners;
using Xunit;

namespace FolderLens.Core.Tests
{
    public class StandardScannerTests : IDisposable
    {
        private readonly string _testDirectory;

        public StandardScannerTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "FolderLensTest_" + Guid.NewGuid());
            Directory.CreateDirectory(_testDirectory);
            
            // Create some test files and folders
            var subFolder1 = Directory.CreateDirectory(Path.Combine(_testDirectory, "SubFolder1"));
            var subFolder2 = Directory.CreateDirectory(Path.Combine(_testDirectory, "SubFolder2"));
            
            File.WriteAllBytes(Path.Combine(_testDirectory, "root1.txt"), new byte[100]);
            File.WriteAllBytes(Path.Combine(subFolder1.FullName, "child1.bin"), new byte[500]);
            File.WriteAllBytes(Path.Combine(subFolder2.FullName, "child2.log"), new byte[200]);
        }

        [Fact]
        public async Task ScanAsync_Returns_Correct_Sizes_And_Counts()
        {
            var scanner = new StandardScanner();
            var result = await scanner.ScanAsync(_testDirectory, null, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(800, result.TotalSizeBytes);
            Assert.Equal(3, result.TotalFilesCount);
            Assert.Equal(2, result.TotalFoldersCount); // SubFolder1, SubFolder2

            // Verify Root Node
            Assert.Equal(800, result.RootNode.Size);
            Assert.Equal(3, result.RootNode.FileCount); // total aggregated files
            Assert.Equal(1, result.RootNode.Files.Count); // direct files
            Assert.Equal(2, result.RootNode.SubfolderCount); // directly under root
            
            // Verify Percentages
            Assert.Equal(100.0, result.RootNode.PercentageOfParent);
            var sub1 = result.RootNode.Subfolders.First(f => f.Name == "SubFolder1");
            Assert.Equal(500, sub1.Size);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
    }
}
