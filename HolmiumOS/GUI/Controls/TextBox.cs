
using System;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using Cosmos.Kernel.System.Input;

namespace HolmiumOS.GUI.Controls
{
    public class TextBox : Control
    {
        public string Text = "";
        public int MaxLength = 30;

        private const int PADDING = 8;

        private static readonly Color BackgroundColor = Color.FromArgb(255, 255, 255);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);
        private static readonly Color BorderColor = Color.FromArgb(190, 190, 190);
        private static readonly Color FocusBorderColor = Color.FromArgb(53, 132, 228);
        private static readonly Color HoverBorderColor = Color.FromArgb(150, 150, 150);
        private static readonly Color CursorColor = Color.FromArgb(36, 36, 36);

        public TextBox(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null)
                return;

            Font font = FontManager.UiFont;
            int fontSize = FontManager.UiFontSize;

            int textHeight = GetFontHeight(font, fontSize);
            int textY = Y + (Height - textHeight) / 2;

            Color borderColor = Focused
                ? FocusBorderColor
                : Hovered ? HoverBorderColor : BorderColor;

            canvas.DrawFilledRectangle(BackgroundColor, X + 1, Y + 1, Width - 2, Height - 2);

            DrawRoundedRect(canvas, X, Y, Width, Height, 7, borderColor);

            int textX = X + PADDING;
            int availableWidth = Width - PADDING * 2;

            string displayText = Text ?? "";
            int textWidth = GetTextWidth(font, displayText, fontSize);

            if (textWidth > availableWidth)
            {
                int startIndex = displayText.Length;

                while (startIndex > 0 && GetTextWidth(font, displayText.Substring(startIndex - 1), fontSize) < availableWidth)
                    startIndex--;

                displayText = displayText.Substring(startIndex);
                textWidth = GetTextWidth(font, displayText, fontSize);
            }

            DrawText(canvas, displayText, font, fontSize, TextColor, textX, textY);

            if (Focused && DateTime.Now.Second % 2 == 0)
            {
                int cursorX = textX + textWidth;

                if (cursorX < X + Width - PADDING)
                    canvas.DrawLine(CursorColor, cursorX, textY, cursorX, textY + textHeight - 1);
            }
        }

        private static int GetFontHeight(Font font, int fontSize)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.GetLineHeight(fontSize);

            return font.Height;
        }

        private static int GetTextWidth(Font font, string text, int fontSize)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.MeasureString(text, fontSize);

            return font.MeasureString(text);
        }

        private static void DrawText(Canvas canvas, string text, Font font, int fontSize, Color color, int x, int y)
        {
            if (font is TrueTypeFont trueTypeFont)
                canvas.DrawString(text, trueTypeFont, fontSize, color, x, y);
            else
                canvas.DrawString(text, font, color, x, y);
        }

        private static void DrawRoundedRect(Canvas canvas, int x, int y, int width, int height, int radius, Color color)
        {
            canvas.DrawLine(color, x + radius, y, x + width - radius - 1, y);
            canvas.DrawLine(color, x + radius, y + height - 1, x + width - radius - 1, y + height - 1);
            canvas.DrawLine(color, x, y + radius, x, y + height - radius - 1);
            canvas.DrawLine(color, x + width - 1, y + radius, x + width - 1, y + height - radius - 1);

            canvas.DrawLine(color, x + 2, y + 1, x + radius - 1, y + 1);
            canvas.DrawLine(color, x + width - radius, y + 1, x + width - 3, y + 1);
            canvas.DrawLine(color, x + 2, y + height - 2, x + radius - 1, y + height - 2);
            canvas.DrawLine(color, x + width - radius, y + height - 2, x + width - 3, y + height - 2);

            canvas.DrawLine(color, x + 1, y + 2, x + 1, y + radius - 1);
            canvas.DrawLine(color, x + width - 2, y + 2, x + width - 2, y + radius - 1);
            canvas.DrawLine(color, x + 1, y + height - radius, x + 1, y + height - 3);
            canvas.DrawLine(color, x + width - 2, y + height - radius, x + width - 2, y + height - 3);
        }

        public override void Click()
        {
            Focused = true;
        }

        public void KeyPressed(KeyEvent keyEvent)
        {
            if (!Focused)
                return;

            if (keyEvent.Key == Key.Backspace)
            {
                if (!string.IsNullOrEmpty(Text))
                    Text = Text.Substring(0, Text.Length - 1);
            }
            else if (keyEvent.Key == Key.Enter)
            {
                return;
            }
            else if (keyEvent.KeyChar != '\0' && Text.Length < MaxLength)
            {
                Text += keyEvent.KeyChar;
            }
        }
    }
}
