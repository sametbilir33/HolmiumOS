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

        public Color BarColor { get; set; } = Color.Green;
        public Color BackgroundColor { get; set; } = Color.LightGray;

        public ProgressBar(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(BackgroundColor, X, Y, Width, Height);

            int progressWidth = (int)((Width * Value) / 100.0);

            if (progressWidth > 0)
            {
                canvas.DrawFilledRectangle(BarColor, X, Y, progressWidth, Height);
            }

            canvas.DrawLine(Color.Black, X, Y, X + Width, Y);
            canvas.DrawLine(Color.Black, X, Y + Height, X + Width, Y + Height);
            canvas.DrawLine(Color.Black, X, Y, X, Y + Height);
            canvas.DrawLine(Color.Black, X + Width, Y, X + Width, Y + Height);
        }

        public override void Click() { this.Focused = true; }
    }
}