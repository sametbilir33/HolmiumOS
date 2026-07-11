using System;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class Button : Control
    {
        public string Text;
        public Action OnClick;

        public Button(
            string text,
            int x,
            int y,
            int width,
            int height
        )
            : base(x, y, width, height)
        {
            Text = text;
        }


        public override void Draw(Canvas canvas)
        {
            if (!Visible)
                return;


            canvas.DrawFilledRectangle(
                Color.Gray,
                AbsoluteX,
                AbsoluteY,
                Width,
                Height
            );


            canvas.DrawString(
                Text,
                PCScreenFont.Default,
                Color.White,
                AbsoluteX + 5,
                AbsoluteY + 5
            );
        }


        public override void Click()
        {
            if (OnClick != null)
            {
                OnClick();
            }
        }
    }
}