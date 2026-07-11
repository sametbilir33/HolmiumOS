using System.Drawing;
using System.Collections.Generic;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI
{
    public class Window
    {
        public int X;
        public int Y;

        public int Width;
        public int Height;

        public string Title;

        public bool Visible = true;

        public bool Active;

        public bool Dragging;


        private int dragOffsetX;
        private int dragOffsetY;


        public AppBase App;


        public List<Control> Controls =
            new List<Control>();


        public Window(
            AppBase app,
            string title,
            int x,
            int y,
            int width,
            int height
        )
        {
            App = app;

            Title = title;

            X = x;
            Y = y;

            Width = width;
            Height = height;
        }



        public void Draw(Canvas canvas)
        {
            if (!Visible)
                return;


            canvas.DrawFilledRectangle(
                Color.LightGray,
                X,
                Y,
                Width,
                Height
            );


            canvas.DrawFilledRectangle(
                Active ? Color.DarkBlue : Color.Gray,
                X,
                Y,
                Width,
                22
            );


            canvas.DrawString(
                Title,
                PCScreenFont.Default,
                Color.White,
                X + 5,
                Y + 5
            );


            canvas.DrawFilledRectangle(
                Color.Red,
                X + Width - 20,
                Y,
                20,
                20
            );


            canvas.DrawString(
                "X",
                PCScreenFont.Default,
                Color.White,
                X + Width - 15,
                Y + 4
            );


            foreach (Control control in Controls)
            {
                control.Draw(canvas);
            }
        }

        public void CheckControlsClick(int mouseX, int mouseY)
        {
            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                Control control = Controls[i];


                if (control.Contains(mouseX, mouseY))
                {
                    control.Click();
                    return;
                }
            }
        }

        public void AddControl(Control control)
        {
            control.Parent = this;

            Controls.Add(control);
        }


        public bool Contains(int mx, int my)
        {
            return
                mx >= X &&
                mx <= X + Width &&
                my >= Y &&
                my <= Y + Height;
        }



        public bool TitleContains(int mx, int my)
        {
            return
                mx >= X &&
                mx <= X + Width &&
                my >= Y &&
                my <= Y + 22;
        }



        public bool CloseContains(int mx, int my)
        {
            return
                mx >= X + Width - 20 &&
                my >= Y &&
                mx <= X + Width &&
                my <= Y + 20;
        }



        public void StartDrag(int mx, int my)
        {
            Dragging = true;

            dragOffsetX = mx - X;
            dragOffsetY = my - Y;
        }



        public void Drag(int mx, int my)
        {
            if (!Dragging)
                return;


            X = mx - dragOffsetX;
            Y = my - dragOffsetY;
        }



        public void StopDrag()
        {
            Dragging = false;
        }
    }
}