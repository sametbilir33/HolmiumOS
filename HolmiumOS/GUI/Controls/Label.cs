
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class Label : Control
    {
        public string Text;

        public Label(string text, int x, int y)
            : base(x, y, (text ?? "").Length * FontManager.UiFontSize, FontManager.UiFontSize + 8)
        {
            Text = text ?? "";
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null)
                return;

            Font font = FontManager.UiFont;

            if (font is TrueTypeFont trueTypeFont)
            {
                canvas.DrawString(Text, trueTypeFont, FontManager.UiFontSize, Color.FromArgb(36, 36, 36), X, Y);
            }
            else
            {
                canvas.DrawString(Text, font, Color.FromArgb(36, 36, 36), X, Y);
            }
        }

        public override void Click() { }
    }
}
