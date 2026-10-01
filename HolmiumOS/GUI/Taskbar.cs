using System;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using Cosmos.Kernel.System.Mouse;

namespace HolmiumOS.GUI
{
    public static class Taskbar
    {
        public static bool MenuOpen;

        private static bool lastPressed;
        private static bool startHover;
        private static bool clockHover;

        private const int height = 48;
        private const int bottomMargin = 0;
        private const int dockPadding = 4;

        private const int startWidth = 90;
        private const int buttonWidth = 180;
        private const int buttonHeight = 40;
        private const int buttonSpacing = 4;

        public static int Height => height + bottomMargin;

        public static int StartX(Canvas canvas)
        {
            return dockPadding;
        }

        private static int GetAppCount()
        {
            int count = 0;

            for (int i = 0;
                 i < AppManager.apps.Count;
                 i++)
            {
                var app = AppManager.apps[i];

                if (app == null ||
                    app.Window == null)
                {
                    continue;
                }

                count++;
            }

            return count;
        }

        private static string FitTitle(
            string title,
            Font font,
            int maxWidth)
        {
            if (string.IsNullOrEmpty(title))
                return "";

            if (font.MeasureString(title) <= maxWidth)
                return title;

            string result = title;

            while (result.Length > 2 &&
                   font.MeasureString(result + "..") >
                   maxWidth)
            {
                result =
                    result.Substring(
                        0,
                        result.Length - 1);
            }

            return result + "..";
        }

        private static void DrawRaisedBox(
            Canvas canvas,
            int x,
            int y,
            int width,
            int height)
        {
            canvas.DrawFilledRectangle(
                Color.FromArgb(192, 192, 192),
                x,
                y,
                width,
                height);

            canvas.DrawLine(
                Color.FromArgb(255, 255, 255),
                x,
                y,
                x + width - 1,
                y);

            canvas.DrawLine(
                Color.FromArgb(255, 255, 255),
                x,
                y,
                x,
                y + height - 1);

            canvas.DrawLine(
                Color.FromArgb(0, 0, 0),
                x,
                y + height - 1,
                x + width - 1,
                y + height - 1);

            canvas.DrawLine(
                Color.FromArgb(0, 0, 0),
                x + width - 1,
                y,
                x + width - 1,
                y + height - 1);

            canvas.DrawLine(
                Color.FromArgb(128, 128, 128),
                x + 1,
                y + height - 2,
                x + width - 2,
                y + height - 2);

            canvas.DrawLine(
                Color.FromArgb(128, 128, 128),
                x + width - 2,
                y + 1,
                x + width - 2,
                y + height - 2);
        }

        private static void DrawSunkenBox(
            Canvas canvas,
            int x,
            int y,
            int width,
            int height)
        {
            canvas.DrawFilledRectangle(
                Color.FromArgb(192, 192, 192),
                x,
                y,
                width,
                height);

            canvas.DrawLine(
                Color.FromArgb(128, 128, 128),
                x,
                y,
                x + width - 1,
                y);

            canvas.DrawLine(
                Color.FromArgb(128, 128, 128),
                x,
                y,
                x,
                y + height - 1);

            canvas.DrawLine(
                Color.FromArgb(0, 0, 0),
                x + 1,
                y + 1,
                x + width - 2,
                y + 1);

            canvas.DrawLine(
                Color.FromArgb(0, 0, 0),
                x + 1,
                y + 1,
                x + 1,
                y + height - 2);

            canvas.DrawLine(
                Color.FromArgb(255, 255, 255),
                x,
                y + height - 1,
                x + width - 1,
                y + height - 1);

            canvas.DrawLine(
                Color.FromArgb(255, 255, 255),
                x + width - 1,
                y,
                x + width - 1,
                y + height - 1);
        }

