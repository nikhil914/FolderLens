[Setup]
AppName=FolderLens
AppVersion=1.0.0
DefaultDirName={autopf}\FolderLens
DefaultGroupName=FolderLens
UninstallDisplayIcon={app}\FolderLens.App.exe
Compression=lzma2
SolidCompression=yes
OutputDir=.
OutputBaseFilename=FolderLensSetup
PrivilegesRequired=admin

[Files]
Source: "FolderLens.App\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\FolderLens"; Filename: "{app}\FolderLens.App.exe"
Name: "{autodesktop}\FolderLens"; Filename: "{app}\FolderLens.App.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a &desktop icon"; GroupDescription: "Additional icons:"

[Run]
Filename: "{app}\FolderLens.App.exe"; Description: "Launch FolderLens"; Flags: nowait postinstall skipifsilent

[Registry]
; Context menu integration (Optional, can be handled by the app itself as we have the ContextMenuInstaller class)
; But typically an installer registers these:
Root: HKCR; Subkey: "Directory\shell\FolderLens"; ValueType: string; ValueData: "Analyze with FolderLens"
Root: HKCR; Subkey: "Directory\shell\FolderLens\command"; ValueType: string; ValueData: """{app}\FolderLens.App.exe"" ""%1"""
Root: HKCR; Subkey: "Directory\Background\shell\FolderLens"; ValueType: string; ValueData: "Analyze this folder with FolderLens"
Root: HKCR; Subkey: "Directory\Background\shell\FolderLens\command"; ValueType: string; ValueData: """{app}\FolderLens.App.exe"" ""%v"""
