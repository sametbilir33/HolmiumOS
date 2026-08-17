using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.HAL;

namespace HolmiumOS.GUI
{
    public static class StatusBar
    {
        private const int height = 32;

        public static int Height => height;

        public static void Draw(Canvas canvas)
        {
            int screenWidth = (int)canvas.Mode.Width;

            canvas.DrawFilledRectangle(Color.FromArgb(35, 35, 40), 0, 0, screenWidth, height);

            canvas.DrawString("HolmiumOS", Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, 12, 8);

            string timeString = $"{RTC.Hour:D2}:{RTC.Minute:D2}";
            int timeWidth = timeString.Length * 8;
            int timeX = screenWidth - timeWidth - 15;

            bool hovered = MouseManager.X >= timeX && MouseManager.X <= timeX + timeWidth &&
                           MouseManager.Y >= 0 && MouseManager.Y < height;

            if (hovered)
                canvas.DrawFilledRectangle(Color.FromArgb(50, 50, 58), timeX - 5, 0, timeWidth + 10, height);

            canvas.DrawString(timeString, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, timeX, 8);
        }

        public static bool ContainsClock(int x, int y, Canvas canvas)
        {
            string timeString = $"{RTC.Hour:D2}:{RTC.Minute:D2}";
            int timeWidth = timeString.Length * 8;
            int timeX = (int)canvas.Mode.Width - timeWidth - 15;

            return x >= timeX && x <= timeX + timeWidth && y >= 0 && y < height;
        }
    }
}