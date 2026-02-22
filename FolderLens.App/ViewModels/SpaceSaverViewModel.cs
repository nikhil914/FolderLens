using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FolderLens.Core.Analyzers;
using FolderLens.Core.Models;

namespace FolderLens.App.ViewModels
{
    public partial class SpaceSaverViewModel : ObservableObject
    {
        private ObservableCollection<DuplicateGroup> _duplicates = new();
        public ObservableCollection<DuplicateGroup> Duplicates
        {
            get => _duplicates;
            set => SetProperty(ref _duplicates, value);
        }

        private ObservableCollection<FileEntry> _largeFiles = new();
        public ObservableCollection<FileEntry> LargeFiles
        {
            get => _largeFiles;
            set => SetProperty(ref _largeFiles, value);
        }

        private ObservableCollection<FolderNode> _developerJunk = new();
        public ObservableCollection<FolderNode> DeveloperJunk
        {
            get => _developerJunk;
            set => SetProperty(ref _developerJunk, value);
        }

        private bool _isAnalyzing;
        public bool IsAnalyzing
        {
            get => _isAnalyzing;
            set => SetProperty(ref _isAnalyzing, value);
        }

        public async Task LoadDataAsync(ScanResult? scan)
        {
            if (scan == null) return;
            
            IsAnalyzing = true;
            
            var result = await SpaceSaverAnalyzer.AnalyzeAsync(scan);
            
            Duplicates = new ObservableCollection<DuplicateGroup>(result.Duplicates);
            LargeFiles = new ObservableCollection<FileEntry>(result.LargeFiles);
            DeveloperJunk = new ObservableCollection<FolderNode>(result.DeveloperJunkFolders);
            
            IsAnalyzing = false;
        }
    }
}
