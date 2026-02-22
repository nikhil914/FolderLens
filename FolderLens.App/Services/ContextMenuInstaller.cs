using System;
using Microsoft.Win32;

namespace FolderLens.App.Services
{
    public static class ContextMenuInstaller
    {
        private const string MenuTitleBackground = "Analyze This Folder with FolderLens";
        private const string MenuTitleDirectory = "Analyze with FolderLens";

        public static void Install(string exePath)
        {
            try
            {
                // Background of a directory
                using (var key = Registry.ClassesRoot.CreateSubKey(@"Directory\Background\shell\FolderLens"))
                {
                    if (key != null)
                    {
                        key.SetValue("", MenuTitleBackground);
                        key.SetValue("Icon", $"\"{exePath}\",0");

                        using (var commandKey = key.CreateSubKey("command"))
                        {
                            commandKey?.SetValue("", $"\"{exePath}\" \"%V\"");
                        }
                    }
                }

                // Directory item
                using (var key = Registry.ClassesRoot.CreateSubKey(@"Directory\shell\FolderLens"))
                {
                    if (key != null)
                    {
                        key.SetValue("", MenuTitleDirectory);
                        key.SetValue("Icon", $"\"{exePath}\",0");

                        using (var commandKey = key.CreateSubKey("command"))
                        {
                            commandKey?.SetValue("", $"\"{exePath}\" \"%1\"");
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException("Installation of context menu requires Administrator privileges.", ex);
            }
        }

        public static void Remove()
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\Background\shell\FolderLens", false);
                Registry.ClassesRoot.DeleteSubKeyTree(@"Directory\shell\FolderLens", false);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new InvalidOperationException("Removal of context menu requires Administrator privileges.", ex);
            }
        }

        public static bool IsInstalled()
        {
            using var key = Registry.ClassesRoot.OpenSubKey(@"Directory\shell\FolderLens");
            return key != null;
        }
    }
}
