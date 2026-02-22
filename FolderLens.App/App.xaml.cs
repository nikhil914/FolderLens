using System.Configuration;
using System.Data;
using System.Windows;

namespace FolderLens.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static string? StartupFolderPath { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (e.Args.Length > 0)
        {
            StartupFolderPath = e.Args[0];
        }

        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}

