using System;
using Cosmos.Kernel.System.Keyboard;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class RichTextBox : Control
    {
        public string Text = "";
        public int MaxLength = 2000;

        private const int PADDING = 5;

        private static readonly Color Win9xWhite = System.Drawing.Color.FromArgb(255, 255, 255);
        private static readonly Color Win9xDarkGray = System.Drawing.Color.FromArgb(128, 128, 128);
        private static readonly Color Win9xBlack = System.Drawing.Color.FromArgb(0, 0, 0);
        private static readonly Color Win9xGray = System.Drawing.Color.FromArgb(192, 192, 192);

        public RichTextBox(
            int x,
            int y,
            int width,
            int height)
            : base(x, y, width, height)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null)
                return;

            Font font = PCScreenFont.DefaultFont;

            int charWidth = font.Width;
            int charHeight = font.Height;

            canvas.DrawFilledRectangle(
                Win9xWhite,
                X,
                Y,
                Width,
                Height);

            canvas.DrawLine(
                Win9xDarkGray,
                X,
                Y,
                X + Width - 1,
                Y);

            canvas.DrawLine(
                Win9xDarkGray,
                X,
                Y,
                X,
                Y + Height - 1);

            canvas.DrawLine(
                Win9xBlack,
                X + 1,
                Y + 1,
                X + Width - 2,
                Y + 1);

            canvas.DrawLine(
                Win9xBlack,
                X + 1,
                Y + 1,
                X + 1,
                Y + Height - 2);

            canvas.DrawLine(
                Win9xWhite,
                X,
                Y + Height,
                X + Width,
                Y + Height);

            canvas.DrawLine(
                Win9xWhite,
                X + Width,
                Y,
                X + Width,
                Y + Height);

            canvas.DrawLine(
                Win9xGray,
                X + 1,
                Y + Height - 1,
                X + Width - 1,
                Y + Height - 1);

            canvas.DrawLine(
                Win9xGray,
                X + Width - 1,
                Y + 1,
                X + Width - 1,
                Y + Height - 1);

            if (Text == null)
                Text = "";

            int currX = X + PADDING;
            int currY = Y + PADDING;

            int maxRight =
                X + Width - PADDING - charWidth;

            int maxBottom =
                Y + Height - PADDING - charHeight;

            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];

                if (c == '\n')
                {
                    currX = X + PADDING;
                    currY += charHeight;

                    if (currY > maxBottom)
                        break;

                    continue;
                }

                if (currX > maxRight)
                {
                    currX = X + PADDING;
                    currY += charHeight;

                    if (currY > maxBottom)
                        break;
                }

                canvas.DrawString(
                    c.ToString(),
                    font,
                    Win9xBlack,
                    currX,
                    currY);

                currX += charWidth;
            }

            int cursorX = currX;
            int cursorY = currY;

            if (Focused && (DateTime.Now.Second % 2) == 0)
            {
                if (cursorY <= maxBottom &&
                    cursorX <= maxRight + charWidth)
                {
                    canvas.DrawLine(
                        Win9xBlack,
                        cursorX,
                        cursorY + 1,
                        cursorX,
                        cursorY + charHeight - 2);
                }
            }
        }

        public override void Click()
        {
            Focused = true;
        }

        public void MouseClick(
            int mouseX,
            int mouseY)
        {
            Focused = true;
        }

        public void KeyPressed(KeyEvent keyEvent)
        {
            if (!Focused)
                return;

            if (keyEvent.Key == ConsoleKeyEx.Backspace)
            {
                if (Text.Length > 0)
                {
                    Text = Text.Substring(
                        0,
                        Text.Length - 1);
                }
            }
            else if (keyEvent.Key == ConsoleKeyEx.Enter)
            {
                if (Text.Length < MaxLength)
                {
                    Text += "\n";
                }
            }
            else if (
                keyEvent.KeyChar != '\0' &&
                Text.Length < MaxLength)
            {
                Text += keyEvent.KeyChar;
            }
        }
    }
}