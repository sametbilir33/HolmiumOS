using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class Taskbar
    {
        public static bool MenuOpen;
        private static int height = 50;
        private static bool lastPressed;

        private static int startX = 10;
        private static int startWidth = 80;
        private static int startHeight = 36;
        private static bool startHover;

        public static int Height => height;

        public static void Draw(Canvas canvas)
        {
            int y = (int)canvas.Mode.Height - height;

            canvas.DrawFilledRectangle(Color.FromArgb(40, 40, 40), 0, y, (int)canvas.Mode.Width, height);

            canvas.DrawFilledRectangle(
                startHover ? Color.FromArgb(100, 100, 100) : Color.FromArgb(70, 70, 70),
                startX, y + 7, startWidth, startHeight
            );
            canvas.DrawString("Start", Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, startX + 22, y + 18);

            var allApps = AppManager.apps;
            int currentBtnX = startX + startWidth + 15;

            for (int i = 0; i < allApps.Count; i++)
            {
                var app = allApps[i];
                if (app == null || app.Window == null) continue;

                var win = app.Window;

                bool isWindowActive = (WindowManager.activeWindow == win);

                Color btnColor = isWindowActive ? Color.FromArgb(90, 90, 90) : Color.FromArgb(50, 50, 50);

                canvas.DrawFilledRectangle(btnColor, currentBtnX, y + 7, 130, startHeight);

                string shortTitle = win.Title.Length > 12 ? win.Title.Substring(0, 12) + ".." : win.Title;
                canvas.DrawString(shortTitle, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, currentBtnX + 10, y + 18);

                currentBtnX += 140;
            }

            if (MenuOpen)
            {
                TaskbarMenu.Draw(canvas);
            }
        }

        public static void UpdateMouse(Canvas canvas)
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            int taskbarY = (int)canvas.Mode.Height - height;

            startHover = mx >= startX && mx <= startX + startWidth && my >= taskbarY + 7 && my <= taskbarY + 7 + startHeight;

            if (MenuOpen) TaskbarMenu.UpdateHover(mx, my);

            bool pressed = (MouseManager.MouseState & MouseState.Left) != 0;

            if (pressed && !lastPressed)
            {
                if (startHover)
                {
                    MenuOpen = !MenuOpen;
                }
                else
                {
                    var allApps = AppManager.apps;
                    int currentBtnX = startX + startWidth + 15;
                    bool clickedAWindowButton = false;

                    for (int i = 0; i < allApps.Count; i++)
                    {
                        var app = allApps[i];
                        if (app == null || app.Window == null) continue;

                        var win = app.Window;

                        if (mx >= currentBtnX && mx <= currentBtnX + 130 && my >= taskbarY + 7 && my <= taskbarY + 7 + startHeight)
                        {
                            WindowManager.Focus(win);
                            clickedAWindowButton = true;
                            break;
                        }
                        currentBtnX += 140;
                    }

                    if (!clickedAWindowButton)
                    {
                        if (MenuOpen && !TaskbarMenu.IsInside(mx, my))
                        {
                            MenuOpen = false;
                        }
                        else if (MenuOpen)
                        {
                            TaskbarMenu.Click(mx, my);
                        }
                    }
                }
            }
            lastPressed = pressed;
        }
    }
}