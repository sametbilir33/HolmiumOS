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
        public Action<bool> OnCheckedChanged;

        private static readonly Color Win9xWhite = Color.FromArgb(255, 255, 255);
        private static readonly Color Win9xDarkGray = Color.FromArgb(128, 128, 128);
        private static readonly Color Win9xBlack = Color.FromArgb(0, 0, 0);
        private static readonly Color Win9xGray = Color.FromArgb(192, 192, 192);

        public CheckBox(string text, int x, int y)
            : base(x, y, 150, 32)
        {
            Text = text ?? "";
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            const int boxSize = 32;

            canvas.DrawFilledRectangle(
                Win9xWhite,
                X,
                Y,
                boxSize,
                boxSize);

            canvas.DrawLine(
                Win9xDarkGray,
                X,
                Y,
                X + boxSize - 1,
                Y);

            canvas.DrawLine(
                Win9xDarkGray,
                X,
                Y,
                X,
                Y + boxSize - 1);

            canvas.DrawLine(
                Win9xBlack,
                X + 1,
                Y + 1,
                X + boxSize - 2,
                Y + 1);

            canvas.DrawLine(
                Win9xBlack,
                X + 1,
                Y + 1,
                X + 1,
                Y + boxSize - 2);

            canvas.DrawLine(
                Win9xWhite,
                X,
                Y + boxSize - 1,
                X + boxSize,
                Y + boxSize - 1);

            canvas.DrawLine(
                Win9xWhite,
                X + boxSize - 1,
                Y,
                X + boxSize - 1,
                Y + boxSize);

            canvas.DrawLine(
                Win9xGray,
                X + 1,
                Y + boxSize - 2,
                X + boxSize - 2,
                Y + boxSize - 2);

            canvas.DrawLine(
                Win9xGray,
                X + boxSize - 2,
                Y + 1,
                X + boxSize - 2,
                Y + boxSize - 2);

            if (Checked)
            {
                Color checkColor = Win9xBlack;

                canvas.DrawLine(checkColor, X + 6, Y + 14, X + 12, Y + 22);
                canvas.DrawLine(checkColor, X + 6, Y + 15, X + 12, Y + 23);

                canvas.DrawLine(checkColor, X + 12, Y + 22, X + 24, Y + 8);
                canvas.DrawLine(checkColor, X + 12, Y + 23, X + 24, Y + 9);
            }

            Font font = PCScreenFont.DefaultFont;

            int textX = X + boxSize + 12;
            int textY = Y + (boxSize - font.Height) / 2;

            canvas.DrawString(
                Text,
                font,
                Win9xBlack,
                textX,
                textY);
        }

        public override void Click()
        {
            Focused = true;
            Checked = !Checked;

            if (OnCheckedChanged != null)
            {
                OnCheckedChanged.Invoke(Checked);
            }
        }
    }
}