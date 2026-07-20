using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class TaskbarMenu
    {
        private static bool rebootHover;
        private static bool shutdownHover;


        public static void Draw(Canvas canvas)
        {
            int x = 10;
            int y = (int)canvas.Mode.Height - 170;


            canvas.DrawFilledRectangle(
                Color.FromArgb(30, 30, 30),
                x,
                y,
                200,
                120
            );


            canvas.DrawFilledRectangle(
                rebootHover
                    ? Color.FromArgb(120, 120, 120)
                    : Color.FromArgb(80, 80, 80),
                x + 10,
                y + 10,
                180,
                35
            );


            canvas.DrawString(
                "Reboot",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White,
                x + 60,
                y + 22
            );


            canvas.DrawFilledRectangle(
                shutdownHover
                    ? Color.FromArgb(120, 120, 120)
                    : Color.FromArgb(80, 80, 80),
                x + 10,
                y + 60,
                180,
                35
            );


            canvas.DrawString(
                "Shutdown",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White,
                x + 50,
                y + 72
            );
        }


        public static bool IsInside(int mouseX, int mouseY)
        {
            int x = 10;
            int y = (int)MouseManager.ScreenHeight - 170;


            return mouseX >= x &&
                   mouseX <= x + 200 &&
                   mouseY >= y &&
                   mouseY <= y + 120;
        }


        public static void UpdateHover(int mouseX, int mouseY)
        {
            int x = 10;
            int y = (int)MouseManager.ScreenHeight - 170;


            rebootHover =
                mouseX >= x + 10 &&
                mouseX <= x + 190 &&
                mouseY >= y + 10 &&
                mouseY <= y + 45;


            shutdownHover =
                mouseX >= x + 10 &&
                mouseX <= x + 190 &&
                mouseY >= y + 60 &&
                mouseY <= y + 95;
        }


        public static void Click(int mouseX, int mouseY)
        {
            int x = 10;
            int y = (int)MouseManager.ScreenHeight - 170;


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