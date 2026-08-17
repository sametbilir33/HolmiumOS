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
                if (StatusBar.ContainsClock(mx, my, canvas))
                {
                    AppManager.Run<Apps.CalendarClock>(50, 50);
                    wasPressed = isPressed;
                    return;
                }

                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    if (i >= windows.Count) break;

                    Window window = windows[i];
                    if (window == null) continue;

                    if (window.CloseContains(mx, my))
                    {
                        Remove(window);
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
            if (!KeyboardManager.TryReadKey(out KeyEvent keyEvent)) return;
            if (activeWindow == null) return;

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