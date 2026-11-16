
using System;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;

namespace HolmiumOS.GUI.Controls
{
    public class ProgressBar : Control
    {
        private int _value = 0;

        public int Value
        {
            get => _value;
            set
            {
                if (value >= 0 && value <= 100)
                    _value = value;
            }
        }

        public Color BarColor { get; set; } = Color.FromArgb(53, 132, 228);
        public Color BackgroundColor { get; set; } = Color.FromArgb(222, 221, 218);

        private const int CornerRadius = 6;

        public ProgressBar(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
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

            DrawRoundedRect(canvas, BackgroundColor, X, Y, Width, Height, CornerRadius);

            int innerPadding = 2;
            int innerWidth = Width - innerPadding * 2;
            int innerHeight = Height - innerPadding * 2;

            if (innerWidth <= 0 || innerHeight <= 0 || Value <= 0)
                return;

            int progressWidth = innerWidth * Value / 100;

            if (progressWidth <= 0)
                return;

            DrawRoundedRect(
                canvas,
                BarColor,
                X + innerPadding,
                Y + innerPadding,
                progressWidth,
                innerHeight,
                Math.Min(CornerRadius - 2, innerHeight / 2));
        }

        public override void Click()
        {
            Focused = true;
        }
    }
}
