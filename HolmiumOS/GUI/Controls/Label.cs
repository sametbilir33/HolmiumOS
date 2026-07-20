using System.Drawing;
using Cosmos.System.Graphics;

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
            canvas.DrawString(Text, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, Color.Black, X, Y);
        }

        public override void Click() { }
    }
}