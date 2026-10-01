using System;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using Cosmos.Kernel.System.Keyboard;

namespace HolmiumOS.GUI.Controls
{
    public class TextBox : Control
    {
        public string Text = "";
        public int MaxLength = 30;

        private static readonly Color Win9xWhite =
            Color.FromArgb(255, 255, 255);

        private static readonly Color Win9xDarkGray =
            Color.FromArgb(128, 128, 128);

        private static readonly Color Win9xBlack =
            Color.FromArgb(0, 0, 0);

        private static readonly Color Win9xGray =
            Color.FromArgb(192, 192, 192);

        public TextBox(
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

            int textY =
                Y + (Height - font.Height) / 2;

            canvas.DrawString(
                Text ?? "",
                font,
                Win9xBlack,
                X + 5,
                textY);

            if (Focused)
            {
                DateTime now = DateTime.Now;
                bool showCursor = (now.Second % 2) == 0;

                if (showCursor)
                {
                    int textWidth =
                        font.MeasureString(Text ?? "");

                    int cursorX =
                        X + 5 + textWidth;

                    int cursorTop =
                        Y + (Height - font.Height) / 2;

                    int cursorBottom =
                        cursorTop + font.Height - 1;

                    if (cursorX < X + Width - 6)
                    {
                        canvas.DrawLine(
                            Win9xBlack,
                            cursorX,
                            cursorTop,
                            cursorX,
                            cursorBottom);
                    }
                }
            }
        }

        public override void Click()
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
            else if (
                keyEvent.KeyChar != '\0' &&
                Text.Length < MaxLength)
            {
                if (keyEvent.Key == ConsoleKeyEx.Enter)
                    return;

                Text += keyEvent.KeyChar;
            }
        }
    }
}