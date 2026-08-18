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

        private static readonly Color Win9xGray = Color.FromArgb(192, 192, 192);
        private static readonly Color Win9xWhite = Color.FromArgb(255, 255, 255);
        private static readonly Color Win9xDarkGray = Color.FromArgb(128, 128, 128);
        private static readonly Color Win9xBlack = Color.FromArgb(0, 0, 0);
        private static readonly Color Win9xSelectedBlue = Color.FromArgb(0, 0, 128);

        public ComboBox(int x, int y, int width) : base(x, y, width, 22)
        {
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            canvas.DrawFilledRectangle(Win9xWhite, X, Y, Width, Height);

            canvas.DrawLine(Win9xDarkGray, X, Y, X + Width - 1, Y);
            canvas.DrawLine(Win9xDarkGray, X, Y, X, Y + Height - 1);

            canvas.DrawLine(Win9xBlack, X + 1, Y + 1, X + Width - 2, Y + 1);
            canvas.DrawLine(Win9xBlack, X + 1, Y + 1, X + 1, Y + Height - 2);

            canvas.DrawLine(Win9xWhite, X, Y + Height, X + Width, Y + Height);
            canvas.DrawLine(Win9xWhite, X + Width, Y, X + Width, Y + Height);

            canvas.DrawLine(Win9xGray, X + 1, Y + Height - 1, X + Width - 1, Y + Height - 1);
            canvas.DrawLine(Win9xGray, X + Width - 1, Y + 1, X + Width - 1, Y + Height - 1);

            int arrowBoxWidth = 18;
            int arrowLeft = X + Width - arrowBoxWidth - 1;
            int arrowTop = Y + 2;
            int arrowHeight = Height - 4;

            canvas.DrawFilledRectangle(Win9xGray, arrowLeft, arrowTop, arrowBoxWidth, arrowHeight);

            canvas.DrawLine(Win9xWhite, arrowLeft, arrowTop, arrowLeft + arrowBoxWidth - 1, arrowTop);
            canvas.DrawLine(Win9xWhite, arrowLeft, arrowTop, arrowLeft, arrowTop + arrowHeight - 1);
            canvas.DrawLine(Win9xBlack, arrowLeft, arrowTop + arrowHeight - 1, arrowLeft + arrowBoxWidth, arrowTop + arrowHeight - 1);
            canvas.DrawLine(Win9xBlack, arrowLeft + arrowBoxWidth - 1, arrowTop, arrowLeft + arrowBoxWidth - 1, arrowTop + arrowHeight);
            canvas.DrawLine(Win9xDarkGray, arrowLeft + 1, arrowTop + arrowHeight - 2, arrowLeft + arrowBoxWidth - 2, arrowTop + arrowHeight - 2);
            canvas.DrawLine(Win9xDarkGray, arrowLeft + arrowBoxWidth - 2, arrowTop + 1, arrowLeft + arrowBoxWidth - 2, arrowTop + arrowHeight - 2);

            canvas.DrawString("v", PCScreenFont.Default, Win9xBlack, arrowLeft + 5, arrowTop + 2);

            if (Items.Count > 0 && SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                canvas.DrawString(Items[SelectedIndex], PCScreenFont.Default, Win9xBlack, X + 5, Y + 4);
            }

            if (IsDropped)
            {
                int listHeight = Items.Count * ItemHeight;

                canvas.DrawFilledRectangle(Win9xWhite, X, Y + Height, Width, listHeight);

                canvas.DrawLine(Win9xBlack, X, Y + Height, X, Y + Height + listHeight);
                canvas.DrawLine(Win9xBlack, X + Width, Y + Height, X + Width, Y + Height + listHeight);
                canvas.DrawLine(Win9xBlack, X, Y + Height + listHeight, X + Width, Y + Height + listHeight);

                canvas.DrawLine(Win9xDarkGray, X + Width + 1, Y + Height + 1, X + Width + 1, Y + Height + listHeight + 1);
                canvas.DrawLine(Win9xDarkGray, X + 1, Y + Height + listHeight + 1, X + Width + 1, Y + Height + listHeight + 1);

                for (int i = 0; i < Items.Count; i++)
                {
                    int itemY = Y + Height + (i * ItemHeight);
                    if (i == SelectedIndex)
                    {
                        canvas.DrawFilledRectangle(Win9xSelectedBlue, X + 1, itemY, Width - 2, ItemHeight);
                        canvas.DrawString(Items[i], PCScreenFont.Default, Win9xWhite, X + 5, itemY + 2);
                    }
                    else
                    {
                        canvas.DrawString(Items[i], PCScreenFont.Default, Win9xBlack, X + 5, itemY + 2);
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