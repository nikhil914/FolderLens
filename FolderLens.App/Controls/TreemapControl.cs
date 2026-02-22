using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FolderLens.Core.Models;

namespace FolderLens.App.Controls
{
    public class TreemapControl : Canvas
    {
        public static readonly DependencyProperty RootNodeProperty =
            DependencyProperty.Register(nameof(RootNode), typeof(FolderNode), typeof(TreemapControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnRootNodeChanged));

        public FolderNode? RootNode
        {
            get => (FolderNode?)GetValue(RootNodeProperty);
            set => SetValue(RootNodeProperty, value);
        }

        private static void OnRootNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TreemapControl)d;
            control.InvalidateVisual();
            control.RenderTreemap();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            RenderTreemap();
        }

        private void RenderTreemap()
        {
            Children.Clear();
            if (RootNode == null || RootNode.Size == 0 || ActualWidth == 0 || ActualHeight == 0) return;

            // Using simple Slice and Dice for top-level and 1-level deep
            var items = RootNode.Subfolders.OrderByDescending(f => f.Size).ToList();
            if (items.Count == 0) return;

            DrawTreemap(items, new Rect(0, 0, ActualWidth, ActualHeight));
        }

        private void DrawTreemap(List<FolderNode> items, Rect bounds)
        {
            double totalSize = items.Sum(i => i.Size);
            if (totalSize == 0) return;

            var currentBounds = bounds;
            bool splitVertical = bounds.Width > bounds.Height;
            
            var colors = new[] { "#5C2E91", "#00529B", "#007A33", "#B35B00", "#A80000", "#3E3E42" };
            int colorIndex = 0;

            foreach (var item in items)
            {
                double ratio = item.Size / totalSize;
                Rect rect;

                if (splitVertical)
                {
                    double w = currentBounds.Width * ratio;
                    rect = new Rect(currentBounds.X, currentBounds.Y, w, currentBounds.Height);
                    currentBounds.X += w;
                    currentBounds.Width -= w;
                }
                else
                {
                    double h = currentBounds.Height * ratio;
                    rect = new Rect(currentBounds.X, currentBounds.Y, currentBounds.Width, h);
                    currentBounds.Y += h;
                    currentBounds.Height -= h;
                }

                splitVertical = currentBounds.Width > currentBounds.Height; // Re-evaluate

                if (rect.Width > 2 && rect.Height > 2)
                {
                    var r = new Rectangle
                    {
                        Width = rect.Width,
                        Height = rect.Height,
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[colorIndex % colors.Length])),
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        ToolTip = $"{item.Name}\n{FormatSize(item.Size)}"
                    };
                    
                    SetLeft(r, rect.X);
                    SetTop(r, rect.Y);
                    Children.Add(r);

                    if (rect.Width > 50 && rect.Height > 20)
                    {
                        var text = new TextBlock
                        {
                            Text = item.Name,
                            Foreground = Brushes.White,
                            IsHitTestVisible = false,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                            Width = rect.Width - 4
                        };
                        SetLeft(text, rect.X + 2);
                        SetTop(text, rect.Y + 2);
                        Children.Add(text);
                    }
                }
                colorIndex++;
            }
        }

        private string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
