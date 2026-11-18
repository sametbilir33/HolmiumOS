
using System;
using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using Cosmos.Kernel.System.Input;

namespace HolmiumOS.GUI
{
    public static class Taskbar
    {
        public static bool MenuOpen;

        private static bool lastPressed;
        private static bool startHover;
        private static bool clockHover;

        private const int height = 48;
        private const int bottomMargin = 0;
        private const int dockPadding = 8;

        private const int startWidth = 76;
        private const int buttonWidth = 180;
        private const int buttonHeight = 36;
        private const int buttonSpacing = 6;

        private const int clockBoxWidth = 84;

        private static readonly Color PanelColor = Color.FromArgb(246, 245, 244);
        private static readonly Color PanelBorder = Color.FromArgb(205, 204, 203);
        private static readonly Color ButtonColor = Color.FromArgb(246, 245, 244);
        private static readonly Color ButtonHoverColor = Color.FromArgb(235, 235, 234);
        private static readonly Color ButtonPressedColor = Color.FromArgb(222, 221, 220);
        private static readonly Color ButtonBorder = Color.FromArgb(205, 204, 203);
        private static readonly Color ActiveColor = Color.FromArgb(220, 235, 250);
        private static readonly Color ActiveBorder = Color.FromArgb(53, 132, 228);
        private static readonly Color TextColor = Color.FromArgb(36, 36, 36);
        private static readonly Color AccentColor = Color.FromArgb(53, 132, 228);

        public static int Height => height + bottomMargin;

