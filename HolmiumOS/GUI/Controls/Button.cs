
using System;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class Button : Control
    {
        public string Text;
        public Action? ClickAction;

        private const int CornerRadius = 8;

        private static readonly Color BackgroundColor = Color.FromArgb(246, 245, 244);
        private static readonly Color BorderColor = Color.FromArgb(199, 199, 199);
        private static readonly Color HoverColor = Color.FromArgb(235, 235, 235);
        private static readonly Color HoverBorderColor = Color.FromArgb(169, 169, 169);
        private static readonly Color PressedColor = Color.FromArgb(222, 222, 220);
        private static readonly Color PressedBorderColor = Color.FromArgb(153, 153, 153);
        private static readonly Color ShadowColor = Color.FromArgb(35, 0, 0, 0);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);

        public Button(string text, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Text = text ?? "";
        }

        private static void DrawRoundedRect(Canvas canvas, Color color, int x, int y, int width, int height, int radius)
        {
            if (width <= 0 || height <= 0)
                return;

            radius = Math.Min(radius, Math.Min(width, height) / 2);

            if (radius <= 0)
            {
                canvas.DrawFilledRectangle(color, x, y, width, height);
                return;
            }

            int centerWidth = width - radius * 2;
            int centerHeight = height - radius * 2;

            if (centerWidth > 0)
                canvas.DrawFilledRectangle(color, x + radius, y, centerWidth, height);

            if (centerHeight > 0)
                canvas.DrawFilledRectangle(color, x, y + radius, width, centerHeight);

            canvas.DrawFilledCircle(color, x + radius, y + radius, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
            canvas.DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null || Width <= 0 || Height <= 0)
                return;

            Color background = BackgroundColor;
            Color border = BorderColor;

            if (Pressed)
            {
                background = PressedColor;
                border = PressedBorderColor;
            }
            else if (Hovered)
            {
                background = HoverColor;
                border = HoverBorderColor;
            }

            DrawRoundedRect(canvas, ShadowColor, X, Y + 2, Width, Height, CornerRadius);
            DrawRoundedRect(canvas, border, X, Y, Width, Height, CornerRadius);
            DrawRoundedRect(canvas, background, X + 1, Y + 1, Width - 2, Height - 2, CornerRadius - 1);

            Font font = FontManager.UiFont;
            int fontSize = FontManager.UiFontSize;

            int textWidth;
            int textHeight;
            int textX;
            int textY;

            if (font is TrueTypeFont trueTypeFont)
            {
                textWidth = trueTypeFont.MeasureString(Text, fontSize);
                textHeight = trueTypeFont.GetLineHeight(fontSize);

                textX = X + (Width - textWidth) / 2;
                textY = Y + (Height - textHeight) / 2;

                canvas.DrawString(Text, trueTypeFont, fontSize, TextColor, textX, textY);
            }
            else
            {
                textWidth = Text.Length * font.Width;
                textHeight = font.Height;

                textX = X + (Width - textWidth) / 2;
                textY = Y + (Height - textHeight) / 2;

                canvas.DrawString(Text, font, TextColor, textX, textY);
            }
        }

        public override void Click()
        {
            ClickAction?.Invoke();
        }
    }
}
