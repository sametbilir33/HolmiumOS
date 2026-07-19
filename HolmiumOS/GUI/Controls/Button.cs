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

        public Button(string text, int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Text = text ?? "";
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(Color.Gray, X, Y, Width, Height);

            canvas.DrawString(Text, PCScreenFont.Default, Color.White, X + 5, Y + 8);
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