        public static int StartX(Canvas canvas)
        {
            return dockPadding;
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

        private static string FitTitle(string title, Font font, int maxWidth)
        {
            if (string.IsNullOrEmpty(title))
                return "";

            if (MeasureText(title, font) <= maxWidth)
                return title;

            string result = title;

            while (result.Length > 0 && MeasureText(result + "...", font) > maxWidth)
                result = result.Substring(0, result.Length - 1);

            return result + "...";
        }

        private static int GetCornerInset(int row, int rectHeight, int radius)
        {
            if (radius <= 1) return 0;

            int cornerY = -1;

            if (row < radius)
                cornerY = radius - 1 - row;
            else if (row >= rectHeight - radius)
                cornerY = row - (rectHeight - radius);

            if (cornerY < 0) return 0;

            int r = radius - 1;
            int x = (int)Math.Sqrt(r * r - cornerY * cornerY);

            return r - x;
        }

        private static void DrawRoundedRect(Canvas canvas, Color color, int x, int y, int width, int rectHeight, int radius)
        {
            if (width <= 0 || rectHeight <= 0) return;

            radius = Math.Max(0, Math.Min(radius, Math.Min(width, rectHeight) / 2));

            if (radius == 0)
            {
                canvas.DrawFilledRectangle(color, x, y, width, rectHeight);
                return;
            }

            for (int row = 0; row < rectHeight; row++)
            {
                int inset = GetCornerInset(row, rectHeight, radius);
                canvas.DrawLine(color, x + inset, y + row, x + width - inset - 1, y + row);
            }
        }

        private static void DrawRoundedOutline(Canvas canvas, Color color, int x, int y, int width, int rectHeight, int radius)
        {
            if (width <= 0 || rectHeight <= 0) return;

            radius = Math.Max(0, Math.Min(radius, Math.Min(width, rectHeight) / 2));

            if (radius == 0)
            {
                canvas.DrawRectangle(color, x, y, width, rectHeight);
                return;
            }

            for (int row = 0; row < rectHeight; row++)
            {
                int inset = GetCornerInset(row, rectHeight, radius);

                canvas.DrawPoint(color, x + inset, y + row);
                canvas.DrawPoint(color, x + width - inset - 1, y + row);
            }

            canvas.DrawLine(color, x + radius - 1, y, x + width - radius, y);
            canvas.DrawLine(color, x + radius - 1, y + rectHeight - 1, x + width - radius, y + rectHeight - 1);
        }

        private static void DrawButton(Canvas canvas, int x, int y, int width, int buttonHeight, bool hovered, bool pressed, bool active)
        {
            Color fill = active ? ActiveColor : pressed ? ButtonPressedColor : hovered ? ButtonHoverColor : ButtonColor;
            Color border = active ? ActiveBorder : ButtonBorder;

            DrawRoundedRect(canvas, fill, x, y, width, buttonHeight, 8);
            DrawRoundedOutline(canvas, border, x, y, width, buttonHeight, 8);

            if (active)
                DrawRoundedRect(canvas, ActiveBorder, x + 8, y + buttonHeight - 4, width - 16, 3, 1);
        }

        public static void Draw(Canvas canvas)
        {
            if (canvas == null) return;

            Font font = FontManager.UiFont;
            int fontHeight = GetFontHeight(font);
            int screenWidth = (int)canvas.Mode.Width;
            int screenHeight = (int)canvas.Mode.Height;
            int taskbarY = screenHeight - height - bottomMargin;

            canvas.DrawFilledRectangle(PanelColor, 0, taskbarY, screenWidth, height);
            canvas.DrawLine(PanelBorder, 0, taskbarY, screenWidth - 1, taskbarY);

            int startY = taskbarY + (height - buttonHeight) / 2;

            DrawButton(canvas, dockPadding, startY, startWidth, buttonHeight, startHover, MenuOpen, MenuOpen);

            int startTextWidth = MeasureText("Uygulamalar", font);
            string startText = startWidth >= startTextWidth + 12 ? "Uygulamalar" : "Menu";
            startTextWidth = MeasureText(startText, font);

            DrawText(canvas, startText, font, TextColor, dockPadding + (startWidth - startTextWidth) / 2, startY + (buttonHeight - fontHeight) / 2);

            int separatorX = dockPadding + startWidth + 10;
            int clockX = screenWidth - clockBoxWidth - dockPadding;
            int clockY = startY;

            canvas.DrawLine(PanelBorder, separatorX, taskbarY + 10, separatorX, taskbarY + height - 10);

            int currentButtonX = separatorX + 10;
            int maxButtonAllowedX = clockX - 10;

            for (int i = 0; i < AppManager.apps.Count; i++)
            {
                var app = AppManager.apps[i];

                if (app == null || app.Window == null)
                    continue;

                if (currentButtonX + buttonWidth > maxButtonAllowedX)
                    break;

                var win = app.Window;
                bool active = WindowManager.activeWindow == win && !win.IsMinimized;

                DrawButton(canvas, currentButtonX, startY, buttonWidth, buttonHeight, false, active, active);

                string title = FitTitle(win.Title, font, buttonWidth - 20);
                int titleWidth = MeasureText(title, font);
                int titleX = currentButtonX + (buttonWidth - titleWidth) / 2;
                int titleY = startY + (buttonHeight - fontHeight) / 2;

                DrawText(canvas, title, font, TextColor, titleX, titleY);

                currentButtonX += buttonWidth + buttonSpacing;
            }

            Color clockFill = clockHover ? ButtonHoverColor : ButtonColor;

            DrawRoundedRect(canvas, clockFill, clockX, clockY, clockBoxWidth, buttonHeight, 8);
            DrawRoundedOutline(canvas, ButtonBorder, clockX, clockY, clockBoxWidth, buttonHeight, 8);

            DateTime now = DateTime.Now;
            string timeString = $"{now.Hour:D2}:{now.Minute:D2}";
            int timeWidth = MeasureText(timeString, font);
            int timeX = clockX + (clockBoxWidth - timeWidth) / 2;
            int timeY = clockY + (buttonHeight - fontHeight) / 2;

            DrawText(canvas, timeString, font, TextColor, timeX, timeY);

            if (MenuOpen)
                TaskbarMenu.Draw(canvas);
        }

        public static bool ContainsClock(int mx, int my, Canvas canvas)
        {
            if (canvas == null) return false;

            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - Height;
            int clockX = screenWidth - clockBoxWidth - dockPadding;
            int clockY = taskbarY + (height - buttonHeight) / 2;

            return mx >= clockX && mx < clockX + clockBoxWidth && my >= clockY && my < clockY + buttonHeight;
        }

        public static void UpdateMouse(Canvas canvas)
        {
            if (canvas == null) return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - height - bottomMargin;
            int separatorX = dockPadding + startWidth + 10;
            int clockX = screenWidth - clockBoxWidth - dockPadding;
            int buttonY = taskbarY + (height - buttonHeight) / 2;

            startHover = mx >= dockPadding && mx < dockPadding + startWidth && my >= buttonY && my < buttonY + buttonHeight;
            clockHover = mx >= clockX && mx < clockX + clockBoxWidth && my >= buttonY && my < buttonY + buttonHeight;

            if (MenuOpen)
                TaskbarMenu.UpdateHover(mx, my);

            bool pressed = MouseManager.LeftButton;

            if (pressed && !lastPressed)
            {
                if (startHover)
                {
                    MenuOpen = !MenuOpen;
                }
                else
                {
                    int currentButtonX = separatorX + 10;
                    bool clickedWindow = false;

                    for (int i = 0; i < AppManager.apps.Count; i++)
                    {
                        var app = AppManager.apps[i];

                        if (app == null || app.Window == null)
                            continue;

                        if (currentButtonX + buttonWidth > clockX - 10)
                            break;

                        if (mx >= currentButtonX && mx < currentButtonX + buttonWidth && my >= buttonY && my < buttonY + buttonHeight)
                        {
                            var win = app.Window;

                            if (win.IsMinimized)
                                WindowManager.Restore(win);
                            else if (WindowManager.activeWindow == win)
                                WindowManager.Minimize(win);
                            else
                                WindowManager.Focus(win);

                            clickedWindow = true;
                            break;
                        }

                        currentButtonX += buttonWidth + buttonSpacing;
                    }

                    if (!clickedWindow && MenuOpen)
                    {
                        if (TaskbarMenu.IsInside(mx, my))
                            TaskbarMenu.Click(mx, my);
                        else
                            MenuOpen = false;
                    }
                }
            }

            lastPressed = pressed;
        }
    }
}