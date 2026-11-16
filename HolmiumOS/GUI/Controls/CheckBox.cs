
using System;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class CheckBox : Control
    {
        public string Text;
        public bool Checked { get; set; } = false;
        public Action<bool>? OnCheckedChanged;

        private const int BoxSize = 20;
        private const int CornerRadius = 5;

        private static readonly Color BackgroundColor = Color.FromArgb(246, 245, 244);
        private static readonly Color BorderColor = Color.FromArgb(160, 160, 160);
        private static readonly Color HoverColor = Color.FromArgb(235, 235, 235);
        private static readonly Color PressedColor = Color.FromArgb(222, 222, 220);
        private static readonly Color AccentColor = Color.FromArgb(53, 132, 228);
        private static readonly Color AccentHoverColor = Color.FromArgb(42, 112, 204);
        private static readonly Color White = Color.FromArgb(255, 255, 255);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);

        public CheckBox(string text, int x, int y)
            : base(x, y, 150, 32)
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
            if (!Visible || canvas == null)
                return;

            Color background = BackgroundColor;
            Color border = BorderColor;

            if (Pressed)
                background = PressedColor;
            else if (Hovered)
                background = HoverColor;

            if (Checked)
            {
                background = Hovered ? AccentHoverColor : AccentColor;
                border = background;
            }

            DrawRoundedRect(canvas, border, X, Y + 1, BoxSize, BoxSize, CornerRadius);
            DrawRoundedRect(canvas, background, X + 1, Y + 2, BoxSize - 2, BoxSize - 2, CornerRadius - 1);

            if (Checked)
            {
                canvas.DrawLine(White, X + 5, Y + 10, X + 8, Y + 13);
                canvas.DrawLine(White, X + 5, Y + 11, X + 8, Y + 14);
                canvas.DrawLine(White, X + 8, Y + 13, X + 15, Y + 6);
                canvas.DrawLine(White, X + 8, Y + 14, X + 15, Y + 7);
            }

            Font font = FontManager.UiFont;
            int textX = X + BoxSize + 10;
            int textY = Y + (Height - font.Height) / 2;

            canvas.DrawString(Text, font, TextColor, textX, textY);
        }

        public override void Click()
        {
            Focused = true;
            Checked = !Checked;
            OnCheckedChanged?.Invoke(Checked);
        }
    }
}
