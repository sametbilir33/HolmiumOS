
using System;
using System.Drawing;
using Cosmos.Kernel.System;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI
{
    public static class TaskbarMenu
    {
        private const int menuWidth = 320;
        private const int menuHeight = 500;
        private const int bannerWidth = 5;

        private const int buttonWidth = 286;
        private const int buttonHeight = 40;
        private const int buttonSpacing = 3;

        private static int hoveredIndex = -1;

        private static readonly Color BackgroundColor = Color.FromArgb(250, 249, 248);
        private static readonly Color BorderColor = Color.FromArgb(205, 204, 203);
        private static readonly Color ButtonColor = Color.FromArgb(250, 249, 248);
        private static readonly Color HoverColor = Color.FromArgb(235, 235, 234);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);
        private static readonly Color SecondaryTextColor = Color.FromArgb(110, 110, 110);
        private static readonly Color AccentColor = Color.FromArgb(53, 132, 228);
        private static readonly Color SeparatorColor = Color.FromArgb(220, 219, 218);

        private class MenuItem
        {
            public string Text;
            public Action Execute;
            public bool SeparatorBefore;

            public MenuItem(string text, Action execute, bool separatorBefore = false)
            {
                Text = text;
                Execute = execute;
                SeparatorBefore = separatorBefore;
            }
        }

        // Yeni uygulamaları buraya ekle.
        private static readonly MenuItem[] Items =
        {
            new MenuItem("About", LaunchAbout),
            new MenuItem("Terminal", LaunchTerminal),
            new MenuItem("File Manager", LaunchFileManager),
            new MenuItem("Calculator", LaunchCalculator),
            new MenuItem("Notepad", LaunchNotepad),
            new MenuItem("Bad Apple!!", LaunchBadApple),
            new MenuItem("Task Manager", LaunchTaskManager),

            new MenuItem("Reboot", Reboot, true),
            new MenuItem("Shutdown", Shutdown)
        };

        private static void LaunchAbout() => AppManager.Run<Apps.About>(70, 70);
        private static void LaunchTerminal() => AppManager.Run<Apps.Terminal>(50, 50);

        private static void LaunchFileManager()
        {
            var app = new Apps.FileManager(UserManager.HomeDirectory);
            AppManager.Run(app);
        }

        private static void LaunchCalculator() => AppManager.Run<Apps.Calculator>(50, 50);

        private static void LaunchNotepad()
        {
            var app = new Apps.Notepad(null);
            AppManager.Run(app);
        }

        private static void LaunchBadApple() => AppManager.Run<Apps.BadApple>(40, 40);
        private static void LaunchTaskManager() => AppManager.Run<Apps.TaskManager>(80, 60);

        private static void Reboot() => Power.Reboot();
        private static void Shutdown() => Power.Shutdown();

        private static int GetMenuX()
        {
            return 8;
        }

        private static int GetMenuY(Canvas canvas)
        {
            return (int)canvas.Mode.Height - Taskbar.Height - menuHeight - 8;
        }

        private static int MeasureText(string text, Font font)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.MeasureString(text ?? "", FontManager.UiFontSize);

            return font.MeasureString(text ?? "");
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
                canvas.DrawString(text ?? "", trueTypeFont, FontManager.UiFontSize, color, x, y);
            else
                canvas.DrawString(text ?? "", font, color, x, y);
        }

        private static void DrawRoundedRect(Canvas canvas, Color color, int x, int y, int width, int height, int radius)
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

        private static void DrawRoundedOutline(Canvas canvas, Color color, int x, int y, int width, int height, int radius)
        {
            if (width <= 0 || height <= 0)
                return;

            radius = Math.Min(radius, Math.Min(width / 2, height / 2));

            if (radius <= 0)
            {
                canvas.DrawLine(color, x, y, x + width - 1, y);
                canvas.DrawLine(color, x, y + height - 1, x + width - 1, y + height - 1);
                canvas.DrawLine(color, x, y, x, y + height - 1);
                canvas.DrawLine(color, x + width - 1, y, x + width - 1, y + height - 1);
                return;
            }

            canvas.DrawLine(color, x + radius, y, x + width - radius - 1, y);
            canvas.DrawLine(color, x + radius, y + height - 1, x + width - radius - 1, y + height - 1);
            canvas.DrawLine(color, x, y + radius, x, y + height - radius - 1);
            canvas.DrawLine(color, x + width - 1, y + radius, x + width - 1, y + height - radius - 1);

            canvas.DrawArc(color, x + radius, y + radius, radius, 180, 270);
            canvas.DrawArc(color, x + width - radius - 1, y + radius, radius, 270, 360);
            canvas.DrawArc(color, x + radius, y + height - radius - 1, radius, 90, 180);
            canvas.DrawArc(color, x + width - radius - 1, y + height - radius - 1, radius, 0, 90);
        }

        private static void DrawButton(Canvas canvas, string text, int x, int y, bool hover, Font font)
        {
            Color background = hover ? HoverColor : ButtonColor;

            DrawRoundedRect(canvas, background, x, y, buttonWidth, buttonHeight, 7);

            if (hover)
                DrawRoundedOutline(canvas, BorderColor, x, y, buttonWidth, buttonHeight, 7);

            int textHeight = GetFontHeight(font);
            DrawText(canvas, text, font, TextColor, x + 14, y + (buttonHeight - textHeight) / 2);

            if (hover)
                canvas.DrawFilledCircle(AccentColor, x + buttonWidth - 20, y + buttonHeight / 2, 3);
        }

        private static void DrawSeparator(Canvas canvas, int x, int y, int width)
        {
            canvas.DrawLine(SeparatorColor, x, y, x + width, y);
        }

        public static void Draw(Canvas canvas)
        {
            if (canvas == null)
                return;

            int x = GetMenuX();
            int y = GetMenuY(canvas);

            DrawRoundedRect(canvas, BackgroundColor, x, y, menuWidth, menuHeight, 12);
            DrawRoundedOutline(canvas, BorderColor, x, y, menuWidth, menuHeight, 12);

            Font font = FontManager.UiFont;
            int fontHeight = GetFontHeight(font);

            DrawRoundedRect(canvas, AccentColor, x + 16, y + 16, bannerWidth, 28, 2);

            DrawText(canvas, "HolmiumOS", font, TextColor, x + 30, y + 16);
            DrawText(canvas, "Uygulamalar", font, SecondaryTextColor, x + 30, y + 16 + fontHeight + 2);

            int contentX = x + 16;
            int buttonY = y + 68;

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].SeparatorBefore)
                {
                    buttonY += 9;
                    DrawSeparator(canvas, contentX, buttonY, buttonWidth);
                    buttonY += 10;
                }

                DrawButton(canvas, Items[i].Text, contentX, buttonY, hoveredIndex == i, font);
                buttonY += buttonHeight + buttonSpacing;
            }
        }

        public static bool IsInside(int mouseX, int mouseY)
        {
            if (Init.canvas == null)
                return false;

            int x = GetMenuX();
            int y = GetMenuY(Init.canvas);

            return mouseX >= x && mouseX < x + menuWidth && mouseY >= y && mouseY < y + menuHeight;
        }

        public static void UpdateHover(int mouseX, int mouseY)
        {
            if (Init.canvas == null)
            {
                hoveredIndex = -1;
                return;
            }

            int x = GetMenuX() + 16;
            int buttonY = GetMenuY(Init.canvas) + 68;

            hoveredIndex = -1;

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].SeparatorBefore)
                {
                    buttonY += 9 + 10;
                }

                if (IsButtonInside(mouseX, mouseY, x, buttonY))
                    hoveredIndex = i;

                buttonY += buttonHeight + buttonSpacing;
            }
        }

        private static bool IsButtonInside(int mouseX, int mouseY, int buttonX, int buttonY)
        {
            return mouseX >= buttonX && mouseX < buttonX + buttonWidth && mouseY >= buttonY && mouseY < buttonY + buttonHeight;
        }

        public static void Click(int mouseX, int mouseY)
        {
            if (hoveredIndex < 0 || hoveredIndex >= Items.Length)
                return;

            if (!IsButtonInside(mouseX, mouseY, GetMenuX() + 16, GetItemY(hoveredIndex)))
                return;

            Action execute = Items[hoveredIndex].Execute;
            hoveredIndex = -1;
            Taskbar.MenuOpen = false;

            execute?.Invoke();
        }

        private static int GetItemY(int index)
        {
            int buttonY = GetMenuY(Init.canvas) + 68;

            for (int i = 0; i < index; i++)
            {
                buttonY += buttonHeight + buttonSpacing;

                if (Items[i + 1].SeparatorBefore)
                    buttonY += 9 + 10;
            }

            return buttonY;
        }
    }
}
