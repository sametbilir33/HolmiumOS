using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.HAL;

namespace HolmiumOS.GUI
{
    public static class Taskbar
    {
        public static bool MenuOpen;

        private static bool lastPressed;
        private static bool startHover;
        private static bool clockHover;

        private const int height = 40;
        private const int bottomMargin = 0;
        private const int dockPadding = 4;

        private const int startWidth = 75;
        private const int buttonWidth = 130;
        private const int buttonHeight = 28;
        private const int buttonSpacing = 4;

        public static int Height => height + bottomMargin;

        public static int StartX(Canvas canvas)
        {
            return dockPadding;
        }

        private static int GetAppCount()
        {
            int count = 0;

            for (int i = 0; i < AppManager.apps.Count; i++)
            {
                var app = AppManager.apps[i];

                if (app == null || app.Window == null)
                    continue;

                count++;
            }

            return count;
        }

        private static void DrawRaisedBox(Canvas canvas, int x, int y, int width, int height)
        {
            canvas.DrawFilledRectangle(Color.FromArgb(192, 192, 192), x, y, width, height);
            canvas.DrawLine(Color.FromArgb(255, 255, 255), x, y, x + width - 1, y);
            canvas.DrawLine(Color.FromArgb(255, 255, 255), x, y, x, y + height - 1);
            canvas.DrawLine(Color.FromArgb(0, 0, 0), x, y + height - 1, x + width - 1, y + height - 1);
            canvas.DrawLine(Color.FromArgb(0, 0, 0), x + width - 1, y, x + width - 1, y + height - 1);
            canvas.DrawLine(Color.FromArgb(128, 128, 128), x + 1, y + height - 2, x + width - 2, y + height - 2);
            canvas.DrawLine(Color.FromArgb(128, 128, 128), x + width - 2, y + 1, x + width - 2, y + height - 2);
        }

        private static void DrawSunkenBox(Canvas canvas, int x, int y, int width, int height)
        {
            canvas.DrawFilledRectangle(Color.FromArgb(192, 192, 192), x, y, width, height);
            canvas.DrawLine(Color.FromArgb(128, 128, 128), x, y, x + width - 1, y);
            canvas.DrawLine(Color.FromArgb(128, 128, 128), x, y, x, y + height - 1);
            canvas.DrawLine(Color.FromArgb(0, 0, 0), x + 1, y + 1, x + width - 2, y + 1);
            canvas.DrawLine(Color.FromArgb(0, 0, 0), x + 1, y + 1, x + 1, y + height - 2);
            canvas.DrawLine(Color.FromArgb(255, 255, 255), x, y + height - 1, x + width - 1, y + height - 1);
            canvas.DrawLine(Color.FromArgb(255, 255, 255), x + width - 1, y, x + width - 1, y + height - 1);
        }

        public static void Draw(Canvas canvas)
        {
            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - height - bottomMargin;

            canvas.DrawFilledRectangle(Color.FromArgb(192, 192, 192), 0, taskbarY, screenWidth, height);
            canvas.DrawLine(Color.FromArgb(255, 255, 255), 0, taskbarY, screenWidth, taskbarY);

            bool startPressed = MenuOpen || startHover;
            if (startPressed)
                DrawSunkenBox(canvas, dockPadding, taskbarY + 6, startWidth, buttonHeight);
            else
                DrawRaisedBox(canvas, dockPadding, taskbarY + 6, startWidth, buttonHeight);

            canvas.DrawString(
                "Start",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.Black,
                dockPadding + 16,
                taskbarY + 12
            );

            int appCount = GetAppCount();
            int separatorX = dockPadding + startWidth + 8;

            string timeString = $"{RTC.Hour:D2}:{RTC.Minute:D2}";
            int clockBoxWidth = 64;
            int clockBoxHeight = buttonHeight;
            int clockX = screenWidth - clockBoxWidth - 8;
            int clockY = taskbarY + 6;

            if (appCount > 0)
            {
                canvas.DrawLine(Color.FromArgb(128, 128, 128), separatorX, taskbarY + 7, separatorX, taskbarY + height - 7);
                canvas.DrawLine(Color.FromArgb(255, 255, 255), separatorX + 1, taskbarY + 7, separatorX + 1, taskbarY + height - 7);

                int currentButtonX = separatorX + 6;
                int maxButtonAllowedX = clockX - 10;

                for (int i = 0; i < AppManager.apps.Count; i++)
                {
                    var app = AppManager.apps[i];

                    if (app == null || app.Window == null)
                        continue;

                    if (currentButtonX + buttonWidth > maxButtonAllowedX)
                        break;

                    var win = app.Window;
                    bool active = WindowManager.activeWindow == win && !win.IsMinimized;

                    if (active)
                        DrawSunkenBox(canvas, currentButtonX, taskbarY + 6, buttonWidth, buttonHeight);
                    else
                        DrawRaisedBox(canvas, currentButtonX, taskbarY + 6, buttonWidth, buttonHeight);

                    string title = win.Title;

                    if (title.Length > 14)
                        title = title.Substring(0, 14);

                    canvas.DrawString(
                        title,
                        Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                        Color.Black,
                        currentButtonX + 8,
                        taskbarY + 12
                    );

                    currentButtonX += buttonWidth + buttonSpacing;
                }
            }

            if (clockHover)
                DrawSunkenBox(canvas, clockX, clockY, clockBoxWidth, clockBoxHeight);
            else
                DrawRaisedBox(canvas, clockX, clockY, clockBoxWidth, clockBoxHeight);

            canvas.DrawString(
                timeString,
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.Black,
                clockX + 10,
                taskbarY + 12
            );

            if (MenuOpen)
            {
                TaskbarMenu.Draw(canvas);
            }
        }

