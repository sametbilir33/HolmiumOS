using System.Collections.Generic;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class NotificationManager
    {
        private static List<Notification> notifications = new List<Notification>();

        private const int Width = 230;
        private const int Height = 65;
        private const int Padding = 10;
        private static bool lastPressed = false;

        public static void Send(string title, string message, NotificationType type = NotificationType.Info, int durationSeconds = 5)
        {
            notifications.Add(new Notification(title, message, type, durationSeconds));
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

        public static void Draw(Canvas canvas)
        {
            if (notifications.Count == 0) return;

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;
            int startY = screenHeight - Taskbar.Height - Padding;

            for (int i = 0; i < notifications.Count; i++)
            {
                var notif = notifications[i];
                notif.UpdateTimer();

                if (notif.IsExpired)
                {
                    notifications.RemoveAt(i);
                    i--;
                    continue;
                }

                int x = screenWidth - Width - Padding;
                int y = startY - ((i + 1) * (Height + Padding));

                DrawRaisedBox(canvas, x, y, Width, Height);

                canvas.DrawFilledRectangle(Color.FromArgb(0, 0, 128), x + 3, y + 3, Width - 6, 16);

                Color typeIndicator = notif.Type switch
                {
                    NotificationType.Success => Color.FromArgb(0, 200, 0),
                    NotificationType.Warning => Color.FromArgb(220, 220, 0),
                    NotificationType.Error => Color.FromArgb(220, 0, 0),
                    _ => Color.FromArgb(0, 128, 255)
                };
                canvas.DrawFilledRectangle(typeIndicator, x + 6, y + 6, 10, 10);

                string titleLine1 = notif.Title;
                if (titleLine1.Length > 20)
                    titleLine1 = titleLine1.Substring(0, 18) + "..";

                string msgLine1 = notif.Message;
                string msgLine2 = "";

                if (msgLine1.Length > 28)
                {
                    msgLine2 = msgLine1.Substring(28);
                    msgLine1 = msgLine1.Substring(0, 28);

                    if (msgLine2.Length > 28)
                        msgLine2 = msgLine2.Substring(0, 26) + "..";
                }

                canvas.DrawString(titleLine1, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, x + 20, y + 5);
                canvas.DrawString(msgLine1, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.Black, x + 8, y + 25);

                if (!string.IsNullOrEmpty(msgLine2))
                    canvas.DrawString(msgLine2, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.Black, x + 8, y + 42);
            }
        }

        public static void UpdateMouse(Canvas canvas)
        {
            bool isPressed = (MouseManager.MouseState & MouseState.Left) != 0;

            if (isPressed && !lastPressed && notifications.Count > 0)
            {
                int mx = (int)MouseManager.X;
                int my = (int)MouseManager.Y;
                int screenWidth = (int)canvas.Mode.Width;
                int screenHeight = (int)canvas.Mode.Height;
                int startY = screenHeight - Taskbar.Height - Padding;

                for (int i = 0; i < notifications.Count; i++)
                {
                    int x = screenWidth - Width - Padding;
                    int y = startY - ((i + 1) * (Height + Padding));

                    if (mx >= x && mx <= x + Width && my >= y && my <= y + Height)
                    {
                        notifications.RemoveAt(i);
                        break;
                    }
                }
            }

            lastPressed = isPressed;
        }
    }
}