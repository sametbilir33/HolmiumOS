using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class ListBox : Control
    {
        private readonly List<string> items = new List<string>();

        public IReadOnlyList<string> Items => items;
        public int SelectedIndex { get; private set; } = -1;
        public Action<int, string> OnSelectedIndexChanged;

        private const int ItemHeight = 20;

        public ListBox(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
        }

        public void AddItem(string item)
        {
            items.Add(item ?? "");
        }

        public void AddItems(IEnumerable<string> collection)
        {
            if (collection == null) return;

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
            if (index < 0 || index >= items.Count) return;

            items.RemoveAt(index);

            if (SelectedIndex >= items.Count)
                SelectedIndex = items.Count - 1;
        }

        public string GetSelectedItem()
        {
            if (SelectedIndex < 0 || SelectedIndex >= items.Count)
                return null;

            return items[SelectedIndex];
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(Color.White, X, Y, Width, Height);
            canvas.DrawRectangle(Color.Black, X, Y, Width, Height);

            int visibleCount = Height / ItemHeight;

            for (int i = 0; i < visibleCount && i < items.Count; i++)
            {
                int itemY = Y + (i * ItemHeight);

                if (i == SelectedIndex)
                {
                    canvas.DrawFilledRectangle(
                        Color.DodgerBlue,
                        X + 1,
                        itemY + 1,
                        Width - 2,
                        ItemHeight - 2
                    );

                    canvas.DrawString(
                        items[i],
                        PCScreenFont.Default,
                        Color.White,
                        X + 5,
                        itemY + 3
                    );
                }
                else
                {
                    canvas.DrawString(
                        items[i],
                        PCScreenFont.Default,
                        Color.Black,
                        X + 5,
                        itemY + 3
                    );
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

            if (OnSelectedIndexChanged != null)
                OnSelectedIndexChanged(index, items[index]);
        }

        public void HandleAbsoluteClick(int absoluteX, int absoluteY, int absoluteListX, int absoluteListY)
        {
            int relativeX = absoluteX - absoluteListX;
            int relativeY = absoluteY - absoluteListY;

            if (relativeX < 0 || relativeX >= Width ||
                relativeY < 0 || relativeY >= Height)
                return;

            int index = relativeY / ItemHeight;

            if (index < 0 || index >= items.Count)
                return;

            SelectIndex(index);
        }
    }
}
