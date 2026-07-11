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

        private static int[] cursorBackground;

        private static int oldX;
        private static int oldY;

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


            canvas.DrawImage(
                wallpaper,
                0,
                0
            );

            Taskbar.Draw(canvas);

            AppManager.Run<Terminal>();

            DrawDebug();


            cursorBackground =
                new int[cursor.Width * cursor.Height];


            oldX = (int)MouseManager.X;
            oldY = (int)MouseManager.Y;


            oldX = Clamp(
                oldX,
                0,
                (int)canvas.Mode.Width - (int)cursor.Width
            );

            oldY = Clamp(
                oldY,
                0,
                (int)canvas.Mode.Height - (int)cursor.Height
            );


            SaveCursorArea(oldX, oldY);

            DrawCursor(oldX, oldY);


            canvas.Display();


            lastSecond = Cosmos.HAL.RTC.Second;


            while (true)
            {
                Taskbar.UpdateMouse(canvas);

                WindowManager.UpdateMouse();


                int x = (int)MouseManager.X;
                int y = (int)MouseManager.Y;


                int maxX =
                    (int)canvas.Mode.Width - (int)cursor.Width;

                int maxY =
                    (int)canvas.Mode.Height - (int)cursor.Height;


                if (maxX < 0)
                    maxX = 0;

                if (maxY < 0)
                    maxY = 0;


                x = Clamp(
                    x,
                    0,
                    maxX
                );


                y = Clamp(
                    y,
                    0,
                    maxY
                );


                if (x != oldX || y != oldY)
                {
                    RestoreCursorArea(oldX, oldY);
                }


                WindowManager.Draw(canvas);

                Taskbar.Draw(canvas);

                DrawDebug();


                SaveCursorArea(x, y);

                DrawCursor(x, y);


                canvas.Display();


                oldX = x;
                oldY = y;


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
            canvas.DrawImageAlpha(
                cursor,
                x,
                y
            );
        }


        private static void SaveCursorArea(int x, int y)
        {
            for (int cx = 0; cx < cursor.Width; cx++)
            {
                for (int cy = 0; cy < cursor.Height; cy++)
                {
                    int px = x + cx;
                    int py = y + cy;


                    if (px >= 0 &&
                        py >= 0 &&
                        px < canvas.Mode.Width &&
                        py < canvas.Mode.Height)
                    {
                        cursorBackground[
                            cy * cursor.Width + cx
                        ] =
                        canvas.GetRawPointColor(px, py);
                    }
                }
            }
        }


        private static void RestoreCursorArea(int x, int y)
        {
            for (int cx = 0; cx < cursor.Width; cx++)
            {
                for (int cy = 0; cy < cursor.Height; cy++)
                {
                    int px = x + cx;
                    int py = y + cy;


                    if (px >= 0 &&
                        py >= 0 &&
                        px < canvas.Mode.Width &&
                        py < canvas.Mode.Height)
                    {
                        canvas.DrawPoint(
                            cursorBackground[
                                cy * cursor.Width + cx
                            ],
                            px,
                            py
                        );
                    }
                }
            }
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
                "Resolution: " +
                canvas.Mode.Width +
                "x" +
                canvas.Mode.Height,
                PCScreenFont.Default,
                Color.White,
                5,
                20
            );


            canvas.DrawString(
                "FPS: " +
                fps +
                " Mouse: " +
                MouseManager.X +
                "," +
                MouseManager.Y,
                PCScreenFont.Default,
                Color.White,
                5,
                35
            );
        }


        private static int Clamp(
            int value,
            int min,
            int max
        )
        {
            if (value < min)
            {
                return min;
            }


            if (value > max)
            {
                return max;
            }


            return value;
        }
    }
}