using System;
using System.Collections.Generic;
using HolmiumOS.GUI.Controls;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI.Apps
{
    public class FileManager : AppBase
    {
        private Button btnBack;
        private TextBox txtPath;
        private Button btnGo;
        private Label lblStatus;

        private Button btnUp;
        private Button btnDown;

        private ListBox lstFiles;

        private string currentPath = "/";

        private int scrollOffset = 0;
        private const int PAGE_SIZE = 12;

        private string initialPathOverride = null;

        public FileManager() : base("Dosya Yoneticisi")
        {
        }

        public FileManager(string initialPath = null) : base("Dosya Yoneticisi")
        {
            initialPathOverride = initialPath;
        }

        public override void Load()
        {
            try
            {
                if (Window != null)
                {
                    Window.Title = "Dosya Yoneticisi";
                }

                InitUserPath();

                btnBack = new Button(
                    "<-",
                    10,
                    10,
                    45,
                    30)
                {
                    ClickAction = DoNavigateBack
                };

                txtPath = new TextBox(
                    60,
                    10,
                    360,
                    30)
                {
                    Text = currentPath,
                    MaxLength = 100
                };

                btnGo = new Button(
                    "Go",
                    425,
                    10,
                    65,
                    30)
                {
                    ClickAction = DoNavigateGo
                };

                btnUp = new Button(
                    "^",
                    450,
                    50,
                    40,
                    140)
                {
                    ClickAction = ScrollUp
                };

                btnDown = new Button(
                    "v",
                    450,
                    195,
                    40,
                    140)
                {
                    ClickAction = ScrollDown
                };

                lblStatus = new Label(
                    "Hazir",
                    10,
                    360);

                if (Window != null)
                {
                    Window.AddControl(btnBack);
                    Window.AddControl(txtPath);
                    Window.AddControl(btnGo);
                    Window.AddControl(btnUp);
                    Window.AddControl(btnDown);
                    Window.AddControl(lblStatus);
                }

                LoadDirectory(currentPath);
            }
            catch
            {
                SetStatus("Baslatma hatasi!");
            }
        }

        private void InitUserPath()
        {
            try
            {
                if (!string.IsNullOrEmpty(initialPathOverride) &&
                    FileSystemManager.DirectoryExists(initialPathOverride))
                {
                    currentPath =
                        FileSystemManager.ResolvePath(
                            initialPathOverride);
                }
                else if (UserManager.IsLoggedIn &&
                         !string.IsNullOrEmpty(
                             UserManager.HomeDirectory) &&
                         FileSystemManager.DirectoryExists(
                             UserManager.HomeDirectory))
                {
                    currentPath =
                        FileSystemManager.ResolvePath(
                            UserManager.HomeDirectory);
                }
                else
                {
                    currentPath = "/";
                }
            }
            catch
            {
                currentPath = "/";
            }

            FixPath();
        }

        private void FixPath()
        {
            if (string.IsNullOrWhiteSpace(currentPath))
            {
                currentPath = "/";
                return;
            }

            currentPath =
                FileSystemManager.ResolvePath(currentPath);
        }

        private void LoadDirectory(string path)
        {
            currentPath =
                FileSystemManager.ResolvePath(path);

            FixPath();

            if (txtPath != null)
            {
                txtPath.Text = currentPath;
            }

            if (lstFiles != null && Window != null)
            {
                try
                {
                    Window.Controls.Remove(lstFiles);
                }
                catch
                {
                }

                lstFiles = null;
            }

            lstFiles = new ListBox(
                10,
                50,
                435,
                285);

            List<ItemEntry> items =
                new List<ItemEntry>();

            try
            {
                if (!PermissionManager.CanRead(currentPath))
                {
                    SetStatus("Erisim Yetkisi Yok!");
                    AttachListBoxToWindow();
                    return;
                }
            }
            catch
            {
            }

            try
            {
                string[] dirs =
                    FileSystemManager.GetDirectories(
                        currentPath);

                if (dirs != null)
                {
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(dirs[i]))
                        {
                            items.Add(
                                new ItemEntry
                                {
                                    Path = dirs[i],
                                    IsDirectory = true
                                });
                        }
                    }
                }
            }
            catch
            {
            }

            try
            {
                string[] files =
                    FileSystemManager.GetFiles(
                        currentPath);

                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(files[i]))
                        {
                            items.Add(
                                new ItemEntry
                                {
                                    Path = files[i],
                                    IsDirectory = false
                                });
                        }
                    }
                }
            }
            catch
            {
            }

            List<ItemEntry> visibleItems =
                new List<ItemEntry>();

            if (scrollOffset < 0)
            {
                scrollOffset = 0;
            }

            if (scrollOffset >
                Math.Max(0, items.Count - PAGE_SIZE))
            {
                scrollOffset =
                    Math.Max(
                        0,
                        items.Count - PAGE_SIZE);
            }

            int endLimit =
                Math.Min(
                    scrollOffset + PAGE_SIZE,
                    items.Count);

            for (
                int i = scrollOffset;
                i < endLimit;
                i++)
            {
                ItemEntry entry = items[i];

                string cleanName =
                    ExtractName(entry.Path);

                if (entry.IsDirectory)
                {
                    lstFiles.AddItem(
                        "[DIR]  " +
                        Shorten(cleanName, 32));
                }
                else
                {
                    lstFiles.AddItem(
                        "[FILE] " +
                        Shorten(cleanName, 31));
                }

                visibleItems.Add(entry);
            }

            if (items.Count > PAGE_SIZE)
            {
                SetStatus(
                    "Gosterilen: " +
                    (scrollOffset + 1) +
                    "-" +
                    endLimit +
                    " / " +
                    items.Count);
            }
            else
            {
                SetStatus(
                    items.Count == 0
                        ? "Klasor bos."
                        : "Toplam: " + items.Count);
            }

            lstFiles.OnSelectedIndexChanged =
                (index, text) =>
                {
                    if (index >= 0 &&
                        index < visibleItems.Count)
                    {
                        ItemEntry selected =
                            visibleItems[index];

                        if (selected.IsDirectory)
                        {
                            scrollOffset = 0;

                            string newPath =
                                FileSystemManager.ResolvePath(
                                    selected.Path);

                            LoadDirectory(newPath);
                        }
                        else
                        {
                            string filePath =
                                FileSystemManager.ResolvePath(
                                    selected.Path);

                            string fileName =
                                ExtractName(filePath);

                            var notepad =
                                new Notepad(filePath);

                            AppManager.Run(notepad);

                            SetStatus(
                                "Dosya: " +
                                Shorten(fileName, 30));
                        }
                    }
                };

            AttachListBoxToWindow();
        }

        private void ScrollUp()
        {
            if (scrollOffset > 0)
            {
                scrollOffset--;
                LoadDirectory(currentPath);
            }
        }

        private void ScrollDown()
        {
            scrollOffset++;

            LoadDirectory(currentPath);
        }

        private void AttachListBoxToWindow()
        {
            if (Window != null &&
                lstFiles != null)
            {
                Window.AddControl(lstFiles);
            }
        }

        private void DoNavigateBack()
        {
            try
            {
                string path =
                    FileSystemManager.ResolvePath(
                        currentPath);

                if (path == "/")
                {
                    return;
                }

                int lastSlash =
                    path.LastIndexOf('/');

                string parent;

                if (lastSlash <= 0)
                {
                    parent = "/";
                }
                else
                {
                    parent =
                        path.Substring(
                            0,
                            lastSlash);
                }

                scrollOffset = 0;

                LoadDirectory(parent);
            }
            catch
            {
                scrollOffset = 0;
                LoadDirectory("/");
            }
        }

        private void DoNavigateGo()
        {
            if (txtPath == null)
            {
                return;
            }

            string target =
                txtPath.Text.Trim();

            if (string.IsNullOrEmpty(target))
            {
                return;
            }

            string resolved =
                FileSystemManager.ResolvePath(target);

            if (FileSystemManager.DirectoryExists(resolved))
            {
                scrollOffset = 0;
                LoadDirectory(resolved);
            }
            else
            {
                SetStatus(
                    "Klasor bulunamadi!");
            }
        }

        private string ExtractName(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return "";
            }

            string path =
                fullPath.TrimEnd('/');

            if (path == "/")
            {
                return "/";
            }

            int index =
                path.LastIndexOf('/');

            if (index >= 0 &&
                index < path.Length - 1)
            {
                return path.Substring(index + 1);
            }

            return path;
        }

        private string Shorten(
            string text,
            int maxLen)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            if (text.Length <= maxLen)
            {
                return text;
            }

            return text.Substring(
                0,
                maxLen - 3) + "...";
        }

        private void SetStatus(string msg)
        {
            if (lblStatus != null)
            {
                lblStatus.Text = msg;
            }
        }

        private class ItemEntry
        {
            public string Path { get; set; }
            public bool IsDirectory { get; set; }
        }
    }
}