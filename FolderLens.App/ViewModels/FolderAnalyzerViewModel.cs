using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FolderLens.Core.Models;
using FolderLens.Core.Scanners;
using Microsoft.Win32;

namespace FolderLens.App.ViewModels
{
    public partial class FolderAnalyzerViewModel : ObservableObject
    {
        private readonly IScanEngine _scanner;
        private CancellationTokenSource? _cts;

        private ScanResult? _currentScan;
        public ScanResult? CurrentScan
        {
            get => _currentScan;
            set => SetProperty(ref _currentScan, value);
        }

        private ObservableCollection<FolderNode> _rootNodes = new();
        public ObservableCollection<FolderNode> RootNodes
        {
            get => _rootNodes;
            set => SetProperty(ref _rootNodes, value);
        }

        private bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        private string _progressText = string.Empty;
        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        private string _selectedPath = string.Empty;
        public string SelectedPath
        {
            get => _selectedPath;
            set => SetProperty(ref _selectedPath, value);
        }

        public IAsyncRelayCommand SelectFolderAndScanCommand { get; }
        public IRelayCommand CancelScanCommand { get; }

        public FolderAnalyzerViewModel()
        {
            _scanner = new MftScanner();
            SelectFolderAndScanCommand = new AsyncRelayCommand(SelectFolderAndScanAsync);
            CancelScanCommand = new RelayCommand(CancelScan);
        }

        private async Task SelectFolderAndScanAsync()
        {
            var folderDialog = new OpenFolderDialog
            {
                Title = "Select Folder to Analyze"
            };

            if (folderDialog.ShowDialog() == true)
            {
                await ScanAsync(folderDialog.FolderName);
            }
        }

        public async Task ScanAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return;

            SelectedPath = path;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            IsScanning = true;
            ProgressText = "Scanning...";
            RootNodes.Clear();

            var progress = new Progress<string>(p => ProgressText = $"Scanning: {p}");

            try
            {
                CurrentScan = await Task.Run(() => _scanner.ScanAsync(path, progress, _cts.Token));
                RootNodes.Add(CurrentScan.RootNode);
                ProgressText = $"Scan complete. Found {CurrentScan.TotalFilesCount:N0} files in {CurrentScan.ScanDuration.TotalSeconds:F2} seconds.";
            }
            catch (OperationCanceledException)
            {
                ProgressText = "Scan cancelled.";
            }
            catch (Exception ex)
            {
                ProgressText = $"Error: {ex.Message}";
            }
            finally
            {
                IsScanning = false;
            }
        }

        private void CancelScan()
        {
            _cts?.Cancel();
        }
    }
}
