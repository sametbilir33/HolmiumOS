using System.Collections.Generic;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class WindowManager
    {
        private static List<Window> windows =
            new List<Window>();

        private static Window activeWindow;

        public static void Add(Window window)
        {
            windows.Add(window);
            Focus(window);
        }

        public static void Remove(Window window)
        {
            windows.Remove(window);
        }

        public static void Draw(Canvas canvas)
        {
            for (int i = 0; i < windows.Count; i++)
            {
                windows[i].Draw(canvas);
            }
        }

        private static void Focus(Window window)
        {
            if (activeWindow != null)
            {
                activeWindow.Active = false;
            }


            windows.Remove(window);

            windows.Add(window);


            activeWindow = window;

            activeWindow.Active = true;
        }

        public static void UpdateMouse()
        {
            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;


            if (MouseManager.MouseState == MouseState.Left)
            {
                for (int i = windows.Count - 1; i >= 0; i--)
                {
                    Window window = windows[i];


                    if (window.CloseContains(mx, my))
                    {
                        Remove(window);
                        return;
                    }


                    if (window.Contains(mx, my))
                    {
                        Focus(window);


                        if (window.TitleContains(mx, my))
                        {
                            window.StartDrag(mx, my);
                            return;
                        }


                        window.CheckControlsClick(
                            mx,
                            my
                        );


                        return;
                    }
                }
            }

            foreach (Window window in windows)
            {
                window.Drag(mx, my);
            }

            if (MouseManager.MouseState != MouseState.Left)
            {
                foreach (Window window in windows)
                {
                    window.StopDrag();
                }
            }
        }
    }
}