        public static void Draw(Canvas canvas)
        {
            Font font =
                PCScreenFont.DefaultFont;

            int screenWidth =
                (int)canvas.Mode.Width;

            int taskbarY =
                (int)canvas.Mode.Height -
                height -
                bottomMargin;

            canvas.DrawFilledRectangle(
                Color.FromArgb(192, 192, 192),
                0,
                taskbarY,
                screenWidth,
                height);

            canvas.DrawLine(
                Color.FromArgb(255, 255, 255),
                0,
                taskbarY,
                screenWidth,
                taskbarY);

            bool startPressed =
                MenuOpen ||
                startHover;

            int startY =
                taskbarY +
                (height - buttonHeight) / 2;

            if (startPressed)
            {
                DrawSunkenBox(
                    canvas,
                    dockPadding,
                    startY,
                    startWidth,
                    buttonHeight);
            }
            else
            {
                DrawRaisedBox(
                    canvas,
                    dockPadding,
                    startY,
                    startWidth,
                    buttonHeight);
            }

            int startTextWidth =
                font.MeasureString("Start");

            int startTextX =
                dockPadding +
                (startWidth - startTextWidth) / 2;

            int startTextY =
                startY +
                (buttonHeight - font.Height) / 2;

            canvas.DrawString(
                "Start",
                font,
                Color.Black,
                startTextX,
                startTextY);

            int appCount =
                GetAppCount();

            int separatorX =
                dockPadding +
                startWidth +
                8;

            DateTime now =
                DateTime.Now;

            string timeString =
                $"{now.Hour:D2}:{now.Minute:D2}";

            int clockBoxWidth = 90;
            int clockBoxHeight = buttonHeight;

            int clockX =
                screenWidth -
                clockBoxWidth -
                8;

            int clockY =
                taskbarY +
                (height - buttonHeight) / 2;

            if (appCount > 0)
            {
                canvas.DrawLine(
                    Color.FromArgb(128, 128, 128),
                    separatorX,
                    taskbarY + 7,
                    separatorX,
                    taskbarY + height - 7);

                canvas.DrawLine(
                    Color.FromArgb(255, 255, 255),
                    separatorX + 1,
                    taskbarY + 7,
                    separatorX + 1,
                    taskbarY + height - 7);

                int currentButtonX =
                    separatorX + 6;

                int maxButtonAllowedX =
                    clockX - 10;

                for (int i = 0;
                     i < AppManager.apps.Count;
                     i++)
                {
                    var app =
                        AppManager.apps[i];

                    if (app == null ||
                        app.Window == null)
                    {
                        continue;
                    }

                    if (currentButtonX +
                        buttonWidth >
                        maxButtonAllowedX)
                    {
                        break;
                    }

                    var win =
                        app.Window;

                    bool active =
                        WindowManager.activeWindow ==
                            win &&
                        !win.IsMinimized;

                    int buttonY =
                        taskbarY +
                        (height - buttonHeight) / 2;

                    if (active)
                    {
                        DrawSunkenBox(
                            canvas,
                            currentButtonX,
                            buttonY,
                            buttonWidth,
                            buttonHeight);
                    }
                    else
                    {
                        DrawRaisedBox(
                            canvas,
                            currentButtonX,
                            buttonY,
                            buttonWidth,
                            buttonHeight);
                    }

                    string title =
                        FitTitle(
                            win.Title,
                            font,
                            buttonWidth - 16);

                    int titleWidth =
                        font.MeasureString(title);

                    int titleX =
                        currentButtonX +
                        (buttonWidth - titleWidth) / 2;

                    int titleY =
                        buttonY +
                        (buttonHeight - font.Height) / 2;

                    canvas.DrawString(
                        title,
                        font,
                        Color.Black,
                        titleX,
                        titleY);

                    currentButtonX +=
                        buttonWidth +
                        buttonSpacing;
                }
            }

            if (clockHover)
            {
                DrawSunkenBox(
                    canvas,
                    clockX,
                    clockY,
                    clockBoxWidth,
                    clockBoxHeight);
            }
            else
            {
                DrawRaisedBox(
                    canvas,
                    clockX,
                    clockY,
                    clockBoxWidth,
                    clockBoxHeight);
            }

            int clockTextWidth =
                font.MeasureString(timeString);

            int clockTextX =
                clockX +
                (clockBoxWidth - clockTextWidth) / 2;

            int clockTextY =
                clockY +
                (clockBoxHeight - font.Height) / 2;

            canvas.DrawString(
                timeString,
                font,
                Color.Black,
                clockTextX,
                clockTextY);

            if (MenuOpen)
            {
                TaskbarMenu.Draw(canvas);
            }
        }

