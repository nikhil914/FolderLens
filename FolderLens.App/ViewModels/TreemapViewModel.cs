using CommunityToolkit.Mvvm.ComponentModel;
using FolderLens.Core.Models;

namespace FolderLens.App.ViewModels
{
    public partial class TreemapViewModel : ObservableObject
    {
        private FolderNode? _currentRootNode;
        public FolderNode? CurrentRootNode
        {
            get => _currentRootNode;
            set => SetProperty(ref _currentRootNode, value);
        }
        
        public void LoadData(ScanResult? scan)
        {
            CurrentRootNode = scan?.RootNode;
        }
    }
}
