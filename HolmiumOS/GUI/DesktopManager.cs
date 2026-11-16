
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using Cosmos.Kernel.System.Input;
using HolmiumOS.GUI.Apps;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI
{
    public static class DesktopManager
    {
        private class DesktopIcon
        {
            public string Path { get; set; } = "";
            public string Name { get; set; } = "";
            public bool IsDirectory { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; } = 100;
            public int Height { get; set; } = 100;
            public bool Selected { get; set; }
        }

        private class DesktopMenuItem
        {
            public string Text { get; set; } = "";
            public int Y { get; set; }
            public int Height { get; set; } = 36;
        }

        private static readonly List<DesktopIcon> icons = new List<DesktopIcon>();
        private static readonly List<DesktopMenuItem> contextMenu = new List<DesktopMenuItem>();

        private static bool wasLeftPressed = false;
        private static bool wasRightPressed = false;
        private static string lastUser = "";

        private static DesktopIcon? selectedIcon = null;
        private static DesktopIcon? lastClickedIcon = null;
        private static DesktopIcon? contextIcon = null;

        private static long lastClickTime = 0;
        private static bool contextMenuVisible = false;
        private static int contextMenuX;
        private static int contextMenuY;
        private static int hoveredMenuItem = -1;

        private static bool isSelecting = false;
        private static int selectionStartX;
        private static int selectionStartY;
        private static int selectionEndX;
        private static int selectionEndY;

        private const int IconStartX = 30;
        private const int IconStartY = 30;
        private const int IconSpacingX = 110;
        private const int IconSpacingY = 115;
        private const int MaxColumns = 12;
        private const int DoubleClickMilliseconds = 450;
        private const int IconWidth = 100;
        private const int IconHeight = 100;
        private const int ContextMenuItemHeight = 36;
        private const int ContextMenuPadding = 12;

        private static readonly Color DesktopTextColor = Color.FromArgb(255, 255, 255);
        private static readonly Color DesktopTextShadow = Color.FromArgb(110, 0, 0, 0);

        private static readonly Color MenuBackground = Color.FromArgb(246, 245, 244);
        private static readonly Color MenuBorder = Color.FromArgb(205, 203, 199);
        private static readonly Color MenuText = Color.FromArgb(36, 36, 36);
        private static readonly Color MenuHover = Color.FromArgb(225, 237, 250);
        private static readonly Color MenuSeparator = Color.FromArgb(225, 223, 220);
        private static readonly Color MenuShadow = Color.FromArgb(55, 0, 0, 0);

        private static readonly Color SelectionBoxFill = Color.FromArgb(55, 53, 132, 228);
        private static readonly Color SelectionBoxBorder = Color.FromArgb(150, 53, 132, 228);
        private static readonly Color IconSelectionFill = Color.FromArgb(65, 53, 132, 228);
        private static readonly Color IconSelectionBorder = Color.FromArgb(130, 53, 132, 228);

        private static readonly Color FolderColor = Color.FromArgb(255, 210, 105);
        private static readonly Color FolderLightColor = Color.FromArgb(255, 224, 145);
        private static readonly Color FolderDarkColor = Color.FromArgb(190, 135, 48);
        private static readonly Color FileColor = Color.FromArgb(255, 255, 255);
        private static readonly Color FileBorderColor = Color.FromArgb(190, 190, 190);
        private static readonly Color FileLineColor = Color.FromArgb(190, 198, 208);

        private static int CompareNames(string a, string b)
        {
            int length = a.Length < b.Length ? a.Length : b.Length;

            for (int i = 0; i < length; i++)
            {
                char ca = a[i];
                char cb = b[i];

                if (ca >= 'A' && ca <= 'Z')
                    ca = (char)(ca + 32);

                if (cb >= 'A' && cb <= 'Z')
                    cb = (char)(cb + 32);

                if (ca < cb)
                    return -1;

                if (ca > cb)
                    return 1;
            }

            if (a.Length < b.Length)
                return -1;

            if (a.Length > b.Length)
                return 1;

            return 0;
        }

        private static int MeasureText(Font font, string text)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.MeasureString(text, FontManager.UiFontSize);

            return font.MeasureString(text);
        }

        private static int GetFontHeight(Font font)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.GetLineHeight(FontManager.UiFontSize);

            return font.Height;
        }

        private static void DrawText(Canvas canvas, string text, Font font, Color color, int x, int y)
        {
            if (font is TrueTypeFont trueTypeFont)
                canvas.DrawString(text, trueTypeFont, FontManager.UiFontSize, color, x, y);
            else
                canvas.DrawString(text, font, color, x, y);
        }

        private static void FillRoundedRect(Canvas canvas, int x, int y, int width, int height, int radius, Color color)
        {
            if (width <= 0 || height <= 0)
                return;

            radius = Math.Min(radius, Math.Min(width / 2, height / 2));

            if (radius <= 0)
            {
                canvas.DrawFilledRectangle(color, x, y, width, height);
                return;
            }

            canvas.DrawFilledRectangle(color, x + radius, y, width - radius * 2, height);
            canvas.DrawFilledRectangle(color, x, y + radius, width, height - radius * 2);
            canvas.DrawFilledCircle(color, x + radius, y + radius, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
            canvas.DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        private static void DrawRoundedOutline(Canvas canvas, int x, int y, int width, int height, int radius, Color color)
        {
            if (width <= 0 || height <= 0)
                return;

            radius = Math.Min(radius, Math.Min(width / 2, height / 2));

            canvas.DrawLine(color, x + radius, y, x + width - radius - 1, y);
            canvas.DrawLine(color, x + radius, y + height - 1, x + width - radius - 1, y + height - 1);
            canvas.DrawLine(color, x, y + radius, x, y + height - radius - 1);
            canvas.DrawLine(color, x + width - 1, y + radius, x + width - 1, y + height - radius - 1);

            canvas.DrawLine(color, x + 2, y + 1, x + radius - 1, y + 1);
            canvas.DrawLine(color, x + width - radius, y + 1, x + width - 3, y + 1);
            canvas.DrawLine(color, x + 2, y + height - 2, x + radius - 1, y + height - 2);
            canvas.DrawLine(color, x + width - radius, y + height - 2, x + width - 3, y + height - 2);
        }

        public static void RefreshIcons()
        {
            icons.Clear();
            selectedIcon = null;
            lastClickedIcon = null;
            contextIcon = null;
            contextMenuVisible = false;
            hoveredMenuItem = -1;
            isSelecting = false;

            if (!UserManager.IsLoggedIn)
                return;

            string homeDir = UserManager.HomeDirectory;

            if (string.IsNullOrEmpty(homeDir) || !FileSystemManager.DirectoryExists(homeDir))
                return;

            var entryList = new List<(string Path, string Name, bool IsDirectory)>();

            try
            {
                string[] dirs = FileSystemManager.GetDirectories(homeDir);

                foreach (string dir in dirs)
                {
                    if (string.IsNullOrEmpty(dir))
                        continue;

                    string dirName = Path.GetFileName(dir.TrimEnd('/', '\\'));

                    if (string.IsNullOrEmpty(dirName))
                        continue;

                    string fullPath = homeDir.TrimEnd('/', '\\') + "/" + dirName;
                    fullPath = FileSystemManager.ResolvePath(fullPath);
                    entryList.Add((fullPath, dirName, true));
                }

                string[] files = FileSystemManager.GetFiles(homeDir);

                foreach (string file in files)
                {
                    if (string.IsNullOrEmpty(file))
                        continue;

                    string fileName = Path.GetFileName(file.TrimEnd('/', '\\'));

                    if (string.IsNullOrEmpty(fileName))
                        continue;

                    string fullPath = homeDir.TrimEnd('/', '\\') + "/" + fileName;
                    fullPath = FileSystemManager.ResolvePath(fullPath);
                    entryList.Add((fullPath, fileName, false));
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
                    icons.Add(new DesktopIcon
                    {
                        Path = entry.Path,
                        Name = entry.Name,
                        IsDirectory = entry.IsDirectory,
                        X = IconStartX + col * IconSpacingX,
                        Y = IconStartY + row * IconSpacingY,
                        Width = IconWidth,
                        Height = IconHeight,
                        Selected = false
                    });

                    col++;

                    if (col >= MaxColumns)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
            catch
            {
            }
        }

        private static void OpenIcon(DesktopIcon? icon)
        {
            if (icon == null)
                return;

            if (icon.IsDirectory)
                AppManager.Run(new FileManager(icon.Path));
            else
                AppManager.Run(new Notepad(icon.Path));
        }

        public static void Draw(Canvas canvas)
        {
            if (!UserManager.IsLoggedIn)
                return;

            if (lastUser != UserManager.CurrentUser)
            {
                lastUser = UserManager.CurrentUser;
                RefreshIcons();
            }

            if (isSelecting)
                DrawSelectionBox(canvas);

            foreach (DesktopIcon icon in icons)
                DrawIcon(canvas, icon);

            if (contextMenuVisible)
                DrawContextMenu(canvas);
        }

        private static void DrawIcon(Canvas canvas, DesktopIcon icon)
        {
            if (icon.Selected)
            {
                FillRoundedRect(canvas, icon.X, icon.Y, icon.Width, icon.Height, 9, IconSelectionFill);
                DrawRoundedOutline(canvas, icon.X, icon.Y, icon.Width, icon.Height, 9, IconSelectionBorder);
            }

            int iconX = icon.X + 28;
            int iconY = icon.Y + 5;

            if (icon.IsDirectory)
                DrawFolderIcon(canvas, iconX, iconY);
            else
                DrawFileIcon(canvas, iconX, iconY);

            Font font = FontManager.UiFont;
            string displayName = icon.Name;
            int maxTextWidth = icon.Width - 8;

            if (MeasureText(font, displayName) > maxTextWidth)
            {
                while (displayName.Length > 2 && MeasureText(font, displayName + "..") > maxTextWidth)
                    displayName = displayName.Substring(0, displayName.Length - 1);

                displayName += "..";
            }

            int textWidth = MeasureText(font, displayName);
            int textX = icon.X + (icon.Width - textWidth) / 2;
            int textY = icon.Y + 52;

            DrawText(canvas, displayName, font, DesktopTextShadow, textX + 1, textY + 1);
            DrawText(canvas, displayName, font, DesktopTextColor, textX, textY);
        }

        private static void DrawFolderIcon(Canvas canvas, int x, int y)
        {
            FillRoundedRect(canvas, x + 2, y + 8, 44, 30, 5, FolderDarkColor);
            FillRoundedRect(canvas, x + 4, y + 2, 19, 12, 4, FolderLightColor);
            FillRoundedRect(canvas, x, y + 8, 44, 28, 5, FolderColor);
            FillRoundedRect(canvas, x + 2, y + 10, 40, 5, 2, FolderLightColor);
        }

        private static void DrawFileIcon(Canvas canvas, int x, int y)
        {
            FillRoundedRect(canvas, x + 8, y + 3, 34, 39, 4, Color.FromArgb(45, 0, 0, 0));
            FillRoundedRect(canvas, x + 5, y, 34, 39, 4, FileBorderColor);
            FillRoundedRect(canvas, x + 6, y + 1, 32, 37, 3, FileColor);

            canvas.DrawLine(FileLineColor, x + 12, y + 13, x + 32, y + 13);
            canvas.DrawLine(FileLineColor, x + 12, y + 19, x + 32, y + 19);
            canvas.DrawLine(FileLineColor, x + 12, y + 25, x + 29, y + 25);
        }

        private static void DrawSelectionBox(Canvas canvas)
        {
            int x = Math.Min(selectionStartX, selectionEndX);
            int y = Math.Min(selectionStartY, selectionEndY);
            int width = Math.Abs(selectionEndX - selectionStartX);
            int height = Math.Abs(selectionEndY - selectionStartY);

            if (width <= 0 || height <= 0)
                return;

            FillRoundedRect(canvas, x, y, width, height, 3, SelectionBoxFill);
            DrawRoundedOutline(canvas, x, y, width, height, 3, SelectionBoxBorder);
        }

        public static void UpdateMouse(Canvas canvas)
        {
            if (!UserManager.IsLoggedIn)
                return;

            int mouseX = (int)MouseManager.X;
            int mouseY = (int)MouseManager.Y;
            bool leftPressed = MouseManager.LeftButton;
            bool rightPressed = MouseManager.RightButton;

            foreach (var window in WindowManager.GetWindows())
            {
                if (window != null && window.Contains(mouseX, mouseY))
                {
                    wasLeftPressed = leftPressed;
                    wasRightPressed = rightPressed;
                    return;
                }
            }

            if (contextMenuVisible)
            {
                hoveredMenuItem = GetContextMenuItemAt(mouseX, mouseY);

                if (leftPressed && !wasLeftPressed)
                    HandleContextMenuClick(canvas, mouseX, mouseY);

                wasLeftPressed = leftPressed;
                wasRightPressed = rightPressed;
                return;
            }

            if (rightPressed && !wasRightPressed)
            {
                DesktopIcon? clickedIcon = GetIconAt(mouseX, mouseY);
                contextIcon = clickedIcon;

                if (clickedIcon != null)
                {
                    SelectIcon(clickedIcon);
                    WindowManager.ClearFocus();
                }
                else
                {
                    ClearSelection();
                    WindowManager.ClearFocus();
                }

                ShowContextMenu(canvas, mouseX, mouseY);
                wasRightPressed = rightPressed;
                return;
            }

            if (leftPressed && !wasLeftPressed)
            {
                DesktopIcon? icon = GetIconAt(mouseX, mouseY);

                if (icon != null)
                {
                    SelectIcon(icon);
                    WindowManager.ClearFocus();
                }
                else
                {
                    isSelecting = true;
                    selectionStartX = mouseX;
                    selectionStartY = mouseY;
                    selectionEndX = mouseX;
                    selectionEndY = mouseY;
                    ClearSelection();
                    WindowManager.ClearFocus();
                }
            }

            if (leftPressed && isSelecting)
            {
                selectionEndX = mouseX;
                selectionEndY = mouseY;
                UpdateBoxSelection();
            }

            if (!leftPressed && wasLeftPressed)
            {
                if (isSelecting)
                {
                    selectionEndX = mouseX;
                    selectionEndY = mouseY;
                    UpdateBoxSelection();
                    isSelecting = false;
                }
                else
                {
                    DesktopIcon? icon = GetIconAt(mouseX, mouseY);

                    if (icon != null)
                        HandleIconClick(icon);
                }
            }

            wasLeftPressed = leftPressed;
            wasRightPressed = rightPressed;
        }

        private static void HandleIconClick(DesktopIcon icon)
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            bool doubleClick = lastClickedIcon == icon && now - lastClickTime <= DoubleClickMilliseconds;

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

        private static DesktopIcon? GetIconAt(int x, int y)
        {
            for (int i = icons.Count - 1; i >= 0; i--)
            {
                DesktopIcon icon = icons[i];

                if (x >= icon.X && x <= icon.X + icon.Width && y >= icon.Y && y <= icon.Y + icon.Height)
                    return icon;
            }

            return null;
        }

        private static void UpdateBoxSelection()
        {
            int left = Math.Min(selectionStartX, selectionEndX);
            int right = Math.Max(selectionStartX, selectionEndX);
            int top = Math.Min(selectionStartY, selectionEndY);
            int bottom = Math.Max(selectionStartY, selectionEndY);

            foreach (DesktopIcon icon in icons)
            {
                int iconLeft = icon.X;
                int iconRight = icon.X + icon.Width;
                int iconTop = icon.Y;
                int iconBottom = icon.Y + icon.Height;

                icon.Selected = iconRight >= left && iconLeft <= right && iconBottom >= top && iconTop <= bottom;
            }

            selectedIcon = null;

            foreach (DesktopIcon icon in icons)
            {
                if (icon.Selected)
                {
                    selectedIcon = icon;
                    break;
                }
            }
        }

        private static void SelectIcon(DesktopIcon? icon)
        {
            foreach (DesktopIcon item in icons)
                item.Selected = false;

            if (icon != null)
            {
                icon.Selected = true;
                selectedIcon = icon;
            }
            else
            {
                selectedIcon = null;
            }
        }

        private static void ClearSelection()
        {
            foreach (DesktopIcon icon in icons)
                icon.Selected = false;

            selectedIcon = null;
        }

        private static int GetContextMenuWidth()
        {
            Font font = FontManager.UiFont;
            int maxWidth = 0;

            foreach (DesktopMenuItem item in contextMenu)
            {
                int textWidth = MeasureText(font, item.Text);

                if (textWidth > maxWidth)
                    maxWidth = textWidth;
            }

            return maxWidth + ContextMenuPadding * 2;
        }

        private static void ShowContextMenu(Canvas canvas, int x, int y)
        {
            contextMenu.Clear();
            hoveredMenuItem = -1;

            if (contextIcon != null)
            {
                contextMenu.Add(new DesktopMenuItem { Text = "Aç", Y = 0 });
                contextMenu.Add(new DesktopMenuItem { Text = "Sil", Y = ContextMenuItemHeight });
                contextMenu.Add(new DesktopMenuItem { Text = "Yenile", Y = ContextMenuItemHeight * 2 });
                contextMenu.Add(new DesktopMenuItem { Text = "İkonları Sırala", Y = ContextMenuItemHeight * 3 });
            }
            else
            {
                contextMenu.Add(new DesktopMenuItem { Text = "Yeni Klasör", Y = 0 });
                contextMenu.Add(new DesktopMenuItem { Text = "Yeni Dosya", Y = ContextMenuItemHeight });
                contextMenu.Add(new DesktopMenuItem { Text = "Yenile", Y = ContextMenuItemHeight * 2 });
                contextMenu.Add(new DesktopMenuItem { Text = "İkonları Sırala", Y = ContextMenuItemHeight * 3 });
            }

            contextMenuX = x;
            contextMenuY = y;

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;
            int menuWidth = GetContextMenuWidth();
            int menuHeight = contextMenu.Count * ContextMenuItemHeight;

            if (contextMenuX + menuWidth > screenWidth)
                contextMenuX = screenWidth - menuWidth;

            if (contextMenuY + menuHeight > screenHeight)
                contextMenuY = screenHeight - menuHeight;

            if (contextMenuX < 0)
                contextMenuX = 0;

            if (contextMenuY < 0)
                contextMenuY = 0;

            contextMenuVisible = true;
        }

        private static void DrawContextMenu(Canvas canvas)
        {
            int width = GetContextMenuWidth();
            int itemHeight = ContextMenuItemHeight;
            int height = contextMenu.Count * itemHeight;

            FillRoundedRect(canvas, contextMenuX + 2, contextMenuY + 3, width, height, 9, MenuShadow);
            FillRoundedRect(canvas, contextMenuX, contextMenuY, width, height, 9, MenuBackground);
            DrawRoundedOutline(canvas, contextMenuX, contextMenuY, width, height, 9, MenuBorder);

            Font font = FontManager.UiFont;
            int fontHeight = GetFontHeight(font);

            for (int i = 0; i < contextMenu.Count; i++)
            {
                DesktopMenuItem item = contextMenu[i];
                int itemY = contextMenuY + item.Y;
                int textWidth = MeasureText(font, item.Text);
                int textX = contextMenuX + ContextMenuPadding;
                int textY = itemY + (itemHeight - fontHeight) / 2;

                if (i == hoveredMenuItem)
                    FillRoundedRect(canvas, contextMenuX + 4, itemY + 2, width - 8, itemHeight - 4, 6, MenuHover);

                DrawText(canvas, item.Text, font, MenuText, textX, textY);
            }
        }

        private static int GetContextMenuItemAt(int x, int y)
        {
            int width = GetContextMenuWidth();
            int height = contextMenu.Count * ContextMenuItemHeight;

            if (x < contextMenuX || x >= contextMenuX + width || y < contextMenuY || y >= contextMenuY + height)
                return -1;

            int index = (y - contextMenuY) / ContextMenuItemHeight;

            if (index < 0 || index >= contextMenu.Count)
                return -1;

            return index;
        }

        private static void HandleContextMenuClick(Canvas canvas, int x, int y)
        {
            int width = GetContextMenuWidth();
            int height = contextMenu.Count * ContextMenuItemHeight;

            if (x < contextMenuX || x >= contextMenuX + width || y < contextMenuY || y >= contextMenuY + height)
            {
                contextMenuVisible = false;
                contextIcon = null;
                hoveredMenuItem = -1;
                return;
            }

            int index = (y - contextMenuY) / ContextMenuItemHeight;

            if (index < 0 || index >= contextMenu.Count)
                return;

            string action = contextMenu[index].Text;
            contextMenuVisible = false;
            hoveredMenuItem = -1;

            if (action == "Aç")
            {
                if (contextIcon != null)
                    OpenIcon(contextIcon);
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
                                Directory.Delete(contextIcon.Path, true);
                        }
                        else
                        {
                            if (File.Exists(contextIcon.Path))
                                File.Delete(contextIcon.Path);
                        }

                        RefreshIcons();
                    }
                    catch
                    {
                    }
                }
            }
            else if (action == "Yeni Klasör")
            {
                string defaultPath = UserManager.IsLoggedIn ? UserManager.HomeDirectory : "/home";

                var msg = new MessageBox("Yeni Klasör", "Klasör adı girin:", true, "YeniKlasor", (inputName) =>
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
                        catch
                        {
                        }
                    }
                });

                AppManager.Run(msg);
            }
            else if (action == "Yeni Dosya")
            {
                string defaultPath = UserManager.IsLoggedIn ? UserManager.HomeDirectory : "/home";

                var msg = new MessageBox("Yeni Dosya", "Dosya adı girin (örn: not.txt):", true, "yeni.txt", (inputName) =>
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
                        catch
                        {
                        }
                    }
                });

                AppManager.Run(msg);
            }
            else if (action == "Yenile")
            {
                RefreshIcons();
            }
            else if (action == "İkonları Sırala")
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
