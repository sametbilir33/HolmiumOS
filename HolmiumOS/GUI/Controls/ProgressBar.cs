using System.Drawing;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI.Controls
{
    public class ProgressBar : Control
    {
        private int _value = 0;
        public int Value
        {
            get => _value;
            set { if (value >= 0 && value <= 100) _value = value; }
        }

        public Color BarColor { get; set; } = Color.FromArgb(0, 0, 128);
        public Color BackgroundColor { get; set; } = Color.FromArgb(192, 192, 192);

        private static readonly Color Win9xDarkGray = Color.FromArgb(128, 128, 128);
        private static readonly Color Win9xBlack = Color.FromArgb(0, 0, 0);
        private static readonly Color Win9xWhite = Color.FromArgb(255, 255, 255);
        private static readonly Color Win9xGray = Color.FromArgb(192, 192, 192);

        public ProgressBar(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(BackgroundColor, X, Y, Width, Height);

            int innerWidth = Width - 4;
            int innerHeight = Height - 4;
            int progressWidth = (int)((innerWidth * Value) / 100.0);

            if (progressWidth > 0)
            {
                canvas.DrawFilledRectangle(BarColor, X + 2, Y + 2, progressWidth, innerHeight);
            }

            canvas.DrawLine(Win9xDarkGray, X, Y, X + Width - 1, Y);
            canvas.DrawLine(Win9xDarkGray, X, Y, X, Y + Height - 1);

            canvas.DrawLine(Win9xBlack, X + 1, Y + 1, X + Width - 2, Y + 1);
            canvas.DrawLine(Win9xBlack, X + 1, Y + 1, X + 1, Y + Height - 2);

            canvas.DrawLine(Win9xWhite, X, Y + Height, X + Width, Y + Height);
            canvas.DrawLine(Win9xWhite, X + Width, Y, X + Width, Y + Height);

            canvas.DrawLine(Win9xGray, X + 1, Y + Height - 1, X + Width - 1, Y + Height - 1);
            canvas.DrawLine(Win9xGray, X + Width - 1, Y + 1, X + Width - 1, Y + Height - 1);
        }

        public override void Click()
        {
            this.Focused = true;
        }
    }
}