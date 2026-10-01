using System.Collections.Generic;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
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

        public List<Control> Controls { get; set; } =
            new List<Control>();

        private const int TitleBarHeight = 40;

        private const int PaddingX = 20;
        private const int PaddingY = 25;

        private const int ButtonSize = 24;
        private const int ButtonGap = 4;
        private const int ButtonTop = 8;

        private int dragX, dragY;
        private int dragOffsetX, dragOffsetY;

        private static readonly Color Win9xGray =
            Color.FromArgb(192, 192, 192);

        private static readonly Color Win9xWhite =
            Color.FromArgb(255, 255, 255);

        private static readonly Color Win9xDarkGray =
            Color.FromArgb(128, 128, 128);

        private static readonly Color Win9xBlack =
            Color.FromArgb(0, 0, 0);

        private static readonly Color Win9xActiveTitle =
            Color.FromArgb(0, 0, 128);

        private static readonly Color Win9xInactiveTitle =
            Color.FromArgb(128, 128, 128);

        public Window(
            AppBase app,
            string title,
            int x,
            int y)
        {
            this.App = app;
            this.Title = title ?? "Uygulama";
            this.X = x;
            this.Y = y;

            Font font =
                PCScreenFont.DefaultFont;

            int titleWidth =
                font.MeasureString(this.Title);

            int minWidth =
                titleWidth +
                3 * ButtonSize +
                40;

            this.Width = minWidth;
            this.Height = TitleBarHeight;
        }

        public void UpdateSize()
        {
            Font font =
                PCScreenFont.DefaultFont;

            int titleWidth =
                font.MeasureString(Title ?? "");

            int minWidth =
                titleWidth +
                3 * ButtonSize +
                40;

            int maxWidth = 0;
            int maxHeight = 0;

            for (int i = 0;
                 i < Controls.Count;
                 i++)
            {
                var c = Controls[i];

                if (c != null && c.Visible)
                {
                    int controlRight =
                        c.X + c.Width;

                    int controlBottom =
                        c.Y + c.Height;

                    if (controlRight > maxWidth)
                        maxWidth = controlRight;

                    if (controlBottom > maxHeight)
                        maxHeight = controlBottom;
                }
            }

            int calculatedWidth =
                maxWidth + PaddingX;

            this.Width =
                calculatedWidth < minWidth
                    ? minWidth
                    : calculatedWidth;

            this.Height =
                TitleBarHeight +
                maxHeight +
                PaddingY;
        }

        public void AddControl(Control control)
        {
            if (control == null)
                return;

            this.Controls.Add(control);
            UpdateSize();
        }

        public void Draw(Canvas canvas)
        {
            if (IsMinimized)
                return;

            if (Dragging)
            {
                Color borderColor =
                    Win9xBlack;

                canvas.DrawLine(
                    borderColor,
                    dragX,
                    dragY,
                    dragX + Width,
                    dragY);

                canvas.DrawLine(
                    borderColor,
                    dragX,
                    dragY + Height,
                    dragX + Width,
                    dragY + Height);

                canvas.DrawLine(
                    borderColor,
                    dragX,
                    dragY,
                    dragX,
                    dragY + Height);

                canvas.DrawLine(
                    borderColor,
                    dragX + Width,
                    dragY,
                    dragX + Width,
                    dragY + Height);

                canvas.DrawLine(
                    borderColor,
                    dragX,
                    dragY + TitleBarHeight,
                    dragX + Width,
                    dragY + TitleBarHeight);

                return;
            }

            canvas.DrawFilledRectangle(
                Win9xGray,
                X,
                Y,
                Width,
                Height);

            canvas.DrawLine(
                Win9xWhite,
                X,
                Y,
                X + Width - 1,
                Y);

            canvas.DrawLine(
                Win9xWhite,
                X,
                Y,
                X,
                Y + Height - 1);

            canvas.DrawLine(
                Win9xDarkGray,
                X + 1,
                Y + 1,
                X + Width - 2,
                Y + 1);

            canvas.DrawLine(
                Win9xDarkGray,
                X + 1,
                Y + 1,
                X + 1,
                Y + Height - 2);

            canvas.DrawLine(
                Win9xBlack,
                X,
                Y + Height - 1,
                X + Width,
                Y + Height - 1);

            canvas.DrawLine(
                Win9xBlack,
                X + Width - 1,
                Y,
                X + Width - 1,
                Y + Height);

            canvas.DrawLine(
                Win9xDarkGray,
                X + 1,
                Y + Height - 2,
                X + Width - 2,
                Y + Height - 2);

            canvas.DrawLine(
                Win9xDarkGray,
                X + Width - 2,
                Y + 1,
                X + Width - 2,
                Y + Height - 2);

            Color titleColor =
                Active
                    ? Win9xActiveTitle
                    : Win9xInactiveTitle;

            canvas.DrawFilledRectangle(
                titleColor,
                X + 2,
                Y + 2,
                Width - 4,
                TitleBarHeight - 4);

            Font font =
                PCScreenFont.DefaultFont;

            int titleTextY =
                Y +
                2 +
                ((TitleBarHeight - 4 - font.Height) / 2);

            canvas.DrawString(
                Title,
                font,
                Win9xWhite,
                X + 8,
                titleTextY);

            int closeX =
                X + Width - ButtonSize - 6;

            int closeY =
                Y + ButtonTop;

            DrawWindowButton(
                canvas,
                closeX,
                closeY,
                true);

            int minBtnX =
                closeX -
                ButtonSize -
                ButtonGap;

            int minBtnY =
                Y + ButtonTop;

            DrawWindowButton(
                canvas,
                minBtnX,
                minBtnY,
                false);

            int count =
                Controls.Count;

            for (int i = 0;
                 i < count;
                 i++)
            {
                var c = Controls[i];

                if (c != null)
                {
                    int originalX = c.X;
                    int originalY = c.Y;

                    c.X =
                        this.X +
                        originalX;

                    c.Y =
                        this.Y +
                        TitleBarHeight +
                        originalY;

                    c.Draw(canvas);

                    c.X = originalX;
                    c.Y = originalY;
                }
            }

            for (int i = 0;
                 i < count;
                 i++)
            {
                if (Controls[i] is ComboBox cb &&
                    cb.IsDropped)
                {
                    int originalX = cb.X;
                    int originalY = cb.Y;

                    cb.X =
                        this.X +
                        originalX;

                    cb.Y =
                        this.Y +
                        TitleBarHeight +
                        originalY;

                    cb.Draw(canvas);

                    cb.X = originalX;
                    cb.Y = originalY;
                }
            }
        }

        private static void DrawWindowButton(
            Canvas canvas,
            int x,
            int y,
            bool closeButton)
        {
            int size = ButtonSize;

            canvas.DrawFilledRectangle(
                Win9xGray,
                x,
                y,
                size,
                size);

            canvas.DrawLine(
                Win9xWhite,
                x,
                y,
                x + size - 1,
                y);

            canvas.DrawLine(
                Win9xWhite,
                x,
                y,
                x,
                y + size - 1);

            canvas.DrawLine(
                Win9xBlack,
                x,
                y + size - 1,
                x + size,
                y + size - 1);

            canvas.DrawLine(
                Win9xBlack,
                x + size - 1,
                y,
                x + size - 1,
                y + size);

            canvas.DrawLine(
                Win9xDarkGray,
                x + 1,
                y + size - 2,
                x + size - 2,
                y + size - 2);

            canvas.DrawLine(
                Win9xDarkGray,
                x + size - 2,
                y + 1,
                x + size - 2,
                y + size - 2);

            if (closeButton)
            {
                Color xColor =
                    Win9xBlack;

                canvas.DrawLine(
                    xColor,
                    x + 6,
                    y + 6,
                    x + size - 7,
                    y + size - 7);

                canvas.DrawLine(
                    xColor,
                    x + 6,
                    y + 7,
                    x + size - 8,
                    y + size - 7);

                canvas.DrawLine(
                    xColor,
                    x + size - 7,
                    y + 6,
                    x + 6,
                    y + size - 7);

                canvas.DrawLine(
                    xColor,
                    x + size - 7,
                    y + 7,
                    x + 7,
                    y + size - 7);
            }
            else
            {
                canvas.DrawLine(
                    Win9xBlack,
                    x + 6,
                    y + size - 8,
                    x + size - 7,
                    y + size - 8);

                canvas.DrawLine(
                    Win9xBlack,
                    x + 6,
                    y + size - 7,
                    x + size - 7,
                    y + size - 7);
            }
        }

        public bool Contains(int mx, int my) =>
            !IsMinimized &&
            mx >= X &&
            mx <= X + Width &&
            my >= Y &&
            my <= Y + Height;

        public bool TitleContains(int mx, int my) =>
            !IsMinimized &&
            mx >= X &&
            mx <= X + Width &&
            my >= Y &&
            my <= Y + TitleBarHeight;

        public bool CloseContains(int mx, int my) =>
            !IsMinimized &&
            mx >= X + Width - ButtonSize - 6 &&
            mx <= X + Width - 6 &&
            my >= Y + ButtonTop &&
            my <= Y + ButtonTop + ButtonSize;

        public bool MinimizeContains(int mx, int my) =>
            !IsMinimized &&
            mx >= X + Width -
                ButtonSize -
                ButtonGap -
                ButtonSize -
                6 &&
            mx <= X + Width -
                ButtonSize -
                ButtonGap -
                6 &&
            my >= Y + ButtonTop &&
            my <= Y + ButtonTop + ButtonSize;

        public void StartDrag(int mx, int my)
        {
            Dragging = true;

            dragOffsetX =
                mx - X;

            dragOffsetY =
                my - Y;

            dragX = X;
            dragY = Y;
        }

        public void Drag(
            int mx,
            int my,
            Canvas canvas)
        {
            if (!Dragging)
                return;

            int targetX =
                mx - dragOffsetX;

            int targetY =
                my - dragOffsetY;

            int screenWidth =
                (int)canvas.Mode.Width;

            int screenHeight =
                (int)canvas.Mode.Height;

            if (targetX < 0)
                targetX = 0;

            if (targetX + Width > screenWidth)
                targetX =
                    screenWidth - Width;

            if (targetY < 0)
                targetY = 0;

            if (targetY + Height > screenHeight)
                targetY =
                    screenHeight - Height;

            dragX = targetX;
            dragY = targetY;
        }

        public void StopDrag()
        {
            if (!Dragging)
                return;

            Dragging = false;

            X = dragX;
            Y = dragY;
        }

        public void CheckControlsClick(
            int mx,
            int my)
        {
            for (int i = 0;
                 i < Controls.Count;
                 i++)
            {
                if (Controls[i] is ComboBox cb &&
                    cb.IsDropped)
                {
                    int cbAbsX =
                        this.X + cb.X;

                    int cbAbsY =
                        this.Y +
                        TitleBarHeight +
                        cb.Y;

                    int listBottom =
                        cbAbsY +
                        cb.Height +
                        (cb.Items.Count * 36);

                    if (!(mx >= cbAbsX &&
                          mx <= cbAbsX + cb.Width &&
                          my >= cbAbsY &&
                          my <= listBottom))
                    {
                        cb.CloseDropdown();
                    }
                }
            }

            for (int i = 0;
                 i < Controls.Count;
                 i++)
            {
                if (Controls[i] != null)
                    Controls[i].Focused = false;
            }

            int count =
                Controls.Count;

            for (int i = 0;
                 i < count;
                 i++)
            {
                var c = Controls[i];

                if (c is ComboBox comboBox &&
                    comboBox.IsDropped)
                {
                    int cbAbsX =
                        this.X + comboBox.X;

                    int cbAbsY =
                        this.Y +
                        TitleBarHeight +
                        comboBox.Y;

                    int listTop =
                        cbAbsY +
                        comboBox.Height;

                    int listBottom =
                        listTop +
                        (comboBox.Items.Count * 36);

                    if (mx >= cbAbsX &&
                        mx <= cbAbsX + comboBox.Width &&
                        my >= listTop &&
                        my <= listBottom)
                    {
                        comboBox.HandleAbsoluteClick(
                            this.X,
                            this.Y,
                            mx,
                            my);

                        return;
                    }
                }
            }

            for (int i = count - 1;
                 i >= 0;
                 i--)
            {
                var c = Controls[i];

                if (c == null ||
                    !c.Visible)
                    continue;

                int absX =
                    this.X + c.X;

                int absY =
                    this.Y +
                    TitleBarHeight +
                    c.Y;

                if (mx >= absX &&
                    mx <= absX + c.Width &&
                    my >= absY &&
                    my <= absY + c.Height)
                {
                    c.Focused = true;

                    if (c is ListBox listBox)
                    {
                        listBox.HandleAbsoluteClick(
                            mx,
                            my,
                            absX,
                            absY);

                        c.Focused = true;
                        return;
                    }

                    if (c is RichTextBox richTextBox)
                    {
                        richTextBox.MouseClick(
                            mx,
                            my);
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