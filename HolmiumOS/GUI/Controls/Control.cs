using Cosmos.System.Graphics;
using HolmiumOS.GUI;

namespace HolmiumOS.GUI.Controls
{
    public abstract class Control
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public bool Visible = true;
        public bool Focused = false;

        public CursorType? Cursor { get; set; }

        protected Control(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public abstract void Draw(Canvas canvas);
        public abstract void Click();

        public bool Contains(int mouseX, int mouseY)
        {
            return mouseX >= X &&
                   mouseX <= X + Width &&
                   mouseY >= Y &&
                   mouseY <= Y + Height;
        }
    }
}