using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

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
        private static bool rebootHover;
        private static bool shutdownHover;

        private static void DrawCenteredString(Canvas canvas, string text, int btnX, int btnY, int btnWidth, int btnHeight)
        {
            int charWidth = 8;
            int charHeight = 16;

            int textWidth = text.Length * charWidth;

            int textX = btnX + ((btnWidth - textWidth) / 2);
            int textY = btnY + ((btnHeight - charHeight) / 2);

            canvas.DrawString(
                text,
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White, textX, textY
            );
        }

        public static void Draw(Canvas canvas)
        {
            int x = 10;
            int y = (int)canvas.Mode.Height - 330;

            canvas.DrawFilledRectangle(
                Color.FromArgb(30, 30, 30),
                x, y, 200, 320
            );


            canvas.DrawFilledRectangle(
                aboutHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 10, 180, 28
            );
            DrawCenteredString(canvas, "About", x + 10, y + 10, 180, 28);

            canvas.DrawFilledRectangle(
                terminalHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 44, 180, 28
            );
            DrawCenteredString(canvas, "Terminal", x + 10, y + 44, 180, 28);

            canvas.DrawFilledRectangle(
                fileManagerHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 78, 180, 28
            );
            DrawCenteredString(canvas, "File Manager", x + 10, y + 78, 180, 28);

            canvas.DrawFilledRectangle(
                calculatorHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 112, 180, 28
            );
            DrawCenteredString(canvas, "Calculator", x + 10, y + 112, 180, 28);

            canvas.DrawFilledRectangle(
                notepadHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 146, 180, 28
            );
            DrawCenteredString(canvas, "Notepad", x + 10, y + 146, 180, 28);

            canvas.DrawFilledRectangle(
                badAppleHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 180, 180, 28
            );
            DrawCenteredString(canvas, "Bad Apple!!", x + 10, y + 180, 180, 28);

            canvas.DrawFilledRectangle(
                rebootHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 214, 180, 28
            );
            DrawCenteredString(canvas, "Reboot", x + 10, y + 214, 180, 28);

            canvas.DrawFilledRectangle(
                shutdownHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 248, 180, 28
            );
            DrawCenteredString(canvas, "Shutdown", x + 10, y + 248, 180, 28);
        }

        public static bool IsInside(int mouseX, int mouseY)
        {
            int x = 10;
            int y = (int)MouseManager.ScreenHeight - 330;

            return mouseX >= x &&
                   mouseX <= x + 200 &&
                   mouseY >= y &&
                   mouseY <= y + 320;
        }

        public static void UpdateHover(int mouseX, int mouseY)
        {
            int x = 10;
            int y = (int)MouseManager.ScreenHeight - 330;

            aboutHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 10 && mouseY <= y + 38;
            terminalHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 44 && mouseY <= y + 72;
            fileManagerHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 78 && mouseY <= y + 106;
            calculatorHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 112 && mouseY <= y + 140;
            notepadHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 146 && mouseY <= y + 174;
            badAppleHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 180 && mouseY <= y + 208;
            rebootHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 214 && mouseY <= y + 242;
            shutdownHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 248 && mouseY <= y + 276;
        }

        public static void Click(int mouseX, int mouseY)
        {
            if (aboutHover)
            {
                AppManager.Run<Apps.About>(70, 70);
                Taskbar.MenuOpen = false;
            }

            if (terminalHover)
            {
                AppManager.Run<Apps.Terminal>(50, 50);
                Taskbar.MenuOpen = false;
            }

            if (fileManagerHover)
            {
                AppManager.Run<Apps.FileManager>(60, 60);
                Taskbar.MenuOpen = false;
            }

            if (calculatorHover)
            {
                AppManager.Run<Apps.Calculator>(50, 50);
                Taskbar.MenuOpen = false;
            }

            if (notepadHover)
            {
                AppManager.Run<Apps.Notepad>(70, 70);
                Taskbar.MenuOpen = false;
            }

            if (badAppleHover)
            {
                AppManager.Run<Apps.BadApple>(40, 40);
                Taskbar.MenuOpen = false;
            }

            if (rebootHover)
            {
                Power.Reboot();
            }

            if (shutdownHover)
            {
                Power.Shutdown();
            }
        }
    }
}