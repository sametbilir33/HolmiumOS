using Cosmos.System;
using Cosmos.System.Graphics;
using HolmiumOS.GUI.Apps;
using HolmiumOS.Shell;
using IL2CPU.API.Attribs;

namespace HolmiumOS.GUI
{
    public static class Init
    {
        public static Canvas canvas;

        [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.Cursor.bmp")]
        private static byte[] cursorData;

        [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.Walpaper.bmp")]
        private static byte[] wallpaperData;

        private static Bitmap cursor;
        private static Bitmap wallpaper;
        public static bool isGuiLoopRunning = true;

        public static void Start()
        {
            isGuiLoopRunning = true;

            try
            {
                canvas = FullScreenCanvas.GetFullScreenCanvas(new Mode(1920, 1080, ColorDepth.ColorDepth32));
            }
            catch
            {
                canvas = FullScreenCanvas.GetFullScreenCanvas();
            }

            cursor = new Bitmap(cursorData);
            wallpaper = new Bitmap(wallpaperData);

            MouseManager.ScreenWidth = canvas.Mode.Width;
            MouseManager.ScreenHeight = canvas.Mode.Height;

            MouseManager.X = canvas.Mode.Width / 2;
            MouseManager.Y = canvas.Mode.Height / 2;

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;
            int cursorWidth = (int)cursor.Width;
            int cursorHeight = (int)cursor.Height;

            while (isGuiLoopRunning)
            {
                bool isLoggedIn = UserManager.IsLoggedIn;
                bool isLoginAppOpen = CheckIfLoginIsOpen();

                if (!isLoggedIn && !isLoginAppOpen)
                {
                    AppManager.Run<Login>(60, 60);
                }

                if (isLoggedIn)
                {
                    Dock.UpdateMouse(canvas);
                }

                DesktopManager.UpdateMouse(canvas);
                WindowManager.UpdateMouse(canvas);
                NotificationManager.UpdateMouse(canvas);
                WindowManager.HandleKeyboard();

                int x = Clamp((int)MouseManager.X, 0, screenWidth - cursorWidth);
                int y = Clamp((int)MouseManager.Y, 0, screenHeight - cursorHeight);

                canvas.DrawImage(wallpaper, 0, 0);

                if (isLoggedIn)
                {
                    StatusBar.Draw(canvas);
                }

                DesktopManager.Draw(canvas);
                WindowManager.Draw(canvas);

                if (isLoggedIn)
                {
                    Dock.Draw(canvas);
                }

                NotificationManager.Draw(canvas);
                DrawCursor(x, y);

                canvas.Display();
            }
        }

        private static bool CheckIfLoginIsOpen()
        {
            for (int i = 0; i < AppManager.apps.Count; i++)
            {
                if (AppManager.apps[i] is Login)
                {
                    return true;
                }
            }

            return false;
        }

        private static void DrawCursor(int x, int y)
        {
            canvas.DrawImageAlpha(cursor, x, y);
        }

        private static int Clamp(int value, int min, int max)
        {
            if (min > max) return min;
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}