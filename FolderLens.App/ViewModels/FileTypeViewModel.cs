using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using FolderLens.Core.Analyzers;
using FolderLens.Core.Models;
using OxyPlot;
using OxyPlot.Series;

namespace FolderLens.App.ViewModels
{
    public partial class FileTypeViewModel : ObservableObject
    {
        private ObservableCollection<FileCategoryResult> _categories = new();
        public ObservableCollection<FileCategoryResult> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private PlotModel _pieChartModel = new();
        public PlotModel PieChartModel
        {
            get => _pieChartModel;
            set => SetProperty(ref _pieChartModel, value);
        }

        public void LoadData(ScanResult? scan)
        {
            if (scan == null) return;
            var classified = FileTypeClassifier.Classify(scan);
            Categories = new ObservableCollection<FileCategoryResult>(classified);

            var model = new PlotModel { TextColor = OxyColors.White, Background = OxyColors.Transparent };
            var pieSeries = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0
            };

            foreach (var c in classified)
            {
                pieSeries.Slices.Add(new PieSlice(c.Category, c.TotalSize) { IsExploded = false });
            }

            model.Series.Add(pieSeries);
            PieChartModel = model;
        }
    }
}
