using System;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HolmiumOS.GUI
{
    public static class TaskSwitcherManager
    {
        private static bool isSwitcherActive = false;
        private static int selectedIndex = 0;
        private static bool lastCtrlState = false;

        public static bool IsActive { get { return isSwitcherActive; } }

        public static void Update()
        {
            bool isCtrlPressed = KeyboardManager.ControlPressed;
            var windows = WindowManager.GetWindows();

            if (windows == null || windows.Count == 0) { isSwitcherActive = false; lastCtrlState = isCtrlPressed; return; }

            if (isCtrlPressed && !lastCtrlState)
            {
                if (isSwitcherActive)
                {
                    isSwitcherActive = false;
                }
                else if (windows.Count > 1)
                {
                    isSwitcherActive = true;
                    selectedIndex = 0;
                }
            }

            lastCtrlState = isCtrlPressed;
        }

        public static void HandleKeyboard()
        {
            if (!isSwitcherActive) return;
            if (!KeyboardManager.TryReadKey(out KeyEvent keyEvent)) return;

            var windows = WindowManager.GetWindows();

            if (windows == null || windows.Count == 0) { isSwitcherActive = false; return; }

            int visibleCount = Math.Min(windows.Count, 6);
            if (visibleCount <= 0) { isSwitcherActive = false; return; }

            switch (keyEvent.Key)
            {
                case ConsoleKeyEx.UpArrow:
                    selectedIndex--;
                    if (selectedIndex < 0) selectedIndex = visibleCount - 1;
                    break;

                case ConsoleKeyEx.DownArrow:
                    selectedIndex++;
                    if (selectedIndex >= visibleCount) selectedIndex = 0;
                    break;

                case ConsoleKeyEx.Enter:
                    ApplySelection();
                    isSwitcherActive = false;
                    break;
            }
        }

        private static void ApplySelection()
        {
            var windows = WindowManager.GetWindows();

            if (windows == null || windows.Count == 0) return;

            int visibleCount = Math.Min(windows.Count, 6);

            if (selectedIndex < 0 || selectedIndex >= visibleCount) selectedIndex = 0;

            int targetIndex = windows.Count - 1 - selectedIndex;

            if (targetIndex < 0 || targetIndex >= windows.Count) return;

            var targetWin = windows[targetIndex];

            if (targetWin == null) return;

            if (targetWin.IsMinimized) WindowManager.Restore(targetWin);
            else WindowManager.Focus(targetWin);
        }

        public static void Draw(Canvas canvas)
        {
            if (!isSwitcherActive || canvas == null) return;

            var windows = WindowManager.GetWindows();

            if (windows == null || windows.Count == 0) return;

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;

            int boxWidth = 300;
            int boxHeight = 180;

            int boxX = (screenWidth - boxWidth) / 2;
            int boxY = (screenHeight - boxHeight) / 2;

            canvas.DrawFilledRectangle(Color.FromArgb(192, 192, 192), boxX, boxY, boxWidth, boxHeight);
            canvas.DrawFilledRectangle(Color.White, boxX, boxY, boxWidth, 2);
            canvas.DrawFilledRectangle(Color.White, boxX, boxY, 2, boxHeight);
            canvas.DrawFilledRectangle(Color.Black, boxX, boxY + boxHeight - 2, boxWidth, 2);
            canvas.DrawFilledRectangle(Color.Black, boxX + boxWidth - 2, boxY, 2, boxHeight);

            canvas.DrawString("Acik Uygulamalar", PCScreenFont.Default, Color.Black, boxX + 12, boxY + 10);
            canvas.DrawFilledRectangle(Color.Gray, boxX + 8, boxY + 30, boxWidth - 16, 1);

            int startY = boxY + 40;
            int itemHeight = 20;
            int visibleCount = Math.Min(windows.Count, 6);

            for (int i = 0; i < visibleCount; i++)
            {
                int windowIndex = windows.Count - 1 - i;

                if (windowIndex < 0 || windowIndex >= windows.Count) continue;

                var win = windows[windowIndex];

                if (win == null) continue;

                int itemY = startY + (i * itemHeight);

                if (i == selectedIndex)
                    canvas.DrawFilledRectangle(Color.FromArgb(0, 0, 128), boxX + 8, itemY - 2, boxWidth - 16, 18);

                canvas.DrawString(win.Title ?? "Uygulama", PCScreenFont.Default, i == selectedIndex ? Color.White : Color.Black, boxX + 14, itemY);
            }
        }
    }
}