        public static bool ContainsClock(int mx, int my, Canvas canvas)
        {
            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - Height;

            int clockBoxWidth = 64;
            int clockX = screenWidth - clockBoxWidth - 8;
            int clockY = taskbarY + 6;
            int clockHeight = buttonHeight;

            return mx >= clockX && mx <= clockX + clockBoxWidth && my >= clockY && my <= clockY + clockHeight;
        }

        public static void UpdateMouse(Canvas canvas)
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - height - bottomMargin;
            int separatorX = dockPadding + startWidth + 8;

            int clockBoxWidth = 64;
            int clockX = screenWidth - clockBoxWidth - 8;

            startHover =
                mx >= dockPadding &&
                mx <= dockPadding + startWidth &&
                my >= taskbarY + 6 &&
                my <= taskbarY + 6 + buttonHeight;

            clockHover =
                mx >= clockX &&
                mx <= clockX + clockBoxWidth &&
                my >= taskbarY + 6 &&
                my <= taskbarY + 6 + buttonHeight;

            if (MenuOpen)
                TaskbarMenu.UpdateHover(mx, my);

            bool pressed = (MouseManager.MouseState & MouseState.Left) != 0;

            if (pressed && !lastPressed)
            {
                if (startHover)
                {
                    MenuOpen = !MenuOpen;
                }
                else
                {
                    int appCount = GetAppCount();
                    if (appCount > 0)
                    {
                        int currentButtonX = separatorX + 6;
                        bool clickedWindow = false;

                        for (int i = 0; i < AppManager.apps.Count; i++)
                        {
                            var app = AppManager.apps[i];

                            if (app == null || app.Window == null)
                                continue;

                            if (
                                mx >= currentButtonX &&
                                mx <= currentButtonX + buttonWidth &&
                                my >= taskbarY + 6 &&
                                my <= taskbarY + 6 + buttonHeight
                            )
                            {
                                var win = app.Window;
                                if (win.IsMinimized)
                                {
                                    WindowManager.Restore(win);
                                }
                                else if (WindowManager.activeWindow == win)
                                {
                                    WindowManager.Minimize(win);
                                }
                                else
                                {
                                    WindowManager.Focus(win);
                                }

                                clickedWindow = true;
                                break;
                            }

                            currentButtonX += buttonWidth + buttonSpacing;
                        }

                        if (!clickedWindow && MenuOpen)
                        {
                            if (TaskbarMenu.IsInside(mx, my))
                                TaskbarMenu.Click(mx, my);
                            else
                                MenuOpen = false;
                        }
                    }
                    else if (MenuOpen)
                    {
                        if (TaskbarMenu.IsInside(mx, my))
                            TaskbarMenu.Click(mx, my);
                        else
                            MenuOpen = false;
                    }
                }
            }

            lastPressed = pressed;
        }
    }
}