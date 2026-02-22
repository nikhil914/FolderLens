# FolderLens

FolderLens is a high-performance Windows WPF application built with .NET 8 that helps you analyze, visualize, and manage your folder sizes and disk space usage. It provides deep insights into file distribution, identifies space-wasting files, and allows you to quickly locate large files and folders.

## Features

- **Fast Folder Scanning**: Utilizes an advanced `MftScanner` (Master File Table) for ultra-fast NTFS volume scanning, falling back to a `StandardScanner` when needed.
- **Space Saver Analysis**: Automatically identifies potential space-saving opportunities by grouping files by type, size, and other metrics. 
- **File Type Classification**: Categorizes files into intuitive types (e.g., Documents, Media, System, Archives) to provide a clear picture of what's consuming your storage.
- **Intuitive UI**: Built with WPF to offer a responsive, modern desktop experience.
- **Detailed Insights**: View breakdown of sizes by files and subfolders, helping you free up disk space effectively.
- **Context Menu Integration**: Optional integration into Windows Explorer context menu ("Analyze with FolderLens").

## Architecture

The project is structured into three main components:
- `FolderLens.App`: The user interface layer built using WPF.
- `FolderLens.Core`: The business logic layer containing the scanning engines (`MftScanner`, `StandardScanner`), data models (`FileEntry`, `FolderNode`), and analyzers (`FileTypeClassifier`, `SpaceSaverAnalyzer`).
- `FolderLens.Core.Tests`: Unit tests for the core logic.

## Requirements

- Windows 10 or later (64-bit)
- .NET 8.0 Desktop Runtime

## Building and Publishing

To build the application, you can use Visual Studio 2022 or the .NET CLI. 

A PowerShell script (`Publish.ps1`) is provided to publish the app as a self-contained, single-file executable:

```powershell
.\Publish.ps1
```

The resulting executable will be placed in `FolderLens.App\bin\Release\net8.0-windows\win-x64\publish\`.
An Inno Setup script (`setup.iss`) is also available to generate a formal Windows installer.
