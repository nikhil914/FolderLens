using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FolderLens.App.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private readonly FolderAnalyzerViewModel _folderAnalyzerViewModel;
        private readonly TreemapViewModel _treemapViewModel;
        private readonly FileTypeViewModel _fileTypeViewModel;
        private readonly SpaceSaverViewModel _spaceSaverViewModel;

        public IRelayCommand ShowFolderAnalyzerCommand { get; }
        public IRelayCommand ShowTreemapCommand { get; }
        public IRelayCommand ShowFileTypesCommand { get; }
        public IRelayCommand ShowSpaceSaverCommand { get; }

        public MainWindowViewModel()
        {
            _folderAnalyzerViewModel = new FolderAnalyzerViewModel();
            _treemapViewModel = new TreemapViewModel();
            _fileTypeViewModel = new FileTypeViewModel();
            _spaceSaverViewModel = new SpaceSaverViewModel();
            CurrentView = _folderAnalyzerViewModel;

            ShowFolderAnalyzerCommand = new RelayCommand(ShowFolderAnalyzer);
            ShowTreemapCommand = new RelayCommand(ShowTreemap);
            ShowFileTypesCommand = new RelayCommand(ShowFileTypes);
            ShowSpaceSaverCommand = new RelayCommand(ShowSpaceSaver);

            // Handle argv[0] routing
            if (!string.IsNullOrEmpty(App.StartupFolderPath))
            {
                _ = _folderAnalyzerViewModel.ScanAsync(App.StartupFolderPath);
            }
        }

        private void ShowFolderAnalyzer()
        {
            CurrentView = _folderAnalyzerViewModel;
        }

        private void ShowTreemap()
        {
            _treemapViewModel.LoadData(_folderAnalyzerViewModel.CurrentScan);
            CurrentView = _treemapViewModel;
        }

        private void ShowFileTypes()
        {
            _fileTypeViewModel.LoadData(_folderAnalyzerViewModel.CurrentScan);
            CurrentView = _fileTypeViewModel;
        }

        private void ShowSpaceSaver()
        {
            _ = _spaceSaverViewModel.LoadDataAsync(_folderAnalyzerViewModel.CurrentScan);
            CurrentView = _spaceSaverViewModel;
        }
    }
}
