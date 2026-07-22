using System.Collections.Generic;
using System.Drawing;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI
{
    public static class NotificationManager
    {
        private static List<Notification> notifications = new List<Notification>();

        private const int Width = 220;
        private const int Height = 60;
        private const int Padding = 10;
        private static bool lastPressed = false;

        public static void Send(string title, string message, NotificationType type = NotificationType.Info, int durationSeconds = 5)
        {
            notifications.Add(new Notification(title, message, type, durationSeconds));
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

                Color accentColor = notif.Type switch
                {
                    NotificationType.Success => Color.FromArgb(40, 167, 69),
                    NotificationType.Warning => Color.FromArgb(255, 193, 7),
                    NotificationType.Error => Color.FromArgb(220, 53, 69),
                    _ => Color.FromArgb(0, 122, 204)
                };

                canvas.DrawFilledRectangle(Color.FromArgb(30, 30, 30), x, y, Width, Height);
                canvas.DrawFilledRectangle(accentColor, x, y, 4, Height);

                canvas.DrawLine(Color.FromArgb(60, 60, 60), x, y, x + Width, y);
                canvas.DrawLine(Color.FromArgb(60, 60, 60), x, y + Height, x + Width, y + Height);
                canvas.DrawLine(Color.FromArgb(60, 60, 60), x + Width, y, x + Width, y + Height);

                string titleLine1 = notif.Title;
                string titleLine2 = "";
                if (titleLine1.Length > 22)
                {
                    titleLine2 = titleLine1.Substring(22);
                    titleLine1 = titleLine1.Substring(0, 22);
                    if (titleLine2.Length > 22) titleLine2 = titleLine2.Substring(0, 20) + "..";
                }

                string msgLine1 = notif.Message;
                string msgLine2 = "";
                if (msgLine1.Length > 26)
                {
                    msgLine2 = msgLine1.Substring(26);
                    msgLine1 = msgLine1.Substring(0, 26);
                    if (msgLine2.Length > 26) msgLine2 = msgLine2.Substring(0, 24) + "..";
                }

                canvas.DrawString(titleLine1, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, x + 12, y + 6);
                if (!string.IsNullOrEmpty(titleLine2))
                    canvas.DrawString(titleLine2, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.White, x + 12, y + 18);

                canvas.DrawString(msgLine1, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.LightGray, x + 12, y + 30);
                if (!string.IsNullOrEmpty(msgLine2))
                    canvas.DrawString(msgLine2, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.LightGray, x + 12, y + 42);
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