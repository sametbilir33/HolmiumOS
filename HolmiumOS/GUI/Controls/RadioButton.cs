
using System;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class RadioButton : Control
    {
        private Window? _parentWindow;

        public string Text;
        public string GroupName;
        public bool Checked { get; set; }

        private const int CircleSize = 20;

        private static readonly Color BorderColor = Color.FromArgb(128, 128, 128);
        private static readonly Color HoverBorderColor = Color.FromArgb(80, 80, 80);
        private static readonly Color BackgroundColor = Color.FromArgb(255, 255, 255);
        private static readonly Color AccentColor = Color.FromArgb(53, 132, 228);
        private static readonly Color AccentHoverColor = Color.FromArgb(42, 112, 204);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);

        public RadioButton(Window parentWindow, string text, string groupName, int x, int y)
            : base(x, y, (text ?? "").Length * FontManager.UiFontSize + 40, 32)
        {
            _parentWindow = parentWindow;
            Text = text ?? "";
            GroupName = groupName ?? "DefaultGroup";
            Checked = false;
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null)
                return;

            int circleX = X;
            int circleY = Y + (Height - CircleSize) / 2;
            int centerX = circleX + CircleSize / 2;
            int centerY = circleY + CircleSize / 2;

            Color border = Hovered ? HoverBorderColor : BorderColor;

            canvas.DrawFilledCircle(BackgroundColor, centerX, centerY, CircleSize / 2);
            canvas.DrawCircle(border, centerX, centerY, CircleSize / 2);

            if (Checked)
            {
                Color accent = Pressed ? AccentHoverColor : AccentColor;

                canvas.DrawFilledCircle(BackgroundColor, centerX, centerY, 7);
                canvas.DrawFilledCircle(accent, centerX, centerY, 5);
            }

            Font font = FontManager.UiFont;
            int textX = X + CircleSize + 10;

            if (font is TrueTypeFont trueTypeFont)
            {
                int textHeight = trueTypeFont.GetLineHeight(FontManager.UiFontSize);
                int textY = Y + (Height - textHeight) / 2;

                canvas.DrawString(Text, trueTypeFont, FontManager.UiFontSize, TextColor, textX, textY);
            }
            else
            {
                int textY = Y + (Height - font.Height) / 2;
                canvas.DrawString(Text, font, TextColor, textX, textY);
            }
        }

        public override void Click()
        {
            Focused = true;

            if (Checked)
                return;

            if (_parentWindow != null)
            {
                int totalControls = _parentWindow.Controls.Count;

                for (int i = 0; i < totalControls; i++)
                {
                    Control? control = _parentWindow.Controls[i];

                    if (control is RadioButton radioButton && radioButton.GroupName == GroupName)
                        radioButton.Checked = false;
                }
            }

            Checked = true;
        }
    }
}
