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

        // Windows 9x Klasik Renk Paleti
        private static readonly Color Win9xWhite = Color.FromArgb(255, 255, 255);
        private static readonly Color Win9xDarkGray = Color.FromArgb(128, 128, 128);
        private static readonly Color Win9xBlack = Color.FromArgb(0, 0, 0);
        private static readonly Color Win9xGray = Color.FromArgb(192, 192, 192);
        private static readonly Color Win9xSelectedBlue = Color.FromArgb(0, 0, 128); // Klasik Win9x Koyu Mavi

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

            // 1. Liste Arka Planı (Beyaz)
            canvas.DrawFilledRectangle(Win9xWhite, X, Y, Width, Height);

            // 2. Win9x Sunken (İçeri Çökük) 3D Çerçeve Efekti
            canvas.DrawLine(Win9xDarkGray, X, Y, X + Width - 1, Y);
            canvas.DrawLine(Win9xDarkGray, X, Y, X, Y + Height - 1);

            canvas.DrawLine(Win9xBlack, X + 1, Y + 1, X + Width - 2, Y + 1);
            canvas.DrawLine(Win9xBlack, X + 1, Y + 1, X + 1, Y + Height - 2);

            canvas.DrawLine(Win9xWhite, X, Y + Height, X + Width, Y + Height);
            canvas.DrawLine(Win9xWhite, X + Width, Y, X + Width, Y + Height);

            canvas.DrawLine(Win9xGray, X + 1, Y + Height - 1, X + Width - 1, Y + Height - 1);
            canvas.DrawLine(Win9xGray, X + Width - 1, Y + 1, X + Width - 1, Y + Height - 1);

            int visibleCount = Height / ItemHeight;

            // 3. Liste Öğelerinin Çizimi
            for (int i = 0; i < visibleCount && i < items.Count; i++)
            {
                int itemY = Y + 2 + (i * ItemHeight); // İç çerçeve payı için küçük bir kaydırma (+2)

                if (i == SelectedIndex)
                {
                    // Seçili Öğe (Klasik Win9x Koyu Mavi Arka Plan, Beyaz Metin)
                    canvas.DrawFilledRectangle(
                        Win9xSelectedBlue,
                        X + 2,
                        itemY,
                        Width - 4,
                        ItemHeight
                    );

                    canvas.DrawString(
                        items[i],
                        PCScreenFont.Default,
                        Win9xWhite,
                        X + 6,
                        itemY + 2
                    );
                }
                else
                {
                    // Normal Öğe (Beyaz Arka Plan, Siyah Metin)
                    canvas.DrawString(
                        items[i],
                        PCScreenFont.Default,
                        Win9xBlack,
                        X + 6,
                        itemY + 2
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