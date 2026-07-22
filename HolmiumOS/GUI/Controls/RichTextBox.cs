using System;
using Cosmos.System;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class RichTextBox : Control
    {
        public string Text = "";
        public int MaxLength = 2000;

        private const int CHAR_WIDTH = 8;
        private const int CHAR_HEIGHT = 16;
        private const int PADDING = 5;

        public RichTextBox(int x, int y, int width, int height) : base(x, y, width, height)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(System.Drawing.Color.White, X, Y, Width, Height);

            System.Drawing.Color borderColor = Focused ? System.Drawing.Color.Blue : System.Drawing.Color.Black;
            canvas.DrawLine(borderColor, X, Y, X + Width, Y);
            canvas.DrawLine(borderColor, X, Y + Height, X + Width, Y + Height);
            canvas.DrawLine(borderColor, X, Y, X, Y + Height);
            canvas.DrawLine(borderColor, X + Width, Y, X + Width, Y + Height);

            if (Text == null) Text = "";

            int currX = X + PADDING;
            int currY = Y + PADDING;

            int cursorX = currX;
            int cursorY = currY;

            int maxRight = X + Width - PADDING - CHAR_WIDTH;
            int maxBottom = Y + Height - PADDING - CHAR_HEIGHT;

            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];

                if (c == '\n')
                {
                    currX = X + PADDING;
                    currY += CHAR_HEIGHT;
                    if (currY > maxBottom) break;
                    continue;
                }

                if (currX > maxRight)
                {
                    currX = X + PADDING;
                    currY += CHAR_HEIGHT;
                    if (currY > maxBottom) break;
                }

                canvas.DrawString(c.ToString(), PCScreenFont.Default, System.Drawing.Color.Black, currX, currY);
                currX += CHAR_WIDTH;
            }

            cursorX = currX;
            cursorY = currY;

            if (Focused && (Cosmos.HAL.RTC.Second % 2) == 0)
            {
                if (cursorY <= maxBottom && cursorX <= maxRight + CHAR_WIDTH)
                {
                    canvas.DrawLine(System.Drawing.Color.Black, cursorX, cursorY + 1, cursorX, cursorY + CHAR_HEIGHT - 2);
                }
            }
        }

        public override void Click()
        {
            this.Focused = true;
        }

        public void MouseClick(int mouseX, int mouseY)
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
            else if (keyEvent.Key == ConsoleKeyEx.Enter)
            {
                if (Text.Length < MaxLength)
                {
                    Text += "\n";
                }
            }
            else if (keyEvent.KeyChar != '\0' && Text.Length < MaxLength)
            {
                Text += keyEvent.KeyChar;
            }
        }
    }
}