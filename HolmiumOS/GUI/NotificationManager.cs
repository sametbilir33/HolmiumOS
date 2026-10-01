using System.Collections.Generic;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using Cosmos.Kernel.System.Mouse;

namespace HolmiumOS.GUI
{
    public static class NotificationManager
    {
        private static List<Notification> notifications =
            new List<Notification>();

        private const int Width = 300;
        private const int Height = 100;
        private const int Padding = 10;

        private const int HeaderHeight = 32;
        private const int MessagePadding = 8;

        private static bool lastPressed = false;

        public static void Send(
            string title,
            string message,
            NotificationType type = NotificationType.Info,
            int durationSeconds = 5)
        {
            notifications.Add(
                new Notification(
                    title,
                    message,
                    type,
                    durationSeconds));
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

        private static string FitText(
            string text,
            Font font,
            int maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            if (font.MeasureString(text) <= maxWidth)
                return text;

            string result = text;

            while (result.Length > 2 &&
                   font.MeasureString(result + "..") > maxWidth)
            {
                result =
                    result.Substring(
                        0,
                        result.Length - 1);
            }

            return result + "..";
        }

        public static void Draw(Canvas canvas)
        {
            if (notifications.Count == 0)
                return;

            Font font =
                PCScreenFont.DefaultFont;

            int screenWidth =
                (int)canvas.Mode.Width;

            int screenHeight =
                (int)canvas.Mode.Height;

            int startY =
                screenHeight -
                Taskbar.Height -
                Padding;

            int maxTextWidth =
                Width - (MessagePadding * 2);

            for (int i = 0;
                 i < notifications.Count;
                 i++)
            {
                var notif =
                    notifications[i];

                notif.UpdateTimer();

                if (notif.IsExpired)
                {
                    notifications.RemoveAt(i);
                    i--;
                    continue;
                }

                int x =
                    screenWidth -
                    Width -
                    Padding;

                int y =
                    startY -
                    ((i + 1) *
                    (Height + Padding));

                DrawRaisedBox(
                    canvas,
                    x,
                    y,
                    Width,
                    Height);

                canvas.DrawFilledRectangle(
                    Color.FromArgb(0, 0, 128),
                    x + 3,
                    y + 3,
                    Width - 6,
                    HeaderHeight);

                Color typeIndicator =
                    notif.Type switch
                    {
                        NotificationType.Success =>
                            Color.FromArgb(
                                0,
                                200,
                                0),

                        NotificationType.Warning =>
                            Color.FromArgb(
                                220,
                                220,
                                0),

                        NotificationType.Error =>
                            Color.FromArgb(
                                220,
                                0,
                                0),

                        _ =>
                            Color.FromArgb(
                                0,
                                128,
                                255)
                    };

                canvas.DrawFilledRectangle(
                    typeIndicator,
                    x + 7,
                    y + 7,
                    18,
                    18);

                int titleMaxWidth =
                    Width - 38;

                string title =
                    FitText(
                        notif.Title,
                        font,
                        titleMaxWidth);

                int titleY =
                    y +
                    3 +
                    (HeaderHeight - font.Height) / 2;

                canvas.DrawString(
                    title,
                    font,
                    Color.White,
                    x + 32,
                    titleY);

                string message =
                    notif.Message ?? "";

                string msgLine1 = message;
                string msgLine2 = "";

                if (font.MeasureString(msgLine1) >
                    maxTextWidth)
                {
                    int splitIndex =
                        msgLine1.Length;

                    while (splitIndex > 0 &&
                           font.MeasureString(
                               msgLine1.Substring(
                                   0,
                                   splitIndex)) >
                               maxTextWidth)
                    {
                        splitIndex--;
                    }

                    if (splitIndex > 0)
                    {
                        msgLine2 =
                            msgLine1.Substring(
                                splitIndex).Trim();

                        msgLine1 =
                            msgLine1.Substring(
                                0,
                                splitIndex).Trim();
                    }
                }

                msgLine1 =
                    FitText(
                        msgLine1,
                        font,
                        maxTextWidth);

                if (!string.IsNullOrEmpty(msgLine2))
                {
                    msgLine2 =
                        FitText(
                            msgLine2,
                            font,
                            maxTextWidth);
                }

                int messageStartY =
                    y +
                    HeaderHeight +
                    5;

                canvas.DrawString(
                    msgLine1,
                    font,
                    Color.Black,
                    x + MessagePadding,
                    messageStartY);

                if (!string.IsNullOrEmpty(msgLine2))
                {
                    canvas.DrawString(
                        msgLine2,
                        font,
                        Color.Black,
                        x + MessagePadding,
                        messageStartY + font.Height);
                }
            }
        }

        public static void UpdateMouse(
            Canvas canvas)
        {
            bool isPressed =
                MouseManager.LeftButton;

            if (isPressed &&
                !lastPressed &&
                notifications.Count > 0)
            {
                int mx =
                    (int)MouseManager.X;

                int my =
                    (int)MouseManager.Y;

                int screenWidth =
                    (int)canvas.Mode.Width;

                int screenHeight =
                    (int)canvas.Mode.Height;

                int startY =
                    screenHeight -
                    Taskbar.Height -
                    Padding;

                for (int i = 0;
                     i < notifications.Count;
                     i++)
                {
                    int x =
                        screenWidth -
                        Width -
                        Padding;

                    int y =
                        startY -
                        ((i + 1) *
                        (Height + Padding));

                    if (mx >= x &&
                        mx <= x + Width &&
                        my >= y &&
                        my <= y + Height)
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