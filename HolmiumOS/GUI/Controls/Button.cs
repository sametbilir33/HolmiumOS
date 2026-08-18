using System;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class Button : Control
    {
        public string Text;
        public Action ClickAction;

        private static readonly Color Win9xGray = Color.FromArgb(192, 192, 192);
        private static readonly Color Win9xWhite = Color.FromArgb(255, 255, 255);
        private static readonly Color Win9xDarkGray = Color.FromArgb(128, 128, 128);
        private static readonly Color Win9xBlack = Color.FromArgb(0, 0, 0);

        public Button(string text, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Text = text ?? "";
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(Win9xGray, X, Y, Width, Height);


            canvas.DrawLine(Win9xWhite, X, Y, X + Width - 1, Y);
            canvas.DrawLine(Win9xWhite, X, Y, X, Y + Height - 1);

            canvas.DrawLine(Win9xDarkGray, X + 1, Y + 1, X + Width - 2, Y + 1);
            canvas.DrawLine(Win9xDarkGray, X + 1, Y + 1, X + 1, Y + Height - 2);

            canvas.DrawLine(Win9xBlack, X, Y + Height - 1, X + Width, Y + Height - 1);
            canvas.DrawLine(Win9xBlack, X + Width - 1, Y, X + Width - 1, Y + Height);

            canvas.DrawLine(Win9xDarkGray, X + 1, Y + Height - 2, X + Width - 2, Y + Height - 2);
            canvas.DrawLine(Win9xDarkGray, X + Width - 2, Y + 1, X + Width - 2, Y + Height - 2);

            int textX = X + 8;
            int textY = Y + (Height / 2) - 4;

            canvas.DrawString(Text, PCScreenFont.Default, Win9xBlack, textX, textY);
        }

        public override void Click()
        {
            if (ClickAction != null)
            {
                ClickAction.Invoke();
            }
        }
    }
}