
using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class ComboBox : Control
    {
        public List<string> Items { get; set; } = new List<string>();
        public int SelectedIndex { get; set; } = 0;
        public bool IsDropped { get; private set; } = false;
        public Action<string>? OnSelectedIndexChanged;

        private const int ItemHeight = 36;
        private const int CornerRadius = 7;
        private const int ArrowWidth = 32;

        private static readonly Color BackgroundColor = Color.FromArgb(246, 245, 244);
        private static readonly Color BorderColor = Color.FromArgb(190, 190, 190);
        private static readonly Color HoverColor = Color.FromArgb(235, 235, 235);
        private static readonly Color PressedColor = Color.FromArgb(222, 222, 220);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);
        private static readonly Color AccentColor = Color.FromArgb(53, 132, 228);
        private static readonly Color AccentHoverColor = Color.FromArgb(42, 112, 204);
        private static readonly Color White = Color.FromArgb(255, 255, 255);
        private static readonly Color ShadowColor = Color.FromArgb(30, 0, 0, 0);

        public ComboBox(int x, int y, int width)
            : base(x, y, width, 36)
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

        private static void DrawArrow(Canvas canvas, Color color, int centerX, int centerY)
        {
            canvas.DrawLine(color, centerX - 5, centerY - 2, centerX, centerY + 3);
            canvas.DrawLine(color, centerX, centerY + 3, centerX + 5, centerY - 2);
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null || Width <= 0 || Height <= 0)
                return;

            Font font = FontManager.UiFont;

            Color background = BackgroundColor;

            if (Pressed)
                background = PressedColor;
            else if (Hovered)
                background = HoverColor;

            DrawRoundedRect(canvas, ShadowColor, X, Y + 2, Width, Height, CornerRadius);
            DrawRoundedRect(canvas, BorderColor, X, Y, Width, Height, CornerRadius);
            DrawRoundedRect(canvas, background, X + 1, Y + 1, Width - 2, Height - 2, CornerRadius - 1);

            int arrowCenterX = X + Width - ArrowWidth / 2 - 1;
            int arrowCenterY = Y + Height / 2;

            DrawArrow(canvas, TextColor, arrowCenterX, arrowCenterY);

            if (Items.Count > 0 && SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                int textY = Y + (Height - font.Height) / 2;

                canvas.DrawString(
                    Items[SelectedIndex],
                    font,
                    TextColor,
                    X + 10,
                    textY);
            }

            if (!IsDropped || Items.Count == 0)
                return;

            int listY = Y + Height;
            int listHeight = Items.Count * ItemHeight;

            DrawRoundedRect(canvas, ShadowColor, X, listY + 2, Width, listHeight + 2, CornerRadius);
            DrawRoundedRect(canvas, BorderColor, X, listY, Width, listHeight, CornerRadius);
            DrawRoundedRect(canvas, White, X + 1, listY + 1, Width - 2, listHeight - 2, CornerRadius - 1);

            for (int i = 0; i < Items.Count; i++)
            {
                int itemY = listY + i * ItemHeight;

                if (i == SelectedIndex)
                {
                    canvas.DrawFilledRectangle(
                        AccentColor,
                        X + 2,
                        itemY,
                        Width - 4,
                        ItemHeight);
                }

                Color itemTextColor = i == SelectedIndex ? White : TextColor;
                int textY = itemY + (ItemHeight - font.Height) / 2;

                canvas.DrawString(
                    Items[i],
                    font,
                    itemTextColor,
                    X + 10,
                    textY);
            }
        }

        public override void Click()
        {
            Focused = true;
            IsDropped = !IsDropped;
        }

        public void CloseDropdown()
        {
            IsDropped = false;
        }

        public void HandleAbsoluteClick(int windowX, int windowY, int mx, int my)
        {
            int absX = windowX + X;
            int absY = windowY + 25 + Y;

            int listTop = absY + Height;
            int listBottom = listTop + Items.Count * ItemHeight;

            if (mx >= absX && mx <= absX + Width && my >= listTop && my < listBottom)
            {
                int relativeY = my - listTop;
                int clickedIndex = relativeY / ItemHeight;

                if (clickedIndex >= 0 && clickedIndex < Items.Count)
                {
                    SelectedIndex = clickedIndex;
                    OnSelectedIndexChanged?.Invoke(Items[SelectedIndex]);
                }
            }

            IsDropped = false;
        }
    }
}
