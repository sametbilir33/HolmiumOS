using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class Label : Control
    {
        public string Text;

        public Label(string text, int x, int y)
            : base(x, y, (text ?? "").Length * 8, 16)
        {
            Text = text ?? "";
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;
            canvas.DrawString(Text, PCScreenFont.DefaultFont, Color.Black, X, Y);
        }

        public override void Click() { }
    }
}