using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
using HolmiumOS.GUI.Apps; // Terminal uygulamasını tetikleyebilmek için eklendi

namespace HolmiumOS.GUI
{
    public static class Taskbar
    {
        public static bool MenuOpen;

        // 1. Taskbar yüksekliğini uzattık (40 -> 50)
        private static int height = 50;
        private static bool lastPressed;

        // Start Butonu Ayarları
        private static int startX = 10;
        private static int startWidth = 80;
        private static int startHeight = 36; // Yüksekliğe göre büyütüldü
        private static bool startHover;

        public static int Height => height;

        public static void Draw(Canvas canvas)
        {
            int y = (int)canvas.Mode.Height - height;

            // Arka Plan (Bar)
            canvas.DrawFilledRectangle(
                Color.FromArgb(40, 40, 40),
                0,
                y,
                (int)canvas.Mode.Width,
                height
            );

            // --- START BUTONU ÇİZİMİ ---
            canvas.DrawFilledRectangle(
                startHover
                    ? Color.FromArgb(100, 100, 100)
                    : Color.FromArgb(70, 70, 70),
                startX,
                y + 7, // Dikeyde ortalandı
                startWidth,
                startHeight
            );

            canvas.DrawString(
                "Start",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White,
                startX + 22,
                y + 18
            );

            if (MenuOpen)
            {
                TaskbarMenu.Draw(canvas);
            }
        }

        public static void UpdateMouse(Canvas canvas)
        {
            int x = (int)MouseManager.X;
            int y = (int)MouseManager.Y;

            int taskbarY = (int)canvas.Mode.Height - height;

            // Start Butonu Hover Kontrolü
            startHover =
                x >= startX &&
                x <= startX + startWidth &&
                y >= taskbarY + 7 &&
                y <= taskbarY + 7 + startHeight;

            if (MenuOpen)
            {
                TaskbarMenu.UpdateHover(x, y);
            }

            bool pressed = (MouseManager.MouseState & MouseState.Left) != 0;

            if (pressed && !lastPressed)
            {
                // Start butonuna tıklanma kontrolü
                if (startHover)
                {
                    MenuOpen = !MenuOpen;
                    Draw(canvas);
                    canvas.Display();
                }
                else if (MenuOpen)
                {
                    if (!TaskbarMenu.IsInside(x, y))
                    {
                        MenuOpen = false;
                        Draw(canvas);
                        canvas.Display();
                    }
                    else
                    {
                        TaskbarMenu.Click(x, y);
                    }
                }
            }

            lastPressed = pressed;
        }
    }
}