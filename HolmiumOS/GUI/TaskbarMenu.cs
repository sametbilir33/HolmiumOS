using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
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

        private const int MenuX = 10;
        private const int MenuWidth = 230;
        private const int MenuHeight = 380;

        private const int ButtonX = 20;
        private const int ButtonWidth = 210;
        private const int ButtonHeight = 30;
        private const int ButtonSpacing = 5;

        private static void DrawCenteredString(Canvas canvas, string text, int x, int y, int width, int height, Color color)
        {
            int charWidth = 8;
            int charHeight = 16;
            int textWidth = text.Length * charWidth;

            int textX = x + ((width - textWidth) / 2);
            int textY = y + ((height - charHeight) / 2);

            canvas.DrawString(
                text,
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                color,
                textX,
                textY
            );
        }

        private static void DrawButton(Canvas canvas, string text, int x, int y, bool hover, Color normalColor, Color hoverColor)
        {
            Color color = hover ? hoverColor : normalColor;

            canvas.DrawFilledRectangle(
                color,
                x,
                y,
                ButtonWidth,
                ButtonHeight
            );

            DrawCenteredString(
                canvas,
                text,
                x,
                y,
                ButtonWidth,
                ButtonHeight,
                Color.White
            );
        }

        public static void Draw(Canvas canvas)
        {
            int y = (int)canvas.Mode.Height - Taskbar.Height - MenuHeight;

            canvas.DrawFilledRectangle(
                Color.FromArgb(25, 25, 30),
                MenuX,
                y,
                MenuWidth,
                MenuHeight
            );

            canvas.DrawFilledRectangle(
                Color.FromArgb(45, 45, 55),
                MenuX,
                y,
                MenuWidth,
                45
            );

            canvas.DrawString(
                "HolmiumOS",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White,
                MenuX + 15,
                y + 8
            );

            canvas.DrawString(
                "Uygulamalar",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.LightGray,
                MenuX + 15,
                y + 25
            );

            int buttonY = y + 55;

            DrawButton(
                canvas,
                "About",
                MenuX + 10,
                buttonY,
                aboutHover,
                Color.FromArgb(55, 55, 65),
                Color.FromArgb(80, 110, 180)
            );

            buttonY += ButtonHeight + ButtonSpacing;

            DrawButton(
                canvas,
                "Terminal",
                MenuX + 10,
                buttonY,
                terminalHover,
                Color.FromArgb(55, 55, 65),
                Color.FromArgb(80, 110, 180)
            );

            buttonY += ButtonHeight + ButtonSpacing;

            DrawButton(
                canvas,
                "File Manager",
                MenuX + 10,
                buttonY,
                fileManagerHover,
                Color.FromArgb(55, 55, 65),
                Color.FromArgb(80, 110, 180)
            );

            buttonY += ButtonHeight + ButtonSpacing;

            DrawButton(
                canvas,
                "Calculator",
                MenuX + 10,
                buttonY,
                calculatorHover,
                Color.FromArgb(55, 55, 65),
                Color.FromArgb(80, 110, 180)
            );

            buttonY += ButtonHeight + ButtonSpacing;

            DrawButton(
                canvas,
                "Notepad",
                MenuX + 10,
                buttonY,
                notepadHover,
                Color.FromArgb(55, 55, 65),
                Color.FromArgb(80, 110, 180)
            );

            buttonY += ButtonHeight + ButtonSpacing;

            DrawButton(
                canvas,
                "Bad Apple!!",
                MenuX + 10,
                buttonY,
                badAppleHover,
                Color.FromArgb(55, 55, 65),
                Color.FromArgb(80, 110, 180)
            );

            buttonY += ButtonHeight + ButtonSpacing;

            DrawButton(
                canvas,
                "Task Manager",
                MenuX + 10,
                buttonY,
                taskManagerHover,
                Color.FromArgb(55, 55, 65),
                Color.FromArgb(80, 110, 180)
            );

            buttonY += ButtonHeight + ButtonSpacing + 5;

            DrawButton(
                canvas,
                "Reboot",
                MenuX + 10,
                buttonY,
                rebootHover,
                Color.FromArgb(75, 65, 55),
                Color.FromArgb(180, 120, 60)
            );

            buttonY += ButtonHeight + ButtonSpacing;

            DrawButton(
                canvas,
                "Shutdown",
                MenuX + 10,
                buttonY,
                shutdownHover,
                Color.FromArgb(75, 50, 50),
                Color.FromArgb(180, 70, 70)
            );
        }
        public static bool IsInside(int mouseX, int mouseY)
        {
            int y = (int)MouseManager.ScreenHeight - Taskbar.Height - MenuHeight;

            return mouseX >= MenuX &&
                   mouseX <= MenuX + MenuWidth &&
                   mouseY >= y &&
                   mouseY <= y + MenuHeight;
        }

        public static void UpdateHover(int mouseX, int mouseY)
        {
            int y = (int)MouseManager.ScreenHeight - Taskbar.Height - MenuHeight;

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
            int x = MenuX + 10;
            int y = menuY + buttonY;

            return mouseX >= x &&
                   mouseX <= x + ButtonWidth &&
                   mouseY >= y &&
                   mouseY <= y + ButtonHeight;
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
                AppManager.Run<Apps.Notepad>(70, 70);
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
