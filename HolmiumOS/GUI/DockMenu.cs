using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI
{
    public static class DockMenu
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

        private const int menuWidth = 230;
        private const int menuHeight = 380;

        private const int buttonWidth = 210;
        private const int buttonHeight = 30;
        private const int buttonSpacing = 5;

        private static int GetMenuX(Canvas canvas)
        {
            int startCenter = Dock.StartX(canvas) + 35;
            return startCenter - (menuWidth / 2);
        }

        private static int GetMenuY(Canvas canvas)
        {
            return (int)canvas.Mode.Height - Dock.Height - menuHeight - 5;
        }

        private static void DrawCenteredString(Canvas canvas, string text, int x, int y, int width, int height, Color color)
        {
            int charWidth = 8;
            int charHeight = 16;
            int textWidth = text.Length * charWidth;

            int textX = x + ((width - textWidth) / 2);
            int textY = y + ((height - charHeight) / 2);

            canvas.DrawString(text, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, color, textX, textY);
        }

        private static void DrawButton(Canvas canvas, string text, int x, int y, bool hover, Color normalColor, Color hoverColor)
        {
            Color color = hover ? hoverColor : normalColor;

            canvas.DrawFilledRectangle(color, x, y, buttonWidth, buttonHeight);
            DrawCenteredString(canvas, text, x, y, buttonWidth, buttonHeight, Color.White);
        }

        public static void Draw(Canvas canvas)
        {
            int x = GetMenuX(canvas);
            int y = GetMenuY(canvas);

            canvas.DrawFilledRectangle(Color.FromArgb(25, 25, 30), x, y, menuWidth, menuHeight);
            canvas.DrawFilledRectangle(Color.FromArgb(45, 45, 55), x, y, menuWidth, 45);

            canvas.DrawString("HolmiumOS", Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, x + 15, y + 8);
            canvas.DrawString("Uygulamalar", Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.LightGray, x + 15, y + 25);

            int buttonY = y + 55;

            DrawButton(canvas, "About", x + 10, buttonY, aboutHover, Color.FromArgb(55, 55, 65), Color.FromArgb(80, 110, 180));
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Terminal", x + 10, buttonY, terminalHover, Color.FromArgb(55, 55, 65), Color.FromArgb(80, 110, 180));
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "File Manager", x + 10, buttonY, fileManagerHover, Color.FromArgb(55, 55, 65), Color.FromArgb(80, 110, 180));
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Calculator", x + 10, buttonY, calculatorHover, Color.FromArgb(55, 55, 65), Color.FromArgb(80, 110, 180));
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Notepad", x + 10, buttonY, notepadHover, Color.FromArgb(55, 55, 65), Color.FromArgb(80, 110, 180));
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Bad Apple!!", x + 10, buttonY, badAppleHover, Color.FromArgb(55, 55, 65), Color.FromArgb(80, 110, 180));
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Task Manager", x + 10, buttonY, taskManagerHover, Color.FromArgb(55, 55, 65), Color.FromArgb(80, 110, 180));
            buttonY += buttonHeight + buttonSpacing + 5;

            DrawButton(canvas, "Reboot", x + 10, buttonY, rebootHover, Color.FromArgb(75, 65, 55), Color.FromArgb(180, 120, 60));
            buttonY += buttonHeight + buttonSpacing;

            DrawButton(canvas, "Shutdown", x + 10, buttonY, shutdownHover, Color.FromArgb(75, 50, 50), Color.FromArgb(180, 70, 70));
        }

        public static bool IsInside(int mouseX, int mouseY)
        {
            int x = Dock.StartX(Init.canvas);
            int y = GetMenuY(Init.canvas);

            return mouseX >= x && mouseX <= x + menuWidth && mouseY >= y && mouseY <= y + menuHeight;
        }

        public static void UpdateHover(int mouseX, int mouseY)
        {
            int y = GetMenuY(Init.canvas);

            aboutHover = IsButtonInside(mouseX, mouseY, y, 55);
            terminalHover = IsButtonInside(mouseX, mouseY, y, 90);
            fileManagerHover = IsButtonInside(mouseX, mouseY, y, 125);
            calculatorHover = IsButtonInside(mouseX, mouseY, y, 160);
            notepadHover = IsButtonInside(mouseX, mouseY, y, 195);
            badAppleHover = IsButtonInside(mouseX, mouseY, y, 230);
            taskManagerHover = IsButtonInside(mouseX, mouseY, y, 265);
            rebootHover = IsButtonInside(mouseX, mouseY, y, 305);
            shutdownHover = IsButtonInside(mouseX, mouseY, y, 340);
        }

        private static bool IsButtonInside(int mouseX, int mouseY, int menuY, int buttonY)
        {
            int x = Dock.StartX(Init.canvas) + 10;
            int y = menuY + buttonY;

            return mouseX >= x && mouseX <= x + buttonWidth && mouseY >= y && mouseY <= y + buttonHeight;
        }

        public static void Click(int mouseX, int mouseY)
        {
            if (aboutHover)
            {
                AppManager.Run<Apps.About>(70, 70);
                Dock.MenuOpen = false;
            }
            else if (terminalHover)
            {
                AppManager.Run<Apps.Terminal>(50, 50);
                Dock.MenuOpen = false;
            }
            else if (fileManagerHover)
            {
                var fileManager = new Apps.FileManager(UserManager.HomeDirectory);
                AppManager.Run(fileManager);
                Dock.MenuOpen = false;
            }
            else if (calculatorHover)
            {
                AppManager.Run<Apps.Calculator>(50, 50);
                Dock.MenuOpen = false;
            }
            else if (notepadHover)
            {
                AppManager.Run<Apps.Notepad>(70, 70);
                Dock.MenuOpen = false;
            }
            else if (badAppleHover)
            {
                AppManager.Run<Apps.BadApple>(40, 40);
                Dock.MenuOpen = false;
            }
            else if (taskManagerHover)
            {
                AppManager.Run<Apps.TaskManager>(80, 60);
                Dock.MenuOpen = false;
            }
            else if (rebootHover)
            {
                Dock.MenuOpen = false;
                Power.Reboot();
            }
            else if (shutdownHover)
            {
                Dock.MenuOpen = false;
                Power.Shutdown();
            }
        }
    }
}