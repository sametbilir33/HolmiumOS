using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class Taskbar
    {
        public static bool MenuOpen;

        private static int height = 40;
        private static bool lastPressed;

        private static int startX = 10;
        private static int startWidth = 80;
        private static int startHeight = 30;

        private static bool startHover;

        public static int Height => height;


        public static void Draw(Canvas canvas)
        {
            int y = (int)canvas.Mode.Height - height;


            canvas.DrawFilledRectangle(
                Color.FromArgb(40, 40, 40),
                0,
                y,
                (int)canvas.Mode.Width,
                height
            );


            canvas.DrawFilledRectangle(
                startHover
                    ? Color.FromArgb(100, 100, 100)
                    : Color.FromArgb(70, 70, 70),
                startX,
                y + 5,
                startWidth,
                startHeight
            );


            canvas.DrawString(
                "Start",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White,
                25,
                y + 15
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


            int taskbarY =
                (int)canvas.Mode.Height - height;


            startHover =
                x >= startX &&
                x <= startX + startWidth &&
                y >= taskbarY &&
                y <= taskbarY + height;


            if (MenuOpen)
            {
                TaskbarMenu.UpdateHover(x, y);
            }


            bool pressed =
                (MouseManager.MouseState & MouseState.Left) != 0;


            if (pressed && !lastPressed)
            {

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