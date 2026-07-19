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
        public int Width { get; set; }
        public int Height { get; set; }

        public bool Active { get; set; }
        public bool Dragging { get; set; }

        public List<Control> Controls { get; set; } = new List<Control>();

        private int dragX, dragY;
        private int dragOffsetX, dragOffsetY;

        public Window(AppBase app, string title, int x, int y, int width, int height)
        {
            this.App = app;
            this.Title = title;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public void AddControl(Control control)
        {
            if (control == null) return;
            this.Controls.Add(control);
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

            System.Drawing.Color bgColor = Active ? System.Drawing.Color.DarkGray : System.Drawing.Color.Gray;
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
                if (i >= Controls.Count) break;
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
            foreach (var ctrl in Controls)
            {
                if (ctrl != null)
                {
                    ctrl.Focused = false;
                }
            }

            int count = Controls.Count;
            for (int i = 0; i < count; i++)
            {
                if (i >= Controls.Count) break;
                var c = Controls[i];
                if (c != null)
                {
                    int absX = this.X + c.X;
                    int absY = this.Y + 25 + c.Y;

                    if (mx >= absX && mx <= absX + c.Width && my >= absY && my <= absY + c.Height)
                    {
                        c.Click();
                        break;
                    }
                }
            }
        }
    }
}