using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class TaskbarMenu
    {
        private static bool terminalHover;
        private static bool fileManagerHover;
        private static bool calculatorHover;
        private static bool rebootHover;
        private static bool shutdownHover;

        public static void Draw(Canvas canvas)
        {
            int x = 10;
            int y = (int)canvas.Mode.Height - 310;

            canvas.DrawFilledRectangle(
                Color.FromArgb(30, 30, 30),
                x, y, 200, 260
            );

            canvas.DrawFilledRectangle(
                terminalHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 10, 180, 35
            );
            canvas.DrawString(
                "Terminal",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White, x + 60, y + 22
            );

            canvas.DrawFilledRectangle(
                fileManagerHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 60, 180, 35
            );
            canvas.DrawString(
                "File Manager",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White, x + 45, y + 72
            );

            canvas.DrawFilledRectangle(
                calculatorHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 110, 180, 35
            );
            canvas.DrawString(
                "Calculator",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White, x + 50, y + 122
            );

            canvas.DrawFilledRectangle(
                rebootHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 160, 180, 35
            );
            canvas.DrawString(
                "Reboot",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White, x + 60, y + 172
            );

            canvas.DrawFilledRectangle(
                shutdownHover ? Color.FromArgb(120, 120, 120) : Color.FromArgb(80, 80, 80),
                x + 10, y + 210, 180, 35
            );
            canvas.DrawString(
                "Shutdown",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White, x + 50, y + 222
            );
        }

        public static bool IsInside(int mouseX, int mouseY)
        {
            int x = 10;
            int y = (int)MouseManager.ScreenHeight - 310;

            return mouseX >= x &&
                   mouseX <= x + 200 &&
                   mouseY >= y &&
                   mouseY <= y + 260;
        }

        public static void UpdateHover(int mouseX, int mouseY)
        {
            int x = 10;
            int y = (int)MouseManager.ScreenHeight - 310;

            terminalHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 10 && mouseY <= y + 45;

            fileManagerHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 60 && mouseY <= y + 95;

            calculatorHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 110 && mouseY <= y + 145;

            rebootHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 160 && mouseY <= y + 195;

            shutdownHover = mouseX >= x + 10 && mouseX <= x + 190 && mouseY >= y + 210 && mouseY <= y + 245;
        }

        public static void Click(int mouseX, int mouseY)
        {
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