using System;
using Cosmos.System;
using Cosmos.System.Graphics;

namespace HolmiumOS.GUI.Controls
{
    public class TextBox : Control
    {
        public string Text = "";
        public int MaxLength = 30;

        public TextBox(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible) return;

            canvas.DrawFilledRectangle(System.Drawing.Color.White, X, Y, Width, Height);

            System.Drawing.Color borderColor = Focused ? System.Drawing.Color.Blue : System.Drawing.Color.Black;
            canvas.DrawLine(borderColor, X, Y, X + Width, Y);
            canvas.DrawLine(borderColor, X, Y + Height, X + Width, Y + Height);
            canvas.DrawLine(borderColor, X, Y, X, Y + Height);
            canvas.DrawLine(borderColor, X + Width, Y, X + Width, Y + Height);

            canvas.DrawString(Text, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, System.Drawing.Color.Black, X + 5, Y + (Height / 2) - 8);

            if (Focused)
            {
                int textWidth = Text.Length * 8;
                int cursorX = X + 5 + textWidth;

                if (cursorX < X + Width - 10)
                {
                    canvas.DrawLine(System.Drawing.Color.Black, cursorX, Y + 4, cursorX, Y + Height - 4);
                }
            }
        }

        public override void Click()
        {
            this.Focused = true;
        }

        public void KeyPressed(KeyEvent keyEvent)
        {
            if (!Focused) return;

            if (keyEvent.Key == ConsoleKeyEx.Backspace)
            {
                if (Text.Length > 0)
                {
                    Text = Text.Substring(0, Text.Length - 1);
                }
            }
            else if (keyEvent.KeyChar != '\0' && Text.Length < MaxLength)
            {
                if (keyEvent.Key == ConsoleKeyEx.Enter) return;

                Text += keyEvent.KeyChar;
            }
        }
    }
}