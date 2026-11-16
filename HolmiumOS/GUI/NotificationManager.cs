
using System.Collections.Generic;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using Cosmos.Kernel.System.Input;

namespace HolmiumOS.GUI
{
    public static class NotificationManager
    {
        private static readonly List<Notification> notifications = new List<Notification>();

        private const int Width = 320;
        private const int Height = 104;
        private const int Padding = 12;
        private const int HeaderHeight = 30;
        private const int MessagePadding = 12;

        private static bool lastPressed = false;

        private static readonly Color BackgroundColor = Color.FromArgb(255, 255, 255);
        private static readonly Color BorderColor = Color.FromArgb(215, 213, 209);
        private static readonly Color ShadowColor = Color.FromArgb(45, 0, 0, 0);
        private static readonly Color TitleColor = Color.FromArgb(36, 36, 36);
        private static readonly Color MessageColor = Color.FromArgb(75, 75, 75);
        private static readonly Color CloseColor = Color.FromArgb(125, 125, 125);
        private static readonly Color CloseHoverColor = Color.FromArgb(210, 70, 70);

        public static void Send(string title, string message, NotificationType type = NotificationType.Info, int durationSeconds = 5)
        {
            notifications.Add(new Notification(title, message, type, durationSeconds));
        }

        private static int MeasureText(Font font, string text)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.MeasureString(text, FontManager.UiFontSize);

            return font.MeasureString(text);
        }

        private static int GetFontHeight(Font font)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.GetLineHeight(FontManager.UiFontSize);

            return font.Height;
        }

        private static void DrawText(Canvas canvas, string text, Font font, Color color, int x, int y)
        {
            if (font is TrueTypeFont trueTypeFont)
                canvas.DrawString(text, trueTypeFont, FontManager.UiFontSize, color, x, y);
            else
                canvas.DrawString(text, font, color, x, y);
        }

        private static void FillRoundedRect(Canvas canvas, int x, int y, int width, int height, int radius, Color color)
        {
            if (width <= 0 || height <= 0)
                return;

            radius = System.Math.Min(radius, System.Math.Min(width / 2, height / 2));

            if (radius <= 0)
            {
                canvas.DrawFilledRectangle(color, x, y, width, height);
                return;
            }

            canvas.DrawFilledRectangle(color, x + radius, y, width - radius * 2, height);
            canvas.DrawFilledRectangle(color, x, y + radius, width, height - radius * 2);
            canvas.DrawFilledCircle(color, x + radius, y + radius, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
            canvas.DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        private static void DrawRoundedOutline(Canvas canvas, int x, int y, int width, int height, int radius, Color color)
        {
            if (width <= 0 || height <= 0)
                return;

            radius = System.Math.Min(radius, System.Math.Min(width / 2, height / 2));

            canvas.DrawLine(color, x + radius, y, x + width - radius - 1, y);
            canvas.DrawLine(color, x + radius, y + height - 1, x + width - radius - 1, y + height - 1);
            canvas.DrawLine(color, x, y + radius, x, y + height - radius - 1);
            canvas.DrawLine(color, x + width - 1, y + radius, x + width - 1, y + height - radius - 1);
        }

        private static Color GetTypeColor(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.Success:
                    return Color.FromArgb(38, 162, 105);

                case NotificationType.Warning:
                    return Color.FromArgb(229, 165, 10);

                case NotificationType.Error:
                    return Color.FromArgb(224, 27, 36);

                default:
                    return Color.FromArgb(53, 132, 228);
            }
        }

        private static string FitText(string text, Font font, int maxWidth)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            if (MeasureText(font, text) <= maxWidth)
                return text;

            string result = text;

            while (result.Length > 1 && MeasureText(font, result + "...") > maxWidth)
                result = result.Substring(0, result.Length - 1);

            return result + "...";
        }

        private static string[] WrapMessage(string message, Font font, int maxWidth)
        {
            string[] lines = new string[2];

            if (string.IsNullOrEmpty(message))
                return lines;

            message = message.Replace('\n', ' ').Trim();

            if (MeasureText(font, message) <= maxWidth)
            {
                lines[0] = message;
                return lines;
            }

            int splitIndex = 0;

            while (splitIndex < message.Length && MeasureText(font, message.Substring(0, splitIndex + 1)) <= maxWidth)
                splitIndex++;

            if (splitIndex == 0)
                splitIndex = 1;

            int spaceIndex = message.LastIndexOf(' ', splitIndex - 1);

            if (spaceIndex > 0)
                splitIndex = spaceIndex;

            lines[0] = message.Substring(0, splitIndex).Trim();
            string remaining = message.Substring(splitIndex).Trim();

            if (remaining.Length > 0)
                lines[1] = FitText(remaining, font, maxWidth);

            return lines;
        }

        private static void DrawCloseIcon(Canvas canvas, int x, int y, Color color)
        {
            canvas.DrawLine(color, x, y, x + 8, y + 8);
            canvas.DrawLine(color, x + 8, y, x, y + 8);
        }

        public static void Draw(Canvas canvas)
        {
            if (notifications.Count == 0)
                return;

            Font font = FontManager.UiFont;
            int fontHeight = GetFontHeight(font);
            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;
            int startY = screenHeight - Taskbar.Height - Padding;
            int maxTextWidth = Width - MessagePadding * 2;

            for (int i = 0; i < notifications.Count; i++)
            {
                Notification notif = notifications[i];
                notif.UpdateTimer();

                if (notif.IsExpired)
                {
                    notifications.RemoveAt(i);
                    i--;
                    continue;
                }

                int x = screenWidth - Width - Padding;
                int y = startY - (i + 1) * (Height + Padding);

                FillRoundedRect(canvas, x + 2, y + 3, Width, Height, 9, ShadowColor);
                FillRoundedRect(canvas, x, y, Width, Height, 9, BackgroundColor);
                DrawRoundedOutline(canvas, x, y, Width, Height, 9, BorderColor);

                Color typeColor = GetTypeColor(notif.Type);
                FillRoundedRect(canvas, x + 1, y + 12, 4, Height - 24, 2, typeColor);

                string title = FitText(notif.Title ?? "", font, Width - 65);
                int titleY = y + (HeaderHeight - fontHeight) / 2 + 2;

                DrawText(canvas, title, font, TitleColor, x + MessagePadding, titleY);

                DrawCloseIcon(canvas, x + Width - 23, y + 11, CloseColor);

                string[] lines = WrapMessage(notif.Message ?? "", font, maxTextWidth);
                int messageStartY = y + HeaderHeight + 7;

                if (!string.IsNullOrEmpty(lines[0]))
                    DrawText(canvas, lines[0], font, MessageColor, x + MessagePadding, messageStartY);

                if (!string.IsNullOrEmpty(lines[1]))
                    DrawText(canvas, lines[1], font, MessageColor, x + MessagePadding, messageStartY + fontHeight + 3);
            }
        }

        public static void UpdateMouse(Canvas canvas)
        {
            bool isPressed = MouseManager.LeftButton;

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
                    int y = startY - (i + 1) * (Height + Padding);

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
