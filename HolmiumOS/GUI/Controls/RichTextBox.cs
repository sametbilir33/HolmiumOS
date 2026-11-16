
using System;
using System.Drawing;
using Cosmos.Kernel.System.Input;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class RichTextBox : Control
    {
        public string Text = "";
        public int MaxLength = 2000;

        private const int Padding = 8;
        private const int CornerRadius = 7;

        private static readonly Color BackgroundColor = Color.FromArgb(255, 255, 255);
        private static readonly Color BorderColor = Color.FromArgb(190, 190, 190);
        private static readonly Color FocusBorderColor = Color.FromArgb(53, 132, 228);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);

        public RichTextBox(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        private static void DrawRoundedRect(Canvas canvas, Color color, int x, int y, int width, int height, int radius)
        {
            if (width <= 0 || height <= 0)
                return;

            radius = Math.Min(radius, Math.Min(width, height) / 2);

            if (radius <= 0)
            {
                canvas.DrawFilledRectangle(color, x, y, width, height);
                return;
            }

            int centerWidth = width - radius * 2;
            int centerHeight = height - radius * 2;

            if (centerWidth > 0)
                canvas.DrawFilledRectangle(color, x + radius, y, centerWidth, height);

            if (centerHeight > 0)
                canvas.DrawFilledRectangle(color, x, y + radius, width, centerHeight);

            canvas.DrawFilledCircle(color, x + radius, y + radius, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
            canvas.DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        private static int GetCharacterWidth(Font font, char character, int fontSize)
        {
            if (font is TrueTypeFont trueTypeFont)
                return Math.Max(1, (int)trueTypeFont.MeasureString(character.ToString(), fontSize));

            return Math.Max(1, (int)font.Width);
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null || Width <= 0 || Height <= 0)
                return;

            if (Text == null)
                Text = "";

            Font font = FontManager.UiFont;
            int fontSize = FontManager.UiFontSize;

            int charHeight = font is TrueTypeFont trueTypeFont
                ? trueTypeFont.GetLineHeight(fontSize)
                : font.Height;

            Color border = Focused ? FocusBorderColor : BorderColor;

            DrawRoundedRect(canvas, border, X, Y, Width, Height, CornerRadius);
            DrawRoundedRect(canvas, BackgroundColor, X + 1, Y + 1, Width - 2, Height - 2, CornerRadius - 1);

            int currX = X + Padding;
            int currY = Y + Padding;

            int maxRight = X + Width - Padding;
            int maxBottom = Y + Height - Padding - charHeight;

            for (int i = 0; i < Text.Length; i++)
            {
                char c = Text[i];

                if (c == '\n')
                {
                    currX = X + Padding;
                    currY += charHeight;

                    if (currY > maxBottom)
                        break;

                    continue;
                }

                int charWidth = GetCharacterWidth(font, c, fontSize);

                if (currX + charWidth > maxRight)
                {
                    currX = X + Padding;
                    currY += charHeight;

                    if (currY > maxBottom)
                        break;
                }

                if (font is TrueTypeFont ttf)
                {
                    canvas.DrawString(c.ToString(), ttf, fontSize, TextColor, currX, currY);
                }
                else
                {
                    canvas.DrawString(c.ToString(), font, TextColor, currX, currY);
                }

                currX += charWidth;
            }

            if (Focused && DateTime.Now.Second % 2 == 0)
            {
                if (currY <= maxBottom && currX <= maxRight)
                {
                    canvas.DrawLine(
                        TextColor,
                        currX,
                        currY + 1,
                        currX,
                        currY + charHeight - 2);
                }
            }
        }

        public override void Click()
        {
            Focused = true;
        }

        public void MouseClick(int mouseX, int mouseY)
        {
            Focused = true;
        }

        public void KeyPressed(KeyEvent keyEvent)
        {
            if (!Focused)
                return;

            if (Text == null)
                Text = "";

            if (keyEvent.Key == Key.Backspace)
            {
                if (Text.Length > 0)
                    Text = Text.Substring(0, Text.Length - 1);
            }
            else if (keyEvent.Key == Key.Enter)
            {
                if (Text.Length < MaxLength)
                    Text += "\n";
            }
            else if (keyEvent.KeyChar != '\0' && Text.Length < MaxLength)
            {
                Text += keyEvent.KeyChar;
            }
        }
    }
}
