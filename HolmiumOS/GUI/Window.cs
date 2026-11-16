
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

        public List<Control> Controls { get; set; } = new List<Control>();

        public const int TitleBarHeight = 40;

        private const int PaddingX = 20;
        private const int PaddingY = 25;

        private const int ButtonSize = 26;
        private const int ButtonGap = 4;
        private const int ButtonTop = 7;
        private const int WindowRadius = 9;

        private int dragX, dragY;
        private int dragOffsetX, dragOffsetY;

        private static readonly Color WindowColor = Color.FromArgb(255, 255, 255);
        private static readonly Color TitleBarColor = Color.FromArgb(246, 245, 244);
        private static readonly Color BorderColor = Color.FromArgb(190, 189, 188);
        private static readonly Color ActiveBorderColor = Color.FromArgb(53, 132, 228);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);
        private static readonly Color MutedTextColor = Color.FromArgb(110, 110, 110);
        private static readonly Color HoverColor = Color.FromArgb(235, 235, 234);
        private static readonly Color CloseHoverColor = Color.FromArgb(192, 40, 40);
        private static readonly Color CloseHoverTextColor = Color.FromArgb(255, 255, 255);
        private static readonly Color ShadowColor = Color.FromArgb(65, 0, 0, 0);

        public Window(AppBase app, string title, int x, int y)
        {
            App = app;
            Title = title ?? "Uygulama";
            X = x;
            Y = y;

            Font font = FontManager.UiFont;
            int titleWidth = MeasureText(Title, font);
            int minWidth = titleWidth + 3 * ButtonSize + 40;

            Width = minWidth;
            Height = TitleBarHeight;
        }

        private static int MeasureText(string text, Font font)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.MeasureString(text ?? "", FontManager.UiFontSize);

            return font.MeasureString(text ?? "");
        }

        private static int GetFontHeight(Font font)
        {
            if (font is TrueTypeFont trueTypeFont)
                return trueTypeFont.GetLineHeight(FontManager.UiFontSize);

            return font.Height;
        }

        private static void DrawText(Canvas canvas, string text, Font font, Color color, int x, int y)
        {
            if (font is TrueTypeFont trueTypeFont)
                canvas.DrawString(text ?? "", trueTypeFont, FontManager.UiFontSize, color, x, y);
            else
                canvas.DrawString(text ?? "", font, color, x, y);
        }

        private static void DrawRoundedRect(Canvas canvas, Color color, int x, int y, int width, int height, int radius)
        {
            if (width <= 0 || height <= 0)
                return;

            radius = System.Math.Min(radius, System.Math.Min(width / 2, height / 2));

            if (radius <= 0)
            {
                canvas.DrawFilledRectangle(color, x, y, width, height);
                return;
            }

            canvas.DrawFilledRectangle(color, x + radius, y, width - radius * 2, height);
            canvas.DrawFilledRectangle(color, x, y + radius, width, height - radius * 2);

            canvas.DrawFilledCircle(color, x + radius, y + radius, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
            canvas.DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        private static void DrawRoundedOutline(Canvas canvas, Color color, int x, int y, int width, int height, int radius)
        {
            canvas.DrawLine(color, x + radius, y, x + width - radius - 1, y);
            canvas.DrawLine(color, x + radius, y + height - 1, x + width - radius - 1, y + height - 1);
            canvas.DrawLine(color, x, y + radius, x, y + height - radius - 1);
            canvas.DrawLine(color, x + width - 1, y + radius, x + width - 1, y + height - radius - 1);

            canvas.DrawArc(color, x + radius, y + radius, radius, 180, 270);
            canvas.DrawArc(color, x + width - radius - 1, y + radius, radius, 270, 360);
            canvas.DrawArc(color, x + radius, y + height - radius - 1, radius, 90, 180);
            canvas.DrawArc(color, x + width - radius - 1, y + height - radius - 1, radius, 0, 90);
        }

        public void UpdateSize()
        {
            Font font = FontManager.UiFont;
            int titleWidth = MeasureText(Title ?? "", font);
            int minWidth = titleWidth + 3 * ButtonSize + 40;

            int maxWidth = 0;
            int maxHeight = 0;

            for (int i = 0; i < Controls.Count; i++)
            {
                var c = Controls[i];

                if (c != null && c.Visible)
                {
                    int controlRight = c.X + c.Width;
                    int controlBottom = c.Y + c.Height;

                    if (controlRight > maxWidth)
                        maxWidth = controlRight;

                    if (controlBottom > maxHeight)
                        maxHeight = controlBottom;
                }
            }

            int calculatedWidth = maxWidth + PaddingX;

            Width = calculatedWidth < minWidth ? minWidth : calculatedWidth;
            Height = TitleBarHeight + maxHeight + PaddingY;
        }

        public void AddControl(Control control)
        {
            if (control == null)
                return;

            Controls.Add(control);
            UpdateSize();
        }

        private void DrawWindowButtons(Canvas canvas)
        {
            int closeX = X + Width - ButtonSize - 7;
            int minX = closeX - ButtonSize - ButtonGap;
            int buttonY = Y + ButtonTop;

            DrawRoundedRect(canvas, HoverColor, minX, buttonY, ButtonSize, ButtonSize, 6);
            DrawRoundedRect(canvas, HoverColor, closeX, buttonY, ButtonSize, ButtonSize, 6);

            Color iconColor = MutedTextColor;

            canvas.DrawLine(iconColor, minX + 7, buttonY + 17, minX + ButtonSize - 7, buttonY + 17);
            canvas.DrawLine(iconColor, minX + 7, buttonY + 18, minX + ButtonSize - 7, buttonY + 18);

            canvas.DrawLine(iconColor, closeX + 8, buttonY + 8, closeX + ButtonSize - 8, buttonY + ButtonSize - 8);
            canvas.DrawLine(iconColor, closeX + 9, buttonY + 8, closeX + ButtonSize - 8, buttonY + ButtonSize - 9);
            canvas.DrawLine(iconColor, closeX + ButtonSize - 8, buttonY + 8, closeX + 8, buttonY + ButtonSize - 8);
            canvas.DrawLine(iconColor, closeX + ButtonSize - 9, buttonY + 8, closeX + 8, buttonY + ButtonSize - 9);
        }

        public void Draw(Canvas canvas)
        {
            if (IsMinimized)
                return;

            if (Dragging)
            {
                Color dragBorderColor = ActiveBorderColor;

                canvas.DrawLine(dragBorderColor, dragX, dragY, dragX + Width, dragY);
                canvas.DrawLine(dragBorderColor, dragX, dragY + Height, dragX + Width, dragY + Height);
                canvas.DrawLine(dragBorderColor, dragX, dragY, dragX, dragY + Height);
                canvas.DrawLine(dragBorderColor, dragX + Width, dragY, dragX + Width, dragY + Height);
                canvas.DrawLine(dragBorderColor, dragX, dragY + TitleBarHeight, dragX + Width, dragY + TitleBarHeight);

                return;
            }

            DrawRoundedRect(canvas, ShadowColor, X + 2, Y + 3, Width, Height, WindowRadius);
            DrawRoundedRect(canvas, WindowColor, X, Y, Width, Height, WindowRadius);

            Color borderColor = Active ? ActiveBorderColor : BorderColor;
            DrawRoundedOutline(canvas, borderColor, X, Y, Width, Height, WindowRadius);

            DrawRoundedRect(canvas, TitleBarColor, X + 1, Y + 1, Width - 2, TitleBarHeight, WindowRadius);

            canvas.DrawLine(BorderColor, X + 1, Y + TitleBarHeight, X + Width - 2, Y + TitleBarHeight);

            Font font = FontManager.UiFont;
            int fontHeight = GetFontHeight(font);

            int titleTextY = Y + (TitleBarHeight - fontHeight) / 2;
            DrawText(canvas, Title ?? "", font, TextColor, X + 14, titleTextY);

            DrawWindowButtons(canvas);

            int count = Controls.Count;

            for (int i = 0; i < count; i++)
            {
                var c = Controls[i];

                if (c == null || !c.Visible)
                    continue;

                int originalX = c.X;
                int originalY = c.Y;

                c.X = X + originalX;
                c.Y = Y + TitleBarHeight + originalY;

                c.Draw(canvas);

                c.X = originalX;
                c.Y = originalY;
            }

            for (int i = 0; i < count; i++)
            {
                if (Controls[i] is ComboBox cb && cb.IsDropped)
                {
                    int originalX = cb.X;
                    int originalY = cb.Y;

                    cb.X = X + originalX;
                    cb.Y = Y + TitleBarHeight + originalY;

                    cb.Draw(canvas);

                    cb.X = originalX;
                    cb.Y = originalY;
                }
            }
        }

        public bool Contains(int mx, int my) =>
            !IsMinimized && mx >= X && mx <= X + Width && my >= Y && my <= Y + Height;

        public bool TitleContains(int mx, int my) =>
            !IsMinimized && mx >= X && mx <= X + Width && my >= Y && my <= Y + TitleBarHeight;

        public bool CloseContains(int mx, int my) =>
            !IsMinimized &&
            mx >= X + Width - ButtonSize - 7 &&
            mx <= X + Width - 7 &&
            my >= Y + ButtonTop &&
            my <= Y + ButtonTop + ButtonSize;

        public bool MinimizeContains(int mx, int my) =>
            !IsMinimized &&
            mx >= X + Width - ButtonSize - ButtonGap - ButtonSize - 7 &&
            mx <= X + Width - ButtonSize - ButtonGap - 7 &&
            my >= Y + ButtonTop &&
            my <= Y + ButtonTop + ButtonSize;

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
            if (!Dragging)
                return;

            int targetX = mx - dragOffsetX;
            int targetY = my - dragOffsetY;

            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;

            if (targetX < 0)
                targetX = 0;

            if (targetX + Width > screenWidth)
                targetX = screenWidth - Width;

            if (targetY < 0)
                targetY = 0;

            if (targetY + Height > screenHeight)
                targetY = screenHeight - Height;

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

        public void CheckControlsClick(int mx, int my)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is ComboBox cb && cb.IsDropped)
                {
                    int cbAbsX = X + cb.X;
                    int cbAbsY = Y + TitleBarHeight + cb.Y;
                    int listBottom = cbAbsY + cb.Height + cb.Items.Count * 36;

                    if (!(mx >= cbAbsX && mx <= cbAbsX + cb.Width && my >= cbAbsY && my <= listBottom))
                        cb.CloseDropdown();
                }
            }

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] != null)
                    Controls[i].Focused = false;
            }

            int count = Controls.Count;

            for (int i = 0; i < count; i++)
            {
                if (Controls[i] is ComboBox comboBox && comboBox.IsDropped)
                {
                    int cbAbsX = X + comboBox.X;
                    int cbAbsY = Y + TitleBarHeight + comboBox.Y;
                    int listTop = cbAbsY + comboBox.Height;
                    int listBottom = listTop + comboBox.Items.Count * 36;

                    if (mx >= cbAbsX && mx <= cbAbsX + comboBox.Width && my >= listTop && my <= listBottom)
                    {
                        comboBox.HandleAbsoluteClick(X, Y, mx, my);
                        return;
                    }
                }
            }

            for (int i = count - 1; i >= 0; i--)
            {
                var c = Controls[i];

                if (c == null || !c.Visible)
                    continue;

                int absX = X + c.X;
                int absY = Y + TitleBarHeight + c.Y;

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
                        richTextBox.MouseClick(mx, my);
                    else
                        c.Click();

                    break;
                }
            }
        }
    }
}
