using System;
using System.Collections.Generic;
using Cosmos.System;
using Cosmos.System.Graphics;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI
{
    public static class WindowManager
    {
        private static List<Window> windows = new List<Window>();
        public static Window activeWindow;
        private static bool wasPressed = false;

        public static void Add(Window window)
        {
            windows.Add(window);
            Focus(window);
        }

        public static void Remove(Window window)
        {
            if (activeWindow == window)
            {
                activeWindow = null;
            }
            windows.Remove(window);
            AppManager.Close(window.App);
        }

        public static void Minimize(Window window)
        {
            if (window == null) return;
            window.IsMinimized = true;

            if (activeWindow == window)
            {
                activeWindow = null;
                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    if (!windows[i].IsMinimized)
                    {
                        Focus(windows[i]);
                        break;
                    }
                }
            }
        }

        public static void Restore(Window window)
        {
            if (window == null) return;
            window.IsMinimized = false;
            Focus(window);
        }

        public static void Draw(Canvas canvas)
        {
            if (windows == null || windows.Count == 0) return;

            int count = windows.Count;
            for (int i = 0; i < count; i++)
            {
                if (i >= windows.Count) break;
                if (windows[i] != null)
                {
                    windows[i].Draw(canvas);
                }
            }
        }

        public static void Focus(Window window)
        {
            if (window == null || activeWindow == window) return;

            if (activeWindow != null)
            {
                activeWindow.Active = false;
            }

            activeWindow = window;
            activeWindow.Active = true;

            if (windows.Contains(window))
            {
                windows.Remove(window);
                windows.Add(window);
            }
        }

        public static void ClearFocus()
        {
            if (activeWindow != null)
            {
                activeWindow.Active = false;
                activeWindow = null;
            }
        }

        public static List<Window> GetWindows()
        {
            return windows;
        }

        public static void UpdateMouse(Canvas canvas)
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            bool isPressed = (MouseManager.MouseState == MouseState.Left);

            if (isPressed && !wasPressed)
            {
                if (Taskbar.ContainsClock(mx, my, canvas))
                {
                    AppManager.Run<Apps.CalendarClock>(50, 50);
                    wasPressed = isPressed;
                    return;
                }

                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    if (i >= windows.Count) break;

                    Window window = windows[i];

                    if (window == null || window.IsMinimized)
                        continue;

                    if (window.CloseContains(mx, my))
                    {
                        Remove(window);
                        wasPressed = isPressed;
                        return;
                    }

                    if (window.MinimizeContains(mx, my))
                    {
                        Minimize(window);
                        wasPressed = isPressed;
                        return;
                    }

                    if (window.Contains(mx, my))
                    {
                        Focus(window);

                        if (window.TitleContains(mx, my))
                        {
                            window.StartDrag(mx, my);
                        }
                        else
                        {
                            window.CheckControlsClick(mx, my);
                        }

                        wasPressed = isPressed;
                        return;
                    }
                }

                ClearFocus();
            }

            if (isPressed)
            {
                int count = windows.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i >= windows.Count) break;
                    if (windows[i] != null && windows[i].Dragging)
                    {
                        windows[i].Drag(mx, my, canvas);
                    }
                }
            }
            else
            {
                int count = windows.Count;
                for (int i = 0; i < count; i++)
                {
                    if (i >= windows.Count) break;
                    if (windows[i] != null)
                    {
                        windows[i].StopDrag();
                    }
                }
            }

            wasPressed = isPressed;
        }

        public static void HandleKeyboard()
        {
            if (TaskSwitcherManager.IsActive)
            {
                TaskSwitcherManager.HandleKeyboard();
                return;
            }

            if (!KeyboardManager.TryReadKey(out KeyEvent keyEvent)) return;

            if (activeWindow == null || activeWindow.IsMinimized) return;

            int count = activeWindow.Controls.Count;
            for (int i = 0; i < count; i++)
            {
                var control = activeWindow.Controls[i];

                if (control is TextBox textBox && textBox.Focused)
                {
                    textBox.KeyPressed(keyEvent);
                    break;
                }

                if (control is RichTextBox richTextBox && richTextBox.Focused)
                {
                    richTextBox.KeyPressed(keyEvent);
                    break;
                }
            }
        }
    }
}