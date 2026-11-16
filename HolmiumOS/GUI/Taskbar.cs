
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
        private static readonly Color MutedTextColor = Color.FromArgb(100, 100, 100);
        private static readonly Color AccentColor = Color.FromArgb(53, 132, 228);

        public static int Height => height + bottomMargin;

        public static int StartX(Canvas canvas)
        {
            return dockPadding;
        }

        private static int GetAppCount()
        {
            int count = 0;

            for (int i = 0; i < AppManager.apps.Count; i++)
            {
                var app = AppManager.apps[i];

                if (app == null || app.Window == null)
                    continue;

                count++;
            }

            return count;
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

            while (result.Length > 2 && MeasureText(result + "...", font) > maxWidth)
                result = result.Substring(0, result.Length - 1);

            return result + "...";
        }

        private static void DrawRoundedRect(Canvas canvas, Color color, int x, int y, int width, int height, int radius)
        {
            if (width <= 0 || height <= 0)
                return;

            if (radius <= 0)
            {
                canvas.DrawFilledRectangle(color, x, y, width, height);
                return;
            }

            radius = Math.Min(radius, Math.Min(width / 2, height / 2));

            canvas.DrawFilledRectangle(color, x + radius, y, width - radius * 2, height);
            canvas.DrawFilledRectangle(color, x, y + radius, width, height - radius * 2);

            canvas.DrawFilledCircle(color, x + radius, y + radius, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + radius, radius);
            canvas.DrawFilledCircle(color, x + radius, y + height - radius - 1, radius);
            canvas.DrawFilledCircle(color, x + width - radius - 1, y + height - radius - 1, radius);
        }

        private static void DrawRoundedOutline(Canvas canvas, Color color, int x, int y, int width, int height, int radius)
        {
            if (width <= 0 || height <= 0)
                return;

            canvas.DrawLine(color, x + radius, y, x + width - radius - 1, y);
            canvas.DrawLine(color, x + radius, y + height - 1, x + width - radius - 1, y + height - 1);
            canvas.DrawLine(color, x, y + radius, x, y + height - radius - 1);
            canvas.DrawLine(color, x + width - 1, y + radius, x + width - 1, y + height - radius - 1);

            canvas.DrawArc(color, x + radius, y + radius, radius, 180, 270);
            canvas.DrawArc(color, x + width - radius - 1, y + radius, radius, 270, 360);
            canvas.DrawArc(color, x + radius, y + height - radius - 1, radius, 90, 180);
            canvas.DrawArc(color, x + width - radius - 1, y + height - radius - 1, radius, 0, 90);
        }

        private static void DrawButton(Canvas canvas, int x, int y, int width, int buttonHeight, bool hovered, bool pressed, bool active)
        {
            Color fill = active ? ActiveColor : pressed ? ButtonPressedColor : hovered ? ButtonHoverColor : ButtonColor;
            Color border = active ? ActiveBorder : ButtonBorder;

            DrawRoundedRect(canvas, fill, x, y, width, buttonHeight, 8);
            DrawRoundedOutline(canvas, border, x, y, width, buttonHeight, 8);

            if (active)
                DrawRoundedRect(canvas, AccentColor, x + 8, y + buttonHeight - 4, width - 16, 3, 2);
        }

        public static void Draw(Canvas canvas)
        {
            if (canvas == null)
                return;

            Font font = FontManager.UiFont;
            int fontHeight = GetFontHeight(font);
            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - height - bottomMargin;

            canvas.DrawFilledRectangle(PanelColor, 0, taskbarY, screenWidth, height);
            canvas.DrawLine(PanelBorder, 0, taskbarY, screenWidth, taskbarY);

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
            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - Height;
            int clockX = screenWidth - clockBoxWidth - dockPadding;
            int clockY = taskbarY + (height - buttonHeight) / 2;

            return mx >= clockX && mx <= clockX + clockBoxWidth && my >= clockY && my <= clockY + buttonHeight;
        }

        public static void UpdateMouse(Canvas canvas)
        {
            if (canvas == null)
                return;

            int mx = (int)MouseManager.X;
            int my = (int)MouseManager.Y;
            int screenWidth = (int)canvas.Mode.Width;
            int taskbarY = (int)canvas.Mode.Height - height - bottomMargin;
            int separatorX = dockPadding + startWidth + 10;
            int clockX = screenWidth - clockBoxWidth - dockPadding;
            int buttonY = taskbarY + (height - buttonHeight) / 2;

            startHover = mx >= dockPadding && mx <= dockPadding + startWidth && my >= buttonY && my <= buttonY + buttonHeight;
            clockHover = mx >= clockX && mx <= clockX + clockBoxWidth && my >= buttonY && my <= buttonY + buttonHeight;

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

                        if (mx >= currentButtonX && mx <= currentButtonX + buttonWidth && my >= buttonY && my <= buttonY + buttonHeight)
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
