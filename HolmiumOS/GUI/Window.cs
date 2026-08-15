using System.Collections.Generic;
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

        public List<Control> Controls { get; set; } = new List<Control>();

        private const int PaddingX = 20;
        private const int PaddingY = 25;

        private int dragX, dragY;
        private int dragOffsetX, dragOffsetY;

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
            if (Dragging)
            {
                System.Drawing.Color borderColor = System.Drawing.Color.Black;

                canvas.DrawLine(borderColor, dragX, dragY, dragX + Width, dragY);
                canvas.DrawLine(borderColor, dragX, dragY + Height, dragX + Width, dragY + Height);
                canvas.DrawLine(borderColor, dragX, dragY, dragX, dragY + Height);
                canvas.DrawLine(borderColor, dragX + Width, dragY, dragX + Width, dragY + Height);

                canvas.DrawLine(borderColor, dragX, dragY + 25, dragX + Width, dragY + 25);
                return;
            }

            System.Drawing.Color bgColor = System.Drawing.Color.DarkGray;
            canvas.DrawFilledRectangle(bgColor, X, Y, Width, Height);

            System.Drawing.Color titleColor = Active ? System.Drawing.Color.Blue : System.Drawing.Color.DimGray;
            canvas.DrawFilledRectangle(titleColor, X, Y, Width, 25);

            System.Drawing.Color textColor = Active ? System.Drawing.Color.White : System.Drawing.Color.LightGray;
            canvas.DrawString(Title, Cosmos.System.Graphics.Fonts.PCScreenFont.Default, textColor, X + 10, Y + 5);

            int closeX = X + Width - 20;
            int closeY = Y + 5;
            int closeSize = 15;

            canvas.DrawFilledRectangle(System.Drawing.Color.Red, closeX, closeY, closeSize, closeSize);
            System.Drawing.Color xColor = System.Drawing.Color.White;

            canvas.DrawLine(xColor, closeX + 3, closeY + 3, closeX + closeSize - 4, closeY + closeSize - 4);
            canvas.DrawLine(xColor, closeX + closeSize - 4, closeY + 3, closeX + 3, closeY + closeSize - 4);

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

        public bool Contains(int mx, int my) => mx >= X && mx <= X + Width && my >= Y && my <= Y + Height;
        public bool TitleContains(int mx, int my) => mx >= X && mx <= X + Width && my >= Y && my <= Y + 25;
        public bool CloseContains(int mx, int my) => mx >= X + Width - 20 && mx <= X + Width - 5 && my >= Y + 5 && my <= Y + 20;

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