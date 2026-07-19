using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using IL2CPU.API.Attribs;
using HolmiumOS.GUI.Apps;

namespace HolmiumOS.GUI
{
    public static class Init
    {
        private static Canvas canvas;

        [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.Cursor.bmp")]
        private static byte[] cursorData;

        [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.Walpaper.bmp")]
        private static byte[] wallpaperData;

        private static Bitmap cursor;
        private static Bitmap wallpaper;

        private static int fps;
        private static int frames;
        private static uint lastSecond;

        public static void Start()
        {
            canvas = FullScreenCanvas.GetFullScreenCanvas();

            cursor = new Bitmap(cursorData);
            wallpaper = new Bitmap(wallpaperData);

            MouseManager.ScreenWidth = canvas.Mode.Width;
            MouseManager.ScreenHeight = canvas.Mode.Height;

            AppManager.Run<Terminal>();

            lastSecond = Cosmos.HAL.RTC.Second;

            while (true)
            {
                Taskbar.UpdateMouse(canvas);
                WindowManager.UpdateMouse();

                int x = (int)MouseManager.X;
                int y = (int)MouseManager.Y;

                int maxX = (int)canvas.Mode.Width - (int)cursor.Width;
                int maxY = (int)canvas.Mode.Height - (int)cursor.Height;

                if (maxX < 0) maxX = 0;
                if (maxY < 0) maxY = 0;

                x = Clamp(x, 0, maxX);
                y = Clamp(y, 0, maxY);


                canvas.DrawImage(wallpaper, 0, 0);

                WindowManager.Draw(canvas);

                Taskbar.Draw(canvas);

                DrawDebug();

                DrawCursor(x, y);

                canvas.Display();


                frames++;
                if (Cosmos.HAL.RTC.Second != lastSecond)
                {
                    fps = frames;
                    frames = 0;
                    lastSecond = Cosmos.HAL.RTC.Second;
                }
            }
        }

        private static void DrawCursor(int x, int y)
        {
            canvas.DrawImageAlpha(cursor, x, y);
        }

        private static void DrawDebug()
        {
            canvas.DrawFilledRectangle(
                Color.Black,
                0,
                0,
                300,
                70
            );

            canvas.DrawString(
                "HolmiumOS Debug",
                PCScreenFont.Default,
                Color.White,
                5,
                5
            );

            canvas.DrawString(
                "Resolution: " + canvas.Mode.Width + "x" + canvas.Mode.Height,
                PCScreenFont.Default,
                Color.White,
                5,
                20
            );

            canvas.DrawString(
                "FPS: " + fps + " Mouse: " + MouseManager.X + "," + MouseManager.Y,
                PCScreenFont.Default,
                Color.White,
                5,
                35
            );
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}