using System.Collections.Generic;
using System.Drawing;
using Cosmos.System.Graphics;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI
{
    public class Window
    {
        public AppBase App { get; set; }
        public string Title { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public bool Active { get; set; }
        public bool Dragging { get; set; }
        public bool IsMinimized { get; set; } = false;

        public List<Control> Controls { get; set; } = new List<Control>();

        private const int PaddingX = 20;
        private const int PaddingY = 25;

        private int dragX, dragY;
        private int dragOffsetX, dragOffsetY;

        private static readonly Color Win9xGray = Color.FromArgb(192, 192, 192);
        private static readonly Color Win9xWhite = Color.FromArgb(255, 255, 255);
        private static readonly Color Win9xDarkGray = Color.FromArgb(128, 128, 128);
        private static readonly Color Win9xBlack = Color.FromArgb(0, 0, 0);
        private static readonly Color Win9xActiveTitle = Color.FromArgb(0, 0, 128);
        private static readonly Color Win9xInactiveTitle = Color.FromArgb(128, 128, 128);

        public Window(AppBase app, string title, int x, int y)
        {
            this.App = app;
            this.Title = title ?? "Uygulama";
            this.X = x;
            this.Y = y;

            int minWidth = (this.Title.Length * 8) + 60;
            this.Width = minWidth;
            this.Height = 40;
        }

        public void UpdateSize()
        {
            int minWidth = (Title.Length * 8) + 60;
            int maxWidth = 0;
            int maxHeight = 0;

            for (int i = 0; i < Controls.Count; i++)
            {
                var c = Controls[i];
                if (c != null && c.Visible)
                {
                    int controlRight = c.X + c.Width;
                    int controlBottom = c.Y + c.Height;

                    if (controlRight > maxWidth) maxWidth = controlRight;
                    if (controlBottom > maxHeight) maxHeight = controlBottom;
                }
            }

            int calculatedWidth = maxWidth + PaddingX;
            this.Width = calculatedWidth < minWidth ? minWidth : calculatedWidth;

            this.Height = 25 + maxHeight + PaddingY;
        }

        public void AddControl(Control control)
        {
            if (control == null) return;
            this.Controls.Add(control);
            UpdateSize();
        }

        public void Draw(Canvas canvas)
        {
            if (IsMinimized) return;

            if (Dragging)
            {
                Color borderColor = Win9xBlack;
                canvas.DrawLine(borderColor, dragX, dragY, dragX + Width, dragY);
                canvas.DrawLine(borderColor, dragX, dragY + Height, dragX + Width, dragY + Height);
                canvas.DrawLine(borderColor, dragX, dragY, dragX, dragY + Height);
                canvas.DrawLine(borderColor, dragX + Width, dragY, dragX + Width, dragY + Height);
                canvas.DrawLine(borderColor, dragX, dragY + 25, dragX + Width, dragY + 25);
                return;
            }

            canvas.DrawFilledRectangle(Win9xGray, X, Y, Width, Height);

            canvas.DrawLine(Win9xWhite, X, Y, X + Width - 1, Y);
            canvas.DrawLine(Win9xWhite, X, Y, X, Y + Height - 1);

            canvas.DrawLine(Win9xDarkGray, X + 1, Y + 1, X + Width - 2, Y + 1);
            canvas.DrawLine(Win9xDarkGray, X + 1, Y + 1, X + 1, Y + Height - 2);

            canvas.DrawLine(Win9xBlack, X, Y + Height - 1, X + Width, Y + Height - 1);
            canvas.DrawLine(Win9xBlack, X + Width - 1, Y, X + Width - 1, Y + Height);

            canvas.DrawLine(Win9xDarkGray, X + 1, Y + Height - 2, X + Width - 2, Y + Height - 2);
            canvas.DrawLine(Win9xDarkGray, X + Width - 2, Y + 1, X + Width - 2, Y + Height - 2);

            Color titleColor = Active ? Win9xActiveTitle : Win9xInactiveTitle;
            canvas.DrawFilledRectangle(titleColor, X + 2, Y + 2, Width - 4, 23);

            Color textColor = Win9xWhite;
            canvas.DrawString(Title, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, textColor, X + 8, Y + 6);

            int closeX = X + Width - 22;
            int closeY = Y + 5;
            int closeSize = 16;

            canvas.DrawFilledRectangle(Win9xGray, closeX, closeY, closeSize, closeSize);
            canvas.DrawLine(Win9xWhite, closeX, closeY, closeX + closeSize - 1, closeY);
            canvas.DrawLine(Win9xWhite, closeX, closeY, closeX, closeY + closeSize - 1);
            canvas.DrawLine(Win9xBlack, closeX, closeY + closeSize - 1, closeX + closeSize, closeY + closeSize - 1);
            canvas.DrawLine(Win9xBlack, closeX + closeSize - 1, closeY, closeX + closeSize - 1, closeY + closeSize);
            canvas.DrawLine(Win9xDarkGray, closeX + 1, closeY + closeSize - 2, closeX + closeSize - 2, closeY + closeSize - 2);
            canvas.DrawLine(Win9xDarkGray, closeX + closeSize - 2, closeY + 1, closeX + closeSize - 2, closeY + closeSize - 2);

            Color xColor = Win9xBlack;
            canvas.DrawLine(xColor, closeX + 4, closeY + 4, closeX + closeSize - 5, closeY + closeSize - 5);
            canvas.DrawLine(xColor, closeX + 4, closeY + 5, closeX + closeSize - 6, closeY + closeSize - 5);
            canvas.DrawLine(xColor, closeX + closeSize - 5, closeY + 4, closeX + 4, closeY + closeSize - 5);
            canvas.DrawLine(xColor, closeX + closeSize - 5, closeY + 5, closeX + 5, closeY + closeSize - 5);

            int minBtnX = closeX - 18;
            int minBtnY = Y + 5;
            int minBtnSize = 16;

            canvas.DrawFilledRectangle(Win9xGray, minBtnX, minBtnY, minBtnSize, minBtnSize);
            canvas.DrawLine(Win9xWhite, minBtnX, minBtnY, minBtnX + minBtnSize - 1, minBtnY);
            canvas.DrawLine(Win9xWhite, minBtnX, minBtnY, minBtnX, minBtnY + minBtnSize - 1);
            canvas.DrawLine(Win9xBlack, minBtnX, minBtnY + minBtnSize - 1, minBtnX + minBtnSize, minBtnY + minBtnSize - 1);
            canvas.DrawLine(Win9xBlack, minBtnX + minBtnSize - 1, minBtnY, minBtnX + minBtnSize - 1, minBtnY + minBtnSize);
            canvas.DrawLine(Win9xDarkGray, minBtnX + 1, minBtnY + minBtnSize - 2, minBtnX + minBtnSize - 2, minBtnY + minBtnSize - 2);
            canvas.DrawLine(Win9xDarkGray, minBtnX + minBtnSize - 2, minBtnY + 1, minBtnX + minBtnSize - 2, minBtnY + minBtnSize - 2);

            canvas.DrawLine(Win9xBlack, minBtnX + 4, minBtnY + 11, minBtnX + minBtnSize - 5, minBtnY + 11);

            int count = Controls.Count;
            for (int i = 0; i < count; i++)
            {
                var c = Controls[i];
                if (c != null)
                {
                    int originalX = c.X;
                    int originalY = c.Y;

                    c.X = this.X + originalX;
                    c.Y = this.Y + 25 + originalY;

                    c.Draw(canvas);

                    c.X = originalX;
                    c.Y = originalY;
                }
            }

            for (int i = 0; i < count; i++)
            {
                if (Controls[i] is ComboBox cb && cb.IsDropped)
                {
                    int originalX = cb.X;
                    int originalY = cb.Y;

                    cb.X = this.X + originalX;
                    cb.Y = this.Y + 25 + originalY;

                    cb.Draw(canvas);

                    cb.X = originalX;
                    cb.Y = originalY;
                }
            }
        }

        public bool Contains(int mx, int my) => !IsMinimized && mx >= X && mx <= X + Width && my >= Y && my <= Y + Height;
        public bool TitleContains(int mx, int my) => !IsMinimized && mx >= X && mx <= X + Width && my >= Y && my <= Y + 25;
        public bool CloseContains(int mx, int my) => !IsMinimized && mx >= X + Width - 22 && mx <= X + Width - 6 && my >= Y + 5 && my <= Y + 21;

        public bool MinimizeContains(int mx, int my) => !IsMinimized && mx >= X + Width - 40 && mx <= X + Width - 24 && my >= Y + 5 && my <= Y + 21;

        public void StartDrag(int mx, int my)
        {
            Dragging = true;
            dragOffsetX = mx - X;
            dragOffsetY = my - Y;
            dragX = X;
            dragY = Y;
        }

        public void Drag(int mx, int my, Canvas canvas)
        {
            if (!Dragging) return;

            int targetX = mx - dragOffsetX;
            int targetY = my - dragOffsetY;

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;

            if (targetX < 0) targetX = 0;
            if (targetX + Width > screenWidth) targetX = screenWidth - Width;
            if (targetY < 0) targetY = 0;
            if (targetY + Height > screenHeight) targetY = screenHeight - Height;

            dragX = targetX;
            dragY = targetY;
        }

        public void StopDrag()
        {
            if (!Dragging) return;
            Dragging = false;

            X = dragX;
            Y = dragY;
        }

        public void CheckControlsClick(int mx, int my)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is ComboBox cb && cb.IsDropped)
                {
                    int cbAbsX = this.X + cb.X;
                    int cbAbsY = this.Y + 25 + cb.Y;
                    int listBottom = cbAbsY + cb.Height + (cb.Items.Count * 20);

                    if (!(mx >= cbAbsX && mx <= cbAbsX + cb.Width && my >= cbAbsY && my <= listBottom))
                    {
                        cb.CloseDropdown();
                    }
                }
            }

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] != null)
                {
                    Controls[i].Focused = false;
                }
            }

            int count = Controls.Count;
            for (int i = 0; i < count; i++)
            {
                var c = Controls[i];
                if (c is ComboBox comboBox && comboBox.IsDropped)
                {
                    int cbAbsX = this.X + comboBox.X;
                    int cbAbsY = this.Y + 25 + comboBox.Y;
                    int listTop = cbAbsY + comboBox.Height;
                    int listBottom = listTop + (comboBox.Items.Count * 20);

                    if (mx >= cbAbsX && mx <= cbAbsX + comboBox.Width && my >= listTop && my <= listBottom)
                    {
                        comboBox.HandleAbsoluteClick(this.X, this.Y, mx, my);
                        return;
                    }
                }
            }

            for (int i = count - 1; i >= 0; i--)
            {
                var c = Controls[i];
                if (c == null || !c.Visible) continue;

                int absX = this.X + c.X;
                int absY = this.Y + 25 + c.Y;

                if (mx >= absX && mx <= absX + c.Width && my >= absY && my <= absY + c.Height)
                {
                    c.Focused = true;

                    if (c is ListBox listBox)
                    {
                        listBox.HandleAbsoluteClick(mx, my, absX, absY);
                        c.Focused = true;
                        return;
                    }

                    if (c is RichTextBox richTextBox)
                    {
                        richTextBox.MouseClick(mx, my);
                    }
                    else
                    {
                        c.Click();
                    }

                    break;
                }
            }
        }
    }
}