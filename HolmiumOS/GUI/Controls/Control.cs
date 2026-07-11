using Cosmos.System.Graphics;

namespace HolmiumOS.GUI.Controls
{
    public abstract class Control
    {
        public int X;
        public int Y;

        public int Width;
        public int Height;

        public bool Visible = true;

        public Window Parent;

        public Control(
            int x,
            int y,
            int width,
            int height
        )
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;
        }

        public int AbsoluteX
        {
            get
            {
                if (Parent == null)
                    return X;

                return Parent.X + X;
            }
        }

        public int AbsoluteY
        {
            get
            {
                if (Parent == null)
                    return Y;

                return Parent.Y + Y;
            }
        }

        public abstract void Draw(Canvas canvas);

        public virtual void Click()
        {

        }

        public bool Contains(int mouseX, int mouseY)
        {
            return
                mouseX >= AbsoluteX &&
                mouseX <= AbsoluteX + Width &&
                mouseY >= AbsoluteY &&
                mouseY <= AbsoluteY + Height;
        }
    }
}