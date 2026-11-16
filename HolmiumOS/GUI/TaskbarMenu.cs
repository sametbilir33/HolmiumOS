
using System.Drawing;
using Cosmos.Kernel.System;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI
{
    public static class TaskbarMenu
    {
        private static bool aboutHover;
        private static bool terminalHover;
        private static bool fileManagerHover;
        private static bool calculatorHover;
        private static bool notepadHover;
        private static bool badAppleHover;
        private static bool taskManagerHover;
        private static bool rebootHover;
        private static bool shutdownHover;

        private const int menuWidth = 320;
        private const int menuHeight = 500;
        private const int bannerWidth = 5;

        private const int buttonWidth = 286;
        private const int buttonHeight = 40;
        private const int buttonSpacing = 3;

        private static readonly Color BackgroundColor = Color.FromArgb(250, 249, 248);
        private static readonly Color BorderColor = Color.FromArgb(205, 204, 203);
        private static readonly Color ButtonColor = Color.FromArgb(250, 249, 248);
        private static readonly Color HoverColor = Color.FromArgb(235, 235, 234);
        private static readonly Color PressedColor = Color.FromArgb(222, 221, 220);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);
        private static readonly Color SecondaryTextColor = Color.FromArgb(110, 110, 110);
        private static readonly Color AccentColor = Color.FromArgb(53, 132, 228);
        private static readonly Color SeparatorColor = Color.FromArgb(220, 219, 218);

        private static int GetMenuX(Canvas canvas)
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

            radius = System.Math.Min(radius, System.Math.Min(width / 2, height / 2));

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
            int textWidth = MeasureText(text, font);

            DrawText(canvas, text, font, TextColor, x + 14, y + (buttonHeight - textHeight) / 2);

            int indicatorX = x + buttonWidth - 20;
            int indicatorY = y + buttonHeight / 2;

            if (hover)
                canvas.DrawFilledCircle(AccentColor, indicatorX, indicatorY, 3);
        }

        private static void DrawSeparator(Canvas canvas, int x, int y, int width)
        {
            canvas.DrawLine(SeparatorColor, x, y, x + width, y);
        }

        public static void Draw(Canvas canvas)
        {
            if (canvas == null)
                return;

            int x = GetMenuX(canvas);
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

            DrawButton(canvas, "About", contentX, buttonY, aboutHover, font);
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Terminal", contentX, buttonY, terminalHover, font);
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "File Manager", contentX, buttonY, fileManagerHover, font);
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Calculator", contentX, buttonY, calculatorHover, font);
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Notepad", contentX, buttonY, notepadHover, font);
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Bad Apple!!", contentX, buttonY, badAppleHover, font);
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Task Manager", contentX, buttonY, taskManagerHover, font);
            buttonY += buttonHeight + 12;

            DrawSeparator(canvas, contentX, buttonY, buttonWidth);
            buttonY += 10;

            DrawButton(canvas, "Reboot", contentX, buttonY, rebootHover, font);
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Shutdown", contentX, buttonY, shutdownHover, font);
        }

        public static bool IsInside(int mouseX, int mouseY)
        {
            int x = GetMenuX(Init.canvas);
            int y = GetMenuY(Init.canvas);

            return mouseX >= x && mouseX <= x + menuWidth && mouseY >= y && mouseY <= y + menuHeight;
        }

        public static void UpdateHover(int mouseX, int mouseY)
        {
            int x = GetMenuX(Init.canvas);
            int y = GetMenuY(Init.canvas);

            int contentX = x + 16;
            int buttonY = y + 68;

            aboutHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
            buttonY += buttonHeight + buttonSpacing;

            terminalHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
            buttonY += buttonHeight + buttonSpacing;

            fileManagerHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
            buttonY += buttonHeight + buttonSpacing;

            calculatorHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
            buttonY += buttonHeight + buttonSpacing;

            notepadHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
            buttonY += buttonHeight + buttonSpacing;

            badAppleHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
            buttonY += buttonHeight + buttonSpacing;

            taskManagerHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
            buttonY += buttonHeight + 12 + 10;

            rebootHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
            buttonY += buttonHeight + buttonSpacing;

            shutdownHover = IsButtonInside(mouseX, mouseY, contentX, buttonY);
        }

        private static bool IsButtonInside(int mouseX, int mouseY, int buttonX, int buttonY)
        {
            return mouseX >= buttonX && mouseX <= buttonX + buttonWidth && mouseY >= buttonY && mouseY <= buttonY + buttonHeight;
        }

        public static void Click(int mouseX, int mouseY)
        {
            if (aboutHover)
            {
                AppManager.Run<Apps.About>(70, 70);
                Taskbar.MenuOpen = false;
            }
            else if (terminalHover)
            {
                AppManager.Run<Apps.Terminal>(50, 50);
                Taskbar.MenuOpen = false;
            }
            else if (fileManagerHover)
            {
                var fileManager = new Apps.FileManager(UserManager.HomeDirectory);
                AppManager.Run(fileManager);
                Taskbar.MenuOpen = false;
            }
            else if (calculatorHover)
            {
                AppManager.Run<Apps.Calculator>(50, 50);
                Taskbar.MenuOpen = false;
            }
            else if (notepadHover)
            {
                var notepad = new Apps.Notepad(null);
                AppManager.Run(notepad);
                Taskbar.MenuOpen = false;
            }
            else if (badAppleHover)
            {
                AppManager.Run<Apps.BadApple>(40, 40);
                Taskbar.MenuOpen = false;
            }
            else if (taskManagerHover)
            {
                AppManager.Run<Apps.TaskManager>(80, 60);
                Taskbar.MenuOpen = false;
            }
            else if (rebootHover)
            {
                Taskbar.MenuOpen = false;
                Power.Reboot();
            }
            else if (shutdownHover)
            {
                Taskbar.MenuOpen = false;
                Power.Shutdown();
            }
        }
    }
}
