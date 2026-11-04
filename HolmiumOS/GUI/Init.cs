using System;
using System.IO;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Input;
using HolmiumOS.GUI.Apps;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI
{
    public static class Init
    {
        public static Canvas canvas = null!;

        private static Png? wallpaper;

        public static bool isGuiLoopRunning = true;

        public static void Start()
        {
            isGuiLoopRunning = true;

            canvas = Canvas.GetFullScreen();

            MouseManager.SetScreenSize(canvas.Width, canvas.Height);

            CursorManager.Initialize();

            wallpaper = LoadWallpaper();

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
                    Taskbar.UpdateMouse(canvas);
                }

                DesktopManager.UpdateMouse(canvas);
                WindowManager.UpdateMouse(canvas);
                NotificationManager.UpdateMouse(canvas);
                WindowManager.HandleKeyboard();

                if (wallpaper is not null)
                {
                    canvas.DrawImage(
                        wallpaper,
                        0,
                        0);
                }

                DesktopManager.Draw(canvas);
                WindowManager.Draw(canvas);

                if (isLoggedIn)
                {
                    Taskbar.Draw(canvas);
                }

                NotificationManager.Draw(canvas);

                CursorManager.Draw(canvas);

                canvas.Display();
            }
        }

        private static Png? LoadWallpaper()
{
    const string path = "/cdrom/Resources/Wallpaper.png";

    if (!File.Exists(path))
        return null;

    using FileStream stream = new FileStream(
        path,
        FileMode.Open,
        FileAccess.Read,
        FileShare.Read
    );

    return new Png(stream);
}

        private static bool CheckIfLoginIsOpen()
        {
            for (int i = 0;
                 i < AppManager.apps.Count;
                 i++)
            {
                if (AppManager.apps[i] is Login)
                {
                    return true;
                }
            }

            return false;
        }
    }
}