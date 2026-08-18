using System;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

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
            : base(x, y, 150, 16)
        {
            Text = text ?? "";
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(Win9xWhite, X, Y, 16, 16);

            canvas.DrawLine(Win9xDarkGray, X, Y, X + 15, Y);
            canvas.DrawLine(Win9xDarkGray, X, Y, X, Y + 15);

            canvas.DrawLine(Win9xBlack, X + 1, Y + 1, X + 14, Y + 1);
            canvas.DrawLine(Win9xBlack, X + 1, Y + 1, X + 1, Y + 14);

            canvas.DrawLine(Win9xWhite, X, Y + 16, X + 16, Y + 16);
            canvas.DrawLine(Win9xWhite, X + 16, Y, X + 16, Y + 16);

            canvas.DrawLine(Win9xGray, X + 1, Y + 15, X + 15, Y + 15);
            canvas.DrawLine(Win9xGray, X + 15, Y + 1, X + 15, Y + 15);

            if (Checked)
            {
                Color checkColor = Win9xBlack;

                canvas.DrawLine(checkColor, X + 3, Y + 7, X + 6, Y + 11);
                canvas.DrawLine(checkColor, X + 3, Y + 8, X + 6, Y + 12);
                canvas.DrawLine(checkColor, X + 6, Y + 11, X + 12, Y + 4);
                canvas.DrawLine(checkColor, X + 6, Y + 12, X + 12, Y + 5);
            }

            canvas.DrawString(Text, PCScreenFont.Default, Win9xBlack, X + 22, Y + 1);
        }

        public override void Click()
        {
            this.Focused = true;
            this.Checked = !this.Checked;

            if (OnCheckedChanged != null)
            {
                OnCheckedChanged.Invoke(this.Checked);
            }
        }
    }
}