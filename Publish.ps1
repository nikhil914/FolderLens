# Publish.ps1
# This script publishes the WPF application as a self-contained single-file executable.

$ErrorActionPreference = "Stop"

Write-Host "Publishing FolderLens.App for Windows x64..."
dotnet publish "FolderLens.App\FolderLens.App.csproj" -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishReadyToRun=true

Write-Host "Publish complete."
Write-Host "Executable is located in: FolderLens.App\bin\Release\net8.0-windows\win-x64\publish\"
Write-Host "You can now compile setup.iss using Inno Setup."
