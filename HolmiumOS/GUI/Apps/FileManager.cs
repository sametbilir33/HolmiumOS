using System;
using System.Collections.Generic;
using HolmiumOS.GUI.Controls;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI.Apps
{
    public class FileEntry
    {
        public string Name { get; set; } = "";
        public string FullPath { get; set; } = "";
        public bool IsDirectory { get; set; }
    }

    public class FileManager : AppBase
    {
        private TextBox pathTextBox;
        private Button btnBack;
        private Button btnGo;
        private Label statusLabel;
        private ListBox fileListBox;

        private string localPath = @"0:\";
        private List<FileEntry> currentEntries = new List<FileEntry>();

        public FileManager() : base("File Manager")
        {
        }

        public override void Load()
        {
            try
            {
                if (this.Window != null)
                {
                    this.Window.Title = "File Manager";
                }

                InitPath();

                btnBack = new Button("<-", 10, 10, 35, 25);
                btnBack.ClickAction = OnClickBack;

                pathTextBox = new TextBox(50, 10, 180, 25);
                pathTextBox.Text = localPath ?? @"0:\";
                pathTextBox.MaxLength = 100;

                btnGo = new Button("Go", 235, 10, 45, 25);
                btnGo.ClickAction = OnClickGo;

                fileListBox = new ListBox(10, 45, 270, 180);

                statusLabel = new Label("Hazir                                ", 10, 235);

                if (this.Window != null)
                {
                    this.Window.AddControl(btnBack);
                    this.Window.AddControl(pathTextBox);
                    this.Window.AddControl(btnGo);
                    this.Window.AddControl(fileListBox);
                    this.Window.AddControl(statusLabel);
                }

                EnsureDirectoryExists(localPath);
                RefreshDirectory();
            }
            catch
            {
            }
        }

        private void InitPath()
        {
            try
            {
                if (UserManager.IsLoggedIn && !string.IsNullOrEmpty(UserManager.CurrentUser))
                {
                    localPath = UserManager.HomeDirectory;
                }
            }
            catch
            {
                localPath = @"0:\";
            }

            FormatPath();
        }

        private void FormatPath()
        {
            if (string.IsNullOrEmpty(localPath))
            {
                localPath = @"0:\";
                return;
            }

            localPath = localPath.Replace('/', '\\');

            if (localPath.StartsWith("\\"))
            {
                localPath = "0:" + localPath;
            }
            else if (!localPath.StartsWith("0:\\") && !localPath.StartsWith("0:"))
            {
                localPath = @"0:\" + localPath;
            }

            localPath = localPath.Replace("\\\\", "\\");

            if (!localPath.EndsWith("\\"))
            {
                localPath += "\\";
            }
        }

        private void EnsureDirectoryExists(string path)
        {
            try
            {
                if (!FileSystemManager.DirectoryExists(path))
                {
                    FileSystemManager.CreateDirectory(path);
                }
            }
            catch
            {
            }
        }

        private string SafeGetFileName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath)) return "";

            int len = fullPath.Length;
            while (len > 0 && (fullPath[len - 1] == '\\' || fullPath[len - 1] == '/')) len--;

            if (len == 0) return "";

            int lastSlash = -1;
            for (int i = len - 1; i >= 0; i--)
            {
                if (fullPath[i] == '\\' || fullPath[i] == '/')
                {
                    lastSlash = i;
                    break;
                }
            }

            if (lastSlash == -1) return fullPath;
            return fullPath.Substring(lastSlash + 1, len - (lastSlash + 1));
        }

        private void RefreshDirectory()
        {
            if (fileListBox == null || pathTextBox == null)
                return;

            fileListBox.OnSelectedIndexChanged = null;

            currentEntries.Clear();
            fileListBox.Clear();

            try
            {
                bool canRead = true;

                try
                {
                    canRead = PermissionManager.CanRead(localPath);
                }
                catch
                {
                }

                if (!canRead)
                {
                    localPath = @"0:\";
                    FormatPath();
                }

                string[] dirs = null;

                try
                {
                    dirs = FileSystemManager.GetDirectories(localPath);
                }
                catch
                {
                }

                if (dirs != null)
                {
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        string dir = dirs[i];

                        if (string.IsNullOrEmpty(dir))
                            continue;

                        string name = SafeGetFileName(dir);

                        currentEntries.Add(new FileEntry
                        {
                            Name = name,
                            FullPath = dir,
                            IsDirectory = true
                        });

                        fileListBox.AddItem("[DIR] " + name);
                    }
                }

                string[] files = null;

                try
                {
                    files = FileSystemManager.GetFiles(localPath);
                }
                catch
                {
                }

                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        string file = files[i];

                        if (string.IsNullOrEmpty(file))
                            continue;

                        string name = SafeGetFileName(file);

                        currentEntries.Add(new FileEntry
                        {
                            Name = name,
                            FullPath = file,
                            IsDirectory = false
                        });

                        fileListBox.AddItem("[FILE] " + name);
                    }
                }

                pathTextBox.Text = localPath;

                if (statusLabel != null)
                {
                    statusLabel.Text = currentEntries.Count == 0
                        ? "Klasor bos."
                        : $"Toplam {currentEntries.Count} oge.";
                }
            }
            catch
            {
                if (statusLabel != null)
                    statusLabel.Text = "Hata olustu.";
            }

            fileListBox.OnSelectedIndexChanged = OnFileSelected;
        }

        private void OnFileSelected(int selectedIndex, string selectedItemText)
        {
            if (selectedIndex < 0 || selectedIndex >= currentEntries.Count)
                return;

            var selectedEntry = currentEntries[selectedIndex];

            try
            {
                if (selectedEntry.IsDirectory)
                {
                    localPath = selectedEntry.FullPath;
                    FormatPath();
                    RefreshDirectory();
                }
                else if (statusLabel != null)
                {
                    statusLabel.Text = "Secildi: " + selectedEntry.Name;
                }
            }
            catch
            {
            }
        }

        private void OnClickBack()
        {
            try
            {
                string trimmedPath = localPath.TrimEnd('\\', '/');

                if (trimmedPath.Length <= 3 || (!trimmedPath.Contains("\\") && !trimmedPath.Contains("/")))
                {
                    return;
                }

                int lastIndex = Math.Max(trimmedPath.LastIndexOf('\\'), trimmedPath.LastIndexOf('/'));
                if (lastIndex > 0)
                {
                    localPath = trimmedPath.Substring(0, lastIndex + 1);
                    FormatPath();
                    RefreshDirectory();
                }
            }
            catch
            {
            }
        }

        private void OnClickGo()
        {
            if (pathTextBox == null) return;

            try
            {
                string targetPath = pathTextBox.Text.Trim();
                if (string.IsNullOrEmpty(targetPath)) return;

                localPath = targetPath;
                FormatPath();

                if (FileSystemManager.DirectoryExists(localPath))
                {
                    RefreshDirectory();
                }
                else
                {
                    if (statusLabel != null) statusLabel.Text = "Klasor bulunamadi!";
                }
            }
            catch
            {
            }
        }
    }
}