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
            public int Width { get; set; } = 70;
            public int Height { get; set; } = 70;
        }

        private static List<DesktopIcon> icons = new List<DesktopIcon>();
        private static bool wasPressed = false;
        private static string lastUser = "";

        public static void RefreshIcons()
        {
            icons.Clear();
            if (!UserManager.IsLoggedIn) return;

            string homeDir = UserManager.HomeDirectory;
            if (!Directory.Exists(homeDir)) return;
            homeDir = homeDir.TrimEnd('/');

            int startX = 30;
            int startY = 30;
            int spacingX = 90;
            int spacingY = 90;
            int maxColumns = 12;

            int col = 0;
            int row = 0;

            try
            {
                string[] dirs = Directory.GetDirectories(homeDir);
                foreach (var dir in dirs)
                {
                    string name = Path.GetFileName(dir);
                    string fullPath = dir.Contains(":") ? dir : $"{homeDir}/{name}";

                    icons.Add(new DesktopIcon
                    {
                        Path = fullPath,
                        Name = name,
                        IsDirectory = true,
                        X = startX + (col * spacingX),
                        Y = startY + (row * spacingY)
                    });

                    col++;
                    if (col >= maxColumns)
                    {
                        col = 0;
                        row++;
                    }
                }

                string[] files = Directory.GetFiles(homeDir);
                foreach (var file in files)
                {
                    string name = Path.GetFileName(file);
                    string fullPath = file.Contains(":") ? file : $"{homeDir}/{name}";

                    icons.Add(new DesktopIcon
                    {
                        Path = fullPath,
                        Name = name,
                        IsDirectory = false,
                        X = startX + (col * spacingX),
                        Y = startY + (row * spacingY)
                    });

                    col++;
                    if (col >= maxColumns)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
            catch { }
        }
        public static void Draw(Canvas canvas)
        {
            if (!UserManager.IsLoggedIn) return;

            if (lastUser != UserManager.CurrentUser)
            {
                lastUser = UserManager.CurrentUser;
                RefreshIcons();
            }

            foreach (var icon in icons)
            {
                System.Drawing.Color bgColor = System.Drawing.Color.FromArgb(80, 20, 20, 40);
                System.Drawing.Color borderColor = System.Drawing.Color.FromArgb(150, 200, 255);

                canvas.DrawFilledRectangle(bgColor, icon.X, icon.Y, icon.Width, icon.Height);

                canvas.DrawLine(borderColor, icon.X, icon.Y, icon.X + icon.Width, icon.Y);
                canvas.DrawLine(borderColor, icon.X, icon.Y + icon.Height, icon.X + icon.Width, icon.Y + icon.Height);
                canvas.DrawLine(borderColor, icon.X, icon.Y, icon.X, icon.Y + icon.Height);
                canvas.DrawLine(borderColor, icon.X + icon.Width, icon.Y, icon.X + icon.Width, icon.Y + icon.Height);

                string displayName = icon.Name;
                if (displayName.Length > 10)
                {
                    displayName = displayName.Substring(0, 8) + "..";
                }

                canvas.DrawString(displayName, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, System.Drawing.Color.White, icon.X + 4, icon.Y + icon.Height - 16);
            }
        }

        public static void UpdateMouse(Canvas canvas)
        {
            if (!UserManager.IsLoggedIn) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool isPressed = (MouseManager.MouseState == MouseState.Left);

            if (isPressed && !wasPressed)
            {
                var windows = WindowManager.GetWindows();
                bool clickedOnWindow = false;

                foreach (var window in windows)
                {
                    if (window != null && window.Contains(mx, my))
                    {
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (!clickedOnWindow)
                {
                    foreach (var icon in icons)
                    {
                        if (mx >= icon.X && mx <= icon.X + icon.Width &&
                            my >= icon.Y && my <= icon.Y + icon.Height)
                        {
                            OpenIcon(icon);
                            break;
                        }
                    }
                }
            }

            wasPressed = isPressed;
        }
        private static void OpenIcon(DesktopIcon icon)
        {
            if (icon.IsDirectory)
            {
                var fileManager = new FileManager();
                AppManager.Run(fileManager);
            }
            else
            {
                var notepad = new Notepad(icon.Path);
                AppManager.Run(notepad);
            }
        }
    }
}