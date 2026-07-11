using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class Label : Control
    {
        public string Text;


        public Label(
            string text,
            int x,
            int y
        )
            : base(x, y, 0, 0)
        {
            Text = text;
        }


        public override void Draw(Canvas canvas)
        {
            if (!Visible)
                return;


            canvas.DrawString(
                Text,
                PCScreenFont.Default,
                Color.Black,
                AbsoluteX,
                AbsoluteY
            );
        }
    }
}