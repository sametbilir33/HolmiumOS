using System.Drawing;
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

            canvas.DrawString(timeString, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, timeX, 8);
        }
    }
}