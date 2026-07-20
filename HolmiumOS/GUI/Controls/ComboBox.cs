using System;
using System.Collections.Generic;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class ComboBox : Control
    {
        public List<string> Items { get; set; } = new List<string>();
        public int SelectedIndex { get; set; } = 0;
        public bool IsDropped { get; private set; } = false;
        public Action<string> OnSelectedIndexChanged;

        private const int ItemHeight = 20;

        public ComboBox(int x, int y, int width) : base(x, y, width, 22)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(Color.White, X, Y, Width, Height);
            Color borderColor = Focused ? Color.Blue : Color.Black;

            canvas.DrawLine(borderColor, X, Y, X + Width, Y);
            canvas.DrawLine(borderColor, X, Y + Height, X + Width, Y + Height);
            canvas.DrawLine(borderColor, X, Y, X, Y + Height);
            canvas.DrawLine(borderColor, X + Width, Y, X + Width, Y + Height);

            int arrowBoxWidth = 20;
            int arrowLeft = X + Width - arrowBoxWidth;
            canvas.DrawFilledRectangle(Color.LightGray, arrowLeft, Y + 1, arrowBoxWidth - 1, Height - 2);
            canvas.DrawLine(Color.Gray, arrowLeft, Y, arrowLeft, Y + Height);
            canvas.DrawString("v", PCScreenFont.Default, Color.Black, arrowLeft + 6, Y + 3);

            if (Items.Count > 0 && SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                canvas.DrawString(Items[SelectedIndex], PCScreenFont.Default, Color.Black, X + 5, Y + 3);
            }

            if (IsDropped)
            {
                int listHeight = Items.Count * ItemHeight;
                canvas.DrawFilledRectangle(Color.White, X, Y + Height, Width, listHeight);

                canvas.DrawLine(Color.Black, X, Y + Height, X, Y + Height + listHeight);
                canvas.DrawLine(Color.Black, X + Width, Y + Height, X + Width, Y + Height + listHeight);
                canvas.DrawLine(Color.Black, X, Y + Height + listHeight, X + Width, Y + Height + listHeight);

                for (int i = 0; i < Items.Count; i++)
                {
                    int itemY = Y + Height + (i * ItemHeight);
                    if (i == SelectedIndex)
                    {
                        canvas.DrawFilledRectangle(Color.Blue, X + 1, itemY, Width - 2, ItemHeight);
                        canvas.DrawString(Items[i], PCScreenFont.Default, Color.White, X + 5, itemY + 2);
                    }
                    else
                    {
                        canvas.DrawString(Items[i], PCScreenFont.Default, Color.Black, X + 5, itemY + 2);
                    }
                }
            }
        }

        public override void Click()
        {
            this.Focused = true;
            this.IsDropped = !this.IsDropped;
        }

        public void CloseDropdown()
        {
            this.IsDropped = false;
        }

        public void HandleAbsoluteClick(int windowX, int windowY, int mx, int my)
        {
            int absX = windowX + this.X;
            int absY = windowY + 25 + this.Y;

            int listTop = absY + Height;
            int listBottom = listTop + (Items.Count * ItemHeight);

            if (mx >= absX && mx <= absX + Width && my >= listTop && my <= listBottom)
            {
                int relativeY = my - listTop;
                int clickedIndex = relativeY / ItemHeight;

                if (clickedIndex >= 0 && clickedIndex < Items.Count)
                {
                    this.SelectedIndex = clickedIndex;

                    if (OnSelectedIndexChanged != null)
                    {
                        OnSelectedIndexChanged.Invoke(Items[SelectedIndex]);
                    }
                }
            }

            this.IsDropped = false;
        }
    }
}