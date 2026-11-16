
using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class ListBox : Control
    {
        private readonly List<string> items = new List<string>();

        public IReadOnlyList<string> Items => items;
        public int SelectedIndex { get; private set; } = -1;
        public Action<int, string>? OnSelectedIndexChanged;

        private const int ItemHeight = 36;
        private const int CornerRadius = 7;

        private static readonly Color BackgroundColor = Color.FromArgb(255, 255, 255);
        private static readonly Color BorderColor = Color.FromArgb(190, 190, 190);
        private static readonly Color SelectedColor = Color.FromArgb(53, 132, 228);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);
        private static readonly Color White = Color.FromArgb(255, 255, 255);

        public ListBox(int x, int y, int width, int height)
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

        public void AddItem(string item)
        {
            items.Add(item ?? "");
        }

        public void AddItems(IEnumerable<string> collection)
        {
            if (collection == null)
                return;

            foreach (var item in collection)
                items.Add(item ?? "");
        }

        public void Clear()
        {
            items.Clear();
            SelectedIndex = -1;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= items.Count)
                return;

            items.RemoveAt(index);

            if (SelectedIndex == index)
                SelectedIndex = -1;
            else if (SelectedIndex > index)
                SelectedIndex--;
        }

        public string? GetSelectedItem()
        {
            if (SelectedIndex < 0 || SelectedIndex >= items.Count)
                return null;

            return items[SelectedIndex];
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null || Width <= 0 || Height <= 0)
                return;

            Font font = FontManager.UiFont;
            int fontSize = FontManager.UiFontSize;

            DrawRoundedRect(canvas, BorderColor, X, Y, Width, Height, CornerRadius);
            DrawRoundedRect(canvas, BackgroundColor, X + 1, Y + 1, Width - 2, Height - 2, CornerRadius - 1);

            int visibleCount = (Height - 4) / ItemHeight;

            for (int i = 0; i < visibleCount && i < items.Count; i++)
            {
                int itemY = Y + 2 + i * ItemHeight;
                bool selected = i == SelectedIndex;

                if (selected)
                {
                    DrawRoundedRect(canvas, SelectedColor, X + 3, itemY, Width - 6, ItemHeight, 5);
                }

                Color textColor = selected ? White : TextColor;

                int textY;

                if (font is TrueTypeFont trueTypeFont)
                {
                    int textHeight = trueTypeFont.GetLineHeight(fontSize);
                    textY = itemY + (ItemHeight - textHeight) / 2;

                    canvas.DrawString(
                        items[i],
                        trueTypeFont,
                        fontSize,
                        textColor,
                        X + 10,
                        textY);
                }
                else
                {
                    textY = itemY + (ItemHeight - font.Height) / 2;

                    canvas.DrawString(
                        items[i],
                        font,
                        textColor,
                        X + 10,
                        textY);
                }
            }
        }

        public override void Click()
        {
            Focused = true;
        }

        public void SelectIndex(int index)
        {
            if (index < 0 || index >= items.Count)
                return;

            SelectedIndex = index;
            Focused = true;
            OnSelectedIndexChanged?.Invoke(index, items[index]);
        }

        public void HandleAbsoluteClick(int absoluteX, int absoluteY, int absoluteListX, int absoluteListY)
        {
            int relativeX = absoluteX - absoluteListX;
            int relativeY = absoluteY - absoluteListY;

            if (relativeX < 0 || relativeX >= Width || relativeY < 0 || relativeY >= Height)
                return;

            int index = relativeY / ItemHeight;

            if (index < 0 || index >= items.Count)
                return;

            SelectIndex(index);
        }
    }
}
