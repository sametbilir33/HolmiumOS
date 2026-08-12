using System;
using System.Collections.Generic;
using System.IO;
using Cosmos.System;
using Cosmos.System.Graphics;
using HolmiumOS.GUI.Apps;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI
{
    public static class DesktopManager
    {
        private class DesktopIcon
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public bool IsDirectory { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; } = 80;
            public int Height { get; set; } = 80;
            public bool Selected { get; set; }
        }

        private class DesktopMenuItem
        {
            public string Text { get; set; }
            public int Y { get; set; }
            public int Height { get; set; } = 28;
        }

        private static readonly List<DesktopIcon> icons = new List<DesktopIcon>();
        private static readonly List<DesktopMenuItem> contextMenu = new List<DesktopMenuItem>();

        private static bool wasLeftPressed = false;
        private static bool wasRightPressed = false;
        private static string lastUser = "";
        private static DesktopIcon selectedIcon = null;
        private static DesktopIcon lastClickedIcon = null;
        private static long lastClickTime = 0;
        private static bool contextMenuVisible = false;
        private static int contextMenuX;
        private static int contextMenuY;
        private static DesktopIcon contextIcon = null;

        private const int IconStartX = 30;
        private const int IconStartY = 30;
        private const int IconSpacingX = 95;
        private const int IconSpacingY = 95;
        private const int MaxColumns = 12;
        private const int DoubleClickMilliseconds = 450;

        private static int CompareNames(string a, string b)
        {
            int length = a.Length < b.Length ? a.Length : b.Length;

            for (int i = 0; i < length; i++)
            {
                char ca = a[i];
                char cb = b[i];

                if (ca >= 'A' && ca <= 'Z') ca = (char)(ca + 32);
                if (cb >= 'A' && cb <= 'Z') cb = (char)(cb + 32);

                if (ca < cb) return -1;
                if (ca > cb) return 1;
            }

            if (a.Length < b.Length) return -1;
            if (a.Length > b.Length) return 1;

            return 0;
        }

        public static void RefreshIcons()
        {
            icons.Clear();
            selectedIcon = null;
            lastClickedIcon = null;
            contextIcon = null;
            contextMenuVisible = false;

            if (!UserManager.IsLoggedIn) return;

            string homeDir = UserManager.HomeDirectory;
            if (string.IsNullOrEmpty(homeDir) || !FileSystemManager.DirectoryExists(homeDir)) return;

            var entryList = new List<(string Path, string Name, bool IsDirectory)>();

            try
            {
                string[] dirs = FileSystemManager.GetDirectories(homeDir);
                foreach (string dir in dirs)
                {
                    if (!string.IsNullOrEmpty(dir))
                    {
                        string dirName = Path.GetFileName(dir.TrimEnd('/', '\\'));
                        if (!string.IsNullOrEmpty(dirName))
                        {
                            string fullPath = homeDir.TrimEnd('/', '\\') + "/" + dirName;
                            fullPath = FileSystemManager.ResolvePath(fullPath);

                            entryList.Add((fullPath, dirName, true));
                        }
                    }
                }

                string[] files = FileSystemManager.GetFiles(homeDir);
                foreach (string file in files)
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        string fileName = Path.GetFileName(file.TrimEnd('/', '\\'));
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            string fullPath = homeDir.TrimEnd('/', '\\') + "/" + fileName;
                            fullPath = FileSystemManager.ResolvePath(fullPath);

                            entryList.Add((fullPath, fileName, false));
                        }
                    }
                }

                for (int i = 0; i < entryList.Count - 1; i++)
                {
                    for (int j = i + 1; j < entryList.Count; j++)
                    {
                        if (CompareNames(entryList[i].Name, entryList[j].Name) > 0)
                        {
                            var temp = entryList[i];
                            entryList[i] = entryList[j];
                            entryList[j] = temp;
                        }
                    }
                }

                int col = 0;
                int row = 0;

                foreach (var entry in entryList)
                {
                    DesktopIcon icon = new DesktopIcon
                    {
                        Path = entry.Path,
                        Name = entry.Name,
                        IsDirectory = entry.IsDirectory,
                        X = IconStartX + (col * IconSpacingX),
                        Y = IconStartY + (row * IconSpacingY),
                        Width = 80,
                        Height = 80,
                        Selected = false
                    };

                    icons.Add(icon);
                    col++;

                    if (col >= MaxColumns)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
            catch { }
        }

        private static void OpenIcon(DesktopIcon icon)
        {
            if (icon == null) return;

            if (icon.IsDirectory)
            {
                var fileManager = new FileManager(icon.Path);
                AppManager.Run(fileManager);
            }
            else
            {
                var notepad = new Notepad(icon.Path);
                AppManager.Run(notepad);
            }
        }

        public static void Draw(Canvas canvas)
        {
            if (!UserManager.IsLoggedIn) return;

            if (lastUser != UserManager.CurrentUser)
            {
                lastUser = UserManager.CurrentUser;
                RefreshIcons();
            }

            foreach (DesktopIcon icon in icons)
            {
                DrawIcon(canvas, icon);
            }

            if (contextMenuVisible)
            {
                DrawContextMenu(canvas);
            }
        }

        private static void DrawIcon(Canvas canvas, DesktopIcon icon)
        {
            if (icon.Selected)
            {
                var selectionColor = System.Drawing.Color.FromArgb(90, 70, 130, 220);
                canvas.DrawFilledRectangle(selectionColor, icon.X, icon.Y, icon.Width, icon.Height);
            }

            int iconX = icon.X + 20;
            int iconY = icon.Y + 5;

            if (icon.IsDirectory)
            {
                DrawFolderIcon(canvas, iconX, iconY);
            }
            else
            {
                DrawFileIcon(canvas, iconX, iconY);
            }

            string displayName = icon.Name;
            if (displayName.Length > 11)
            {
                displayName = displayName.Substring(0, 9) + "..";
            }

            canvas.DrawString(
                displayName,
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                System.Drawing.Color.White,
                icon.X + 4,
                icon.Y + 50
            );
        }

        private static void DrawFolderIcon(Canvas canvas, int x, int y)
        {
            var folderColor = System.Drawing.Color.FromArgb(255, 255, 205, 55);
            var folderTopColor = System.Drawing.Color.FromArgb(255, 245, 190, 35);
            var folderBorder = System.Drawing.Color.FromArgb(255, 190, 145, 20);

            canvas.DrawFilledRectangle(folderTopColor, x + 4, y, 18, 8);
            canvas.DrawFilledRectangle(folderColor, x, y + 6, 44, 30);

            canvas.DrawLine(folderBorder, x, y + 6, x + 44, y + 6);
            canvas.DrawLine(folderBorder, x, y + 36, x + 44, y + 36);
            canvas.DrawLine(folderBorder, x, y + 6, x, y + 36);
            canvas.DrawLine(folderBorder, x + 44, y + 6, x + 44, y + 36);
        }

        private static void DrawFileIcon(Canvas canvas, int x, int y)
        {
            var fileColor = System.Drawing.Color.FromArgb(255, 245, 245, 245);
            var borderColor = System.Drawing.Color.FromArgb(255, 170, 175, 185);

            canvas.DrawFilledRectangle(fileColor, x + 6, y, 32, 38);

            canvas.DrawLine(borderColor, x + 6, y, x + 38, y);
            canvas.DrawLine(borderColor, x + 6, y + 38, x + 38, y + 38);
            canvas.DrawLine(borderColor, x + 6, y, x + 6, y + 38);
            canvas.DrawLine(borderColor, x + 38, y, x + 38, y + 38);
            canvas.DrawLine(borderColor, x + 28, y, x + 38, y + 10);
        }

        public static void UpdateMouse(Canvas canvas)
        {
            if (!UserManager.IsLoggedIn) return;

            int mouseX = (int)MouseManager.X;
            int mouseY = (int)MouseManager.Y;

            foreach (var window in WindowManager.GetWindows())
            {
                if (window != null && window.Contains(mouseX, mouseY))
                {
                    wasLeftPressed = MouseManager.MouseState == MouseState.Left;
                    wasRightPressed = MouseManager.MouseState == MouseState.Right;
                    return;
                }
            }

            bool leftPressed = MouseManager.MouseState == MouseState.Left;
            bool rightPressed = MouseManager.MouseState == MouseState.Right;

            if (contextMenuVisible)
            {
                if (leftPressed && !wasLeftPressed)
                {
                    HandleContextMenuClick(canvas, mouseX, mouseY);
                }

                wasLeftPressed = leftPressed;
                wasRightPressed = rightPressed;
                return;
            }

            if (rightPressed && !wasRightPressed)
            {
                DesktopIcon clickedIcon = GetIconAt(mouseX, mouseY);
                contextIcon = clickedIcon;

                if (clickedIcon != null)
                {
                    SelectIcon(clickedIcon);
                }

                ShowContextMenu(canvas, mouseX, mouseY);
                wasRightPressed = rightPressed;
                return;
            }

            if (leftPressed && !wasLeftPressed)
            {
                DesktopIcon icon = GetIconAt(mouseX, mouseY);
                if (icon != null)
                {
                    SelectIcon(icon);
                }
                else
                {
                    ClearSelection();
                }
            }

            if (!leftPressed && wasLeftPressed)
            {
                DesktopIcon icon = GetIconAt(mouseX, mouseY);
                if (icon != null)
                {
                    HandleIconClick(icon);
                }
            }

            wasLeftPressed = leftPressed;
            wasRightPressed = rightPressed;
        }
        private static void HandleIconClick(DesktopIcon icon)
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

            bool doubleClick = lastClickedIcon == icon && (now - lastClickTime) <= DoubleClickMilliseconds;

            if (doubleClick)
            {
                lastClickedIcon = null;
                lastClickTime = 0;
                OpenIcon(icon);
            }
            else
            {
                SelectIcon(icon);
                lastClickedIcon = icon;
                lastClickTime = now;
            }
        }

        private static DesktopIcon GetIconAt(int x, int y)
        {
            for (int i = icons.Count - 1; i >= 0; i--)
            {
                DesktopIcon icon = icons[i];

                if (x >= icon.X && x <= icon.X + icon.Width && y >= icon.Y && y <= icon.Y + icon.Height)
                {
                    return icon;
                }
            }

            return null;
        }

        private static void SelectIcon(DesktopIcon icon)
        {
            foreach (DesktopIcon item in icons)
            {
                item.Selected = false;
            }

            icon.Selected = true;
            selectedIcon = icon;
        }

        private static void ClearSelection()
        {
            foreach (DesktopIcon icon in icons)
            {
                icon.Selected = false;
            }

            selectedIcon = null;
        }

        private static void ShowContextMenu(Canvas canvas, int x, int y)
        {
            contextMenu.Clear();

            if (contextIcon != null)
            {
                contextMenu.Add(new DesktopMenuItem { Text = "Ac", Y = 0 });
                contextMenu.Add(new DesktopMenuItem { Text = "Sil", Y = 28 });
                contextMenu.Add(new DesktopMenuItem { Text = "Yenile", Y = 56 });
                contextMenu.Add(new DesktopMenuItem { Text = "Ikonlari Sirala", Y = 84 });
            }
            else
            {
                contextMenu.Add(new DesktopMenuItem { Text = "Yeni Klasor", Y = 0 });
                contextMenu.Add(new DesktopMenuItem { Text = "Yeni Dosya", Y = 28 });
                contextMenu.Add(new DesktopMenuItem { Text = "Yenile", Y = 56 });
                contextMenu.Add(new DesktopMenuItem { Text = "Ikonlari Sirala", Y = 84 });
            }

            contextMenuX = x;
            contextMenuY = y;

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;
            int menuWidth = 150;
            int menuHeight = contextMenu.Count * 28;

            if (contextMenuX + menuWidth > screenWidth) contextMenuX = screenWidth - menuWidth;
            if (contextMenuY + menuHeight > screenHeight) contextMenuY = screenHeight - menuHeight;
            if (contextMenuX < 0) contextMenuX = 0;
            if (contextMenuY < 0) contextMenuY = 0;

            contextMenuVisible = true;
        }

        private static void DrawContextMenu(Canvas canvas)
        {
            int width = 150;
            int height = contextMenu.Count * 28;

            var background = System.Drawing.Color.FromArgb(245, 25, 28, 38);
            var border = System.Drawing.Color.FromArgb(255, 120, 130, 150);

            canvas.DrawFilledRectangle(background, contextMenuX, contextMenuY, width, height);

            canvas.DrawLine(border, contextMenuX, contextMenuY, contextMenuX + width, contextMenuY);
            canvas.DrawLine(border, contextMenuX, contextMenuY + height, contextMenuX + width, contextMenuY + height);
            canvas.DrawLine(border, contextMenuX, contextMenuY, contextMenuX, contextMenuY + height);
            canvas.DrawLine(border, contextMenuX + width, contextMenuY, contextMenuX + width, contextMenuY + height);

            foreach (DesktopMenuItem item in contextMenu)
            {
                canvas.DrawString(
                    item.Text,
                    Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                    System.Drawing.Color.White,
                    contextMenuX + 10,
                    contextMenuY + item.Y + 8
                );
            }
        }

        private static void HandleContextMenuClick(Canvas canvas, int x, int y)
        {
            int width = 150;
            int height = contextMenu.Count * 28;

            if (x < contextMenuX || x > contextMenuX + width || y < contextMenuY || y > contextMenuY + height)
            {
                contextMenuVisible = false;
                contextIcon = null;
                return;
            }

            int relativeY = y - contextMenuY;
            int index = relativeY / 28;

            if (index < 0 || index >= contextMenu.Count) return;

            string action = contextMenu[index].Text;
            contextMenuVisible = false;

            if (action == "Ac")
            {
                if (contextIcon != null) OpenIcon(contextIcon);
            }
            else if (action == "Sil")
            {
                if (contextIcon != null)
                {
                    try
                    {
                        if (contextIcon.IsDirectory)
                        {
                            if (Directory.Exists(contextIcon.Path))
                            {
                                Directory.Delete(contextIcon.Path, true);
                            }
                        }
                        else
                        {
                            if (File.Exists(contextIcon.Path))
                            {
                                File.Delete(contextIcon.Path);
                            }
                        }
                        RefreshIcons();
                    }
                    catch { }
                }
            }
            else if (action == "Yeni Klasor")
            {
                string defaultPath = UserManager.IsLoggedIn ? UserManager.HomeDirectory : "0:/home";
                var msg = new MessageBox(
                    "Yeni Klasor",
                    "Klasor adi girin:",
                    true,
                    "YeniKlasor",
                    (inputName) =>
                    {
                        if (!string.IsNullOrEmpty(inputName))
                        {
                            try
                            {
                                string targetPath = defaultPath.TrimEnd('/', '\\') + "/" + inputName.Trim();
                                targetPath = FileSystemManager.ResolvePath(targetPath);
                                FileSystemManager.CreateDirectory(targetPath);
                                RefreshIcons();
                            }
                            catch { }
                        }
                    }
                );
                AppManager.Run(msg);
            }
            else if (action == "Yeni Dosya")
            {
                string defaultPath = UserManager.IsLoggedIn ? UserManager.HomeDirectory : "0:/home";
                var msg = new MessageBox(
                    "Yeni Dosya",
                    "Dosya adi girin (orn: not.txt):",
                    true,
                    "yeni.txt",
                    (inputName) =>
                    {
                        if (!string.IsNullOrEmpty(inputName))
                        {
                            try
                            {
                                string targetPath = defaultPath.TrimEnd('/', '\\') + "/" + inputName.Trim();
                                targetPath = FileSystemManager.ResolvePath(targetPath);
                                FileSystemManager.WriteFile(targetPath, "");
                                RefreshIcons();
                            }
                            catch { }
                        }
                    }
                );
                AppManager.Run(msg);
            }
            else if (action == "Yenile")
            {
                RefreshIcons();
            }
            else if (action == "Ikonlari Sirala")
            {
                ArrangeIcons();
            }

            contextIcon = null;
        }

        private static void ArrangeIcons()
        {
            RefreshIcons();
        }
    }
}