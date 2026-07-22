using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.HAL;
using HolmiumOS.GUI.Apps;

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

        private static int clockWidth = 90;
        private static int clockHeight = 36;
        private static bool clockHover;

        public static int Height => height;

        public static void Draw(Canvas canvas)
        {
            int screenWidth = (int)canvas.Mode.Width;
            int y = (int)canvas.Mode.Height - height;

            canvas.DrawFilledRectangle(Color.FromArgb(40, 40, 40), 0, y, screenWidth, height);

            canvas.DrawFilledRectangle(
                startHover ? Color.FromArgb(100, 100, 100) : Color.FromArgb(70, 70, 70),
                startX, y + 7, startWidth, startHeight
            );
            canvas.DrawString("Start", Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, startX + 22, y + 18);

            int clockX = screenWidth - clockWidth - 15;
            clockHover = (MouseManager.X >= clockX && MouseManager.X <= clockX + clockWidth &&
                          MouseManager.Y >= y + 7 && MouseManager.Y <= y + 7 + clockHeight);

            canvas.DrawFilledRectangle(
                clockHover ? Color.FromArgb(90, 90, 90) : Color.FromArgb(60, 60, 60),
                clockX, y + 7, clockWidth, clockHeight
            );

            string timeString = $"{RTC.Hour:D2}:{RTC.Minute:D2}";
            canvas.DrawString(timeString, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, clockX + 16, y + 18);

            var allApps = AppManager.apps;
            int currentBtnX = startX + startWidth + 15;
            int maxButtonAreaX = clockX - 15;

            for (int i = 0; i < allApps.Count; i++)
            {
                var app = allApps[i];
                if (app == null || app.Window == null) continue;

                if (currentBtnX + 130 > maxButtonAreaX) break;

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
            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - height;
            int clockX = screenWidth - clockWidth - 15;

            startHover = mx >= startX && mx <= startX + startWidth && my >= taskbarY + 7 && my <= taskbarY + 7 + startHeight;
            clockHover = mx >= clockX && mx <= clockX + clockWidth && my >= taskbarY + 7 && my <= taskbarY + 7 + clockHeight;

            if (MenuOpen) TaskbarMenu.UpdateHover(mx, my);

            bool pressed = (MouseManager.MouseState & MouseState.Left) != 0;

            if (pressed && !lastPressed)
            {
                if (startHover)
                {
                    MenuOpen = !MenuOpen;
                }
                else if (clockHover)
                {
                    AppManager.Run<CalendarClock>(60, 60);
                    if (MenuOpen) MenuOpen = false;
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