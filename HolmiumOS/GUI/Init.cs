using System;
using System.IO;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Mouse;
using HolmiumOS.GUI.Apps;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI
{
    public static class Init
    {
        public static Canvas canvas = null!;

        private static Bitmap? wallpaper;

        public static bool isGuiLoopRunning = true;

        public static void Start()
        {
            isGuiLoopRunning = true;

            canvas = Canvas.GetFullScreen();

            CursorManager.Initialize();

            wallpaper = LoadWallpaper();

            while (isGuiLoopRunning)
            {
                bool isLoggedIn =
                    UserManager.IsLoggedIn;

                bool isLoginAppOpen =
                    CheckIfLoginIsOpen();

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

                TaskSwitcherManager.Update();

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

                TaskSwitcherManager.Draw(canvas);

                CursorManager.Draw(canvas);

                canvas.Display();
            }
        }

        private static Bitmap? LoadWallpaper()
        {
            string[] paths =
            {
                "/mnt/boot/Walpaper.bmp",
                "/mnt/Walpaper.bmp",
                "/mnt/resources/Walpaper.bmp",
                "/mnt/Resources/Walpaper.bmp",
                "/mnt/etc/Walpaper.bmp"
            };

            foreach (string path in paths)
            {
                try
                {
                    if (!File.Exists(path))
                    {
                        continue;
                    }

                    return new Bitmap(path);
                }
                catch
                {
                    // Bir sonraki yolu dene.
                }
            }

            return null;
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