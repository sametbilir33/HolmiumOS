using System.Collections.Generic;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class TaskSwitcherManager
    {
        private static bool isSwitcherActive = false;
        private static int selectedIndex = 0;
        private static bool lastCtrlState = false;

        public static void Update()
        {
            bool isCtrlPressed = KeyboardManager.ControlPressed;

            if (isCtrlPressed && !lastCtrlState)
            {
                var windows = WindowManager.GetWindows();
                if (windows != null && windows.Count > 1)
                {
                    if (!isSwitcherActive)
                    {
                        isSwitcherActive = true;
                        selectedIndex = 1;
                    }
                    else
                    {
                        selectedIndex = (selectedIndex + 1) % windows.Count;
                    }
                }
            }

            if (isSwitcherActive && !isCtrlPressed && lastCtrlState)
            {
                ApplySelection();
                isSwitcherActive = false;
            }

            lastCtrlState = isCtrlPressed;
        }

        private static void ApplySelection()
        {
            var windows = WindowManager.GetWindows();
            if (windows != null && windows.Count > 0)
            {
                int targetIndex = windows.Count - 1 - selectedIndex;
                if (targetIndex >= 0 && targetIndex < windows.Count)
                {
                    var targetWin = windows[targetIndex];
                    if (targetWin.IsMinimized)
                    {
                        WindowManager.Restore(targetWin);
                    }
                    else
                    {
                        WindowManager.Focus(targetWin);
                    }
                }
            }
        }

        public static void Draw(Canvas canvas)
        {
            if (!isSwitcherActive) return;

            var windows = WindowManager.GetWindows();
            if (windows == null || windows.Count == 0) return;

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;

            int boxWidth = 260;
            int boxHeight = 160;
            int boxX = (screenWidth - boxWidth) / 2;
            int boxY = (screenHeight - boxHeight) / 2;

            canvas.DrawFilledRectangle(Color.FromArgb(192, 192, 192), boxX, boxY, boxWidth, boxHeight);
            canvas.DrawLine(Color.FromArgb(255, 255, 255), boxX, boxY, boxX + boxWidth - 1, boxY);
            canvas.DrawLine(Color.FromArgb(255, 255, 255), boxX, boxY, boxX, boxY + boxHeight - 1);
            canvas.DrawLine(Color.FromArgb(0, 0, 0), boxX, boxY + boxHeight - 1, boxX + boxWidth - 1, boxY + boxHeight - 1);
            canvas.DrawLine(Color.FromArgb(0, 0, 0), boxX + boxWidth - 1, boxY, boxX + boxWidth - 1, boxY + boxHeight - 1);

            canvas.DrawString("Running Applications (Ctrl)", Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.Black, boxX + 12, boxY + 12);

            int startY = boxY + 38;
            for (int i = 0; i < windows.Count; i++)
            {
                if (i >= 5) break;
                var win = windows[windows.Count - 1 - i];

                if (i == selectedIndex)
                {
                    canvas.DrawFilledRectangle(Color.FromArgb(0, 0, 128), boxX + 10, startY + (i * 20) - 2, boxWidth - 20, 18);
                    canvas.DrawString(win.Title, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, boxX + 15, startY + (i * 20));
                }
                else
                {
                    canvas.DrawString(win.Title, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.Black, boxX + 15, startY + (i * 20));
                }
            }
        }
    }
}