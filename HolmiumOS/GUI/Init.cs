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

        [ManifestResourceStream(ResourceName = "HolmiumOS.Resources.Walpaper.bmp")]
        private static byte[] wallpaperData;

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

            CursorManager.Initialize();

            wallpaper = new Bitmap(wallpaperData);

            MouseManager.ScreenWidth = canvas.Mode.Width;
            MouseManager.ScreenHeight = canvas.Mode.Height;

            MouseManager.X = canvas.Mode.Width / 2;
            MouseManager.Y = canvas.Mode.Height / 2;

            while (isGuiLoopRunning)
            {
                bool isLoggedIn = UserManager.IsLoggedIn;
                bool isLoginAppOpen = CheckIfLoginIsOpen();

                if (!isLoggedIn && !isLoginAppOpen)
                    AppManager.Run<Login>(60, 60);

                if (isLoggedIn)
                    Taskbar.UpdateMouse(canvas);

                DesktopManager.UpdateMouse(canvas);
                WindowManager.UpdateMouse(canvas);
                NotificationManager.UpdateMouse(canvas);
                WindowManager.HandleKeyboard();

                TaskSwitcherManager.Update();

                canvas.DrawImage(wallpaper, 0, 0);

                DesktopManager.Draw(canvas);
                WindowManager.Draw(canvas);

                if (isLoggedIn)
                    Taskbar.Draw(canvas);

                NotificationManager.Draw(canvas);

                TaskSwitcherManager.Draw(canvas);

                CursorManager.Draw(canvas);

                canvas.Display();
            }
        }

        private static bool CheckIfLoginIsOpen()
        {
            for (int i = 0; i < AppManager.apps.Count; i++)
            {
                if (AppManager.apps[i] is Login)
                    return true;
            }

            return false;
        }
    }
}