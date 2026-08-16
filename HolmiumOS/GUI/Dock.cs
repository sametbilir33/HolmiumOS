using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class Dock
    {
        public static bool MenuOpen;

        private static bool lastPressed;
        private static bool startHover;

        private const int height = 54;
        private const int bottomMargin = 10;
        private const int dockPadding = 5;

        private const int startWidth = 70;
        private const int buttonWidth = 125;
        private const int buttonHeight = 36;
        private const int buttonSpacing = 8;

        public static int Height => height + bottomMargin;

        public static int StartX(Canvas canvas)
        {
            return GetDockX(canvas) + dockPadding;
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

        private static int GetDockWidth()
        {
            int appCount = GetAppCount();

            if (appCount == 0)
                return startWidth + dockPadding * 2;

            return dockPadding * 2 +
                   startWidth +
                   buttonSpacing +
                   (appCount * buttonWidth) +
                   ((appCount - 1) * buttonSpacing);
        }

        private static int GetDockX(Canvas canvas)
        {
            return ((int)canvas.Mode.Width - GetDockWidth()) / 2;
        }

        private static int GetDockY(Canvas canvas)
        {
            return (int)canvas.Mode.Height - height - bottomMargin;
        }

        public static void Draw(Canvas canvas)
        {
            int dockWidth = GetDockWidth();
            int dockX = GetDockX(canvas);
            int dockY = GetDockY(canvas);

            canvas.DrawFilledRectangle(
                Color.FromArgb(35, 35, 40),
                dockX,
                dockY,
                dockWidth,
                height
            );

            canvas.DrawFilledRectangle(
                startHover
                    ? Color.FromArgb(90, 90, 100)
                    : Color.FromArgb(60, 60, 70),
                dockX + dockPadding,
                dockY + 9,
                startWidth,
                buttonHeight
            );

            canvas.DrawString(
                "Start",
                Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                Color.White,
                dockX + dockPadding + 15,
                dockY + 19
            );

            int currentButtonX = dockX + dockPadding + startWidth + buttonSpacing;

            for (int i = 0; i < AppManager.apps.Count; i++)
            {
                var app = AppManager.apps[i];

                if (app == null || app.Window == null)
                    continue;

                var win = app.Window;
                bool active = WindowManager.activeWindow == win;

                Color buttonColor = active
                    ? Color.FromArgb(90, 90, 100)
                    : Color.FromArgb(55, 55, 65);

                canvas.DrawFilledRectangle(
                    buttonColor,
                    currentButtonX,
                    dockY + 9,
                    buttonWidth,
                    buttonHeight
                );

                string title = win.Title;

                if (title.Length > 12)
                    title = title.Substring(0, 12) + "..";

                canvas.DrawString(
                    title,
                    Cosmos.System.Graphics.Fonts.PCScreenFont.Default,
                    Color.White,
                    currentButtonX + 10,
                    dockY + 19
                );

                currentButtonX += buttonWidth + buttonSpacing;
            }

            if (MenuOpen)
                DockMenu.Draw(canvas);
        }

        public static void UpdateMouse(Canvas canvas)
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;

            int dockX = GetDockX(canvas);
            int dockY = GetDockY(canvas);

            startHover =
                mx >= dockX + dockPadding &&
                mx <= dockX + dockPadding + startWidth &&
                my >= dockY + 9 &&
                my <= dockY + 9 + buttonHeight;

            if (MenuOpen)
                DockMenu.UpdateHover(mx, my);

            bool pressed = (MouseManager.MouseState & MouseState.Left) != 0;

            if (pressed && !lastPressed)
            {
                if (startHover)
                {
                    MenuOpen = !MenuOpen;
                }
                else
                {
                    int currentButtonX =
                        dockX + dockPadding + startWidth + buttonSpacing;

                    bool clickedWindow = false;

                    for (int i = 0; i < AppManager.apps.Count; i++)
                    {
                        var app = AppManager.apps[i];

                        if (app == null || app.Window == null)
                            continue;

                        if (
                            mx >= currentButtonX &&
                            mx <= currentButtonX + buttonWidth &&
                            my >= dockY + 9 &&
                            my <= dockY + 9 + buttonHeight
                        )
                        {
                            WindowManager.Focus(app.Window);
                            clickedWindow = true;
                            break;
                        }

                        currentButtonX += buttonWidth + buttonSpacing;
                    }

                    if (!clickedWindow && MenuOpen)
                    {
                        if (DockMenu.IsInside(mx, my))
                            DockMenu.Click(mx, my);
                        else
                            MenuOpen = false;
                    }
                }
            }

            lastPressed = pressed;
        }
    }
}