        public static bool ContainsClock(
            int mx,
            int my,
            Canvas canvas)
        {
            int screenWidth =
                (int)canvas.Mode.Width;

            int taskbarY =
                (int)canvas.Mode.Height -
                Height;

            int clockBoxWidth = 90;

            int clockX =
                screenWidth -
                clockBoxWidth -
                8;

            int clockY =
                taskbarY +
                (height - buttonHeight) / 2;

            int clockHeight =
                buttonHeight;

            return mx >= clockX &&
                   mx <= clockX + clockBoxWidth &&
                   my >= clockY &&
                   my <= clockY + clockHeight;
        }

        public static void UpdateMouse(
            Canvas canvas)
        {
            int mx =
                (int)MouseManager.X;

            int my =
                (int)MouseManager.Y;

            int screenWidth =
                (int)canvas.Mode.Width;

            int taskbarY =
                (int)canvas.Mode.Height -
                height -
                bottomMargin;

            int separatorX =
                dockPadding +
                startWidth +
                8;

            int clockBoxWidth = 90;

            int clockX =
                screenWidth -
                clockBoxWidth -
                8;

            int buttonY =
                taskbarY +
                (height - buttonHeight) / 2;

            startHover =
                mx >= dockPadding &&
                mx <= dockPadding + startWidth &&
                my >= buttonY &&
                my <= buttonY + buttonHeight;

            clockHover =
                mx >= clockX &&
                mx <= clockX + clockBoxWidth &&
                my >= buttonY &&
                my <= buttonY + buttonHeight;

            if (MenuOpen)
            {
                TaskbarMenu.UpdateHover(
                    mx,
                    my);
            }

            bool pressed =
                MouseManager.LeftButton;

            if (pressed && !lastPressed)
            {
                if (startHover)
                {
                    MenuOpen = !MenuOpen;
                }
                else
                {
                    int appCount =
                        GetAppCount();

                    if (appCount > 0)
                    {
                        int currentButtonX =
                            separatorX + 6;

                        bool clickedWindow =
                            false;

                        for (int i = 0;
                             i < AppManager.apps.Count;
                             i++)
                        {
                            var app =
                                AppManager.apps[i];

                            if (app == null ||
                                app.Window == null)
                            {
                                continue;
                            }

                            if (
                                mx >= currentButtonX &&
                                mx <= currentButtonX +
                                      buttonWidth &&
                                my >= buttonY &&
                                my <= buttonY +
                                      buttonHeight
                            )
                            {
                                var win =
                                    app.Window;

                                if (win.IsMinimized)
                                {
                                    WindowManager.Restore(
                                        win);
                                }
                                else if (
                                    WindowManager.activeWindow ==
                                    win)
                                {
                                    WindowManager.Minimize(
                                        win);
                                }
                                else
                                {
                                    WindowManager.Focus(
                                        win);
                                }

                                clickedWindow = true;
                                break;
                            }

                            currentButtonX +=
                                buttonWidth +
                                buttonSpacing;
                        }

                        if (!clickedWindow &&
                            MenuOpen)
                        {
                            if (TaskbarMenu.IsInside(
                                    mx,
                                    my))
                            {
                                TaskbarMenu.Click(
                                    mx,
                                    my);
                            }
                            else
                            {
                                MenuOpen = false;
                            }
                        }
                    }
                    else if (MenuOpen)
                    {
                        if (TaskbarMenu.IsInside(
                                mx,
                                my))
                        {
                            TaskbarMenu.Click(
                                mx,
                                my);
                        }
                        else
                        {
                            MenuOpen = false;
                        }
                    }
                }
            }

            lastPressed = pressed;
        }
    }
}