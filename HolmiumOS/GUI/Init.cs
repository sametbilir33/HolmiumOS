using Cosmos.System;
using Cosmos.System.Graphics;
using HolmiumOS.GUI.Apps;
using HolmiumOS.Shell;
using IL2CPU.API.Attribs;

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
        private static bool isLoginAppOpen = false;

        public static void Start()
        {
            canvas = FullScreenCanvas.GetFullScreenCanvas();

            cursor = new Bitmap(cursorData);
            wallpaper = new Bitmap(wallpaperData);

            MouseManager.ScreenWidth = canvas.Mode.Width;
            MouseManager.ScreenHeight = canvas.Mode.Height;

            MouseManager.X = canvas.Mode.Width / 2;
            MouseManager.Y = canvas.Mode.Height / 2;

            while (true)
            {
                isLoginAppOpen = false;
                for (int i = 0; i < AppManager.apps.Count; i++)
                {
                    if (AppManager.apps[i] is Login)
                    {
                        isLoginAppOpen = true;
                        break;
                    }
                }

                if (!UserManager.IsLoggedIn && !isLoginAppOpen)
                {
                    AppManager.Run<Login>(60, 60);
                    isLoginAppOpen = true;
                }

                if (UserManager.IsLoggedIn)
                {
                    Taskbar.UpdateMouse(canvas);
                }

                WindowManager.UpdateMouse(canvas);
                NotificationManager.UpdateMouse(canvas);

                WindowManager.HandleKeyboard();

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

                if (UserManager.IsLoggedIn)
                {
                    Taskbar.Draw(canvas);
                }

                NotificationManager.Draw(canvas);

                DrawCursor(x, y);

                canvas.Display();

            }
        }

        private static void DrawCursor(int x, int y)
        {
            canvas.DrawImageAlpha(cursor, x, y);
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}