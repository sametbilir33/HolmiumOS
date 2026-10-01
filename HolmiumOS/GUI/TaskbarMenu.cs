using System.Drawing;
using Cosmos.Kernel.System;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI
{
    public static class TaskbarMenu
    {
        private static bool aboutHover;
        private static bool terminalHover;
        private static bool fileManagerHover;
        private static bool calculatorHover;
        private static bool notepadHover;
        private static bool badAppleHover;
        private static bool taskManagerHover;
        private static bool rebootHover;
        private static bool shutdownHover;

        private const int menuWidth = 300;
        private const int menuHeight = 500;

        private const int bannerWidth = 36;

        private const int buttonWidth = 250;
        private const int buttonHeight = 40;
        private const int buttonSpacing = 2;

        private static int GetMenuX(Canvas canvas)
        {
            return 4;
        }

        private static int GetMenuY(Canvas canvas)
        {
            return (int)canvas.Mode.Height -
                   Taskbar.Height -
                   menuHeight -
                   4;
        }

        private static void DrawRaisedBox(
            Canvas canvas,
            int x,
            int y,
            int width,
            int height)
        {
            canvas.DrawFilledRectangle(
                Color.FromArgb(192, 192, 192),
                x,
                y,
                width,
                height);

            canvas.DrawLine(
                Color.FromArgb(255, 255, 255),
                x,
                y,
                x + width - 1,
                y);

            canvas.DrawLine(
                Color.FromArgb(255, 255, 255),
                x,
                y,
                x,
                y + height - 1);

            canvas.DrawLine(
                Color.FromArgb(0, 0, 0),
                x,
                y + height - 1,
                x + width - 1,
                y + height - 1);

            canvas.DrawLine(
                Color.FromArgb(0, 0, 0),
                x + width - 1,
                y,
                x + width - 1,
                y + height - 1);

            canvas.DrawLine(
                Color.FromArgb(128, 128, 128),
                x + 1,
                y + height - 2,
                x + width - 2,
                y + height - 2);

            canvas.DrawLine(
                Color.FromArgb(128, 128, 128),
                x + width - 2,
                y + 1,
                x + width - 2,
                y + height - 2);
        }

        private static void DrawButton(
            Canvas canvas,
            string text,
            int x,
            int y,
            bool hover)
        {
            Color background =
                hover
                    ? Color.FromArgb(0, 0, 128)
                    : Color.FromArgb(192, 192, 192);

            Color textColor =
                hover
                    ? Color.White
                    : Color.Black;

            canvas.DrawFilledRectangle(
                background,
                x,
                y,
                buttonWidth,
                buttonHeight);

            Font font =
                PCScreenFont.DefaultFont;

            int textWidth =
                font.MeasureString(text);

            int textX =
                x +
                (buttonWidth - textWidth) / 2;

            int textY =
                y +
                (buttonHeight - font.Height) / 2;

            canvas.DrawString(
                text,
                font,
                textColor,
                textX,
                textY);
        }

        public static void Draw(Canvas canvas)
        {
            int x = GetMenuX(canvas);
            int y = GetMenuY(canvas);

            DrawRaisedBox(
                canvas,
                x,
                y,
                menuWidth,
                menuHeight);

            canvas.DrawFilledRectangle(
                Color.FromArgb(0, 0, 128),
                x + 2,
                y + 2,
                bannerWidth,
                menuHeight - 4);

            Font font =
                PCScreenFont.DefaultFont;

            string bannerText =
                "HolmiumOS";

            int bannerHeight =
                bannerText.Length *
                font.Height;

            int bannerStartY =
                y +
                (menuHeight - bannerHeight) / 2;

            for (int i = 0;
                 i < bannerText.Length;
                 i++)
            {
                string letter =
                    bannerText[i].ToString();

                int letterWidth =
                    font.MeasureString(letter);

                int letterX =
                    x +
                    2 +
                    (bannerWidth - letterWidth) / 2;

                int letterY =
                    bannerStartY +
                    (i * font.Height);

                canvas.DrawString(
                    letter,
                    font,
                    Color.White,
                    letterX,
                    letterY);
            }

            int contentX =
                x +
                bannerWidth +
                6;

            int buttonY =
                y + 8;

            DrawButton(
                canvas,
                "About",
                contentX,
                buttonY,
                aboutHover);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            DrawButton(
                canvas,
                "Terminal",
                contentX,
                buttonY,
                terminalHover);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            DrawButton(
                canvas,
                "File Manager",
                contentX,
                buttonY,
                fileManagerHover);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            DrawButton(
                canvas,
                "Calculator",
                contentX,
                buttonY,
                calculatorHover);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            DrawButton(
                canvas,
                "Notepad",
                contentX,
                buttonY,
                notepadHover);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            DrawButton(
                canvas,
                "Bad Apple!!",
                contentX,
                buttonY,
                badAppleHover);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            DrawButton(
                canvas,
                "Task Manager",
                contentX,
                buttonY,
                taskManagerHover);

            buttonY +=
                buttonHeight +
                8;

            canvas.DrawLine(
                Color.FromArgb(128, 128, 128),
                contentX,
                buttonY,
                contentX + buttonWidth - 4,
                buttonY);

            canvas.DrawLine(
                Color.FromArgb(255, 255, 255),
                contentX,
                buttonY + 1,
                contentX + buttonWidth - 4,
                buttonY + 1);

            buttonY += 8;

            DrawButton(
                canvas,
                "Reboot",
                contentX,
                buttonY,
                rebootHover);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            DrawButton(
                canvas,
                "Shutdown",
                contentX,
                buttonY,
                shutdownHover);
        }

        public static bool IsInside(
            int mouseX,
            int mouseY)
        {
            int x =
                GetMenuX(Init.canvas);

            int y =
                GetMenuY(Init.canvas);

            return mouseX >= x &&
                   mouseX <= x + menuWidth &&
                   mouseY >= y &&
                   mouseY <= y + menuHeight;
        }

        public static void UpdateHover(
            int mouseX,
            int mouseY)
        {
            int x =
                GetMenuX(Init.canvas);

            int y =
                GetMenuY(Init.canvas);

            int contentX =
                x +
                bannerWidth +
                6;

            int buttonY =
                y + 8;

            aboutHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            terminalHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            fileManagerHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            calculatorHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            notepadHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            badAppleHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            taskManagerHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);

            buttonY +=
                buttonHeight +
                16;

            rebootHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);

            buttonY +=
                buttonHeight +
                buttonSpacing;

            shutdownHover =
                IsButtonInside(
                    mouseX,
                    mouseY,
                    contentX,
                    buttonY);
        }

        private static bool IsButtonInside(
            int mouseX,
            int mouseY,
            int buttonX,
            int buttonY)
        {
            return mouseX >= buttonX &&
                   mouseX <= buttonX + buttonWidth &&
                   mouseY >= buttonY &&
                   mouseY <= buttonY + buttonHeight;
        }

        public static void Click(
            int mouseX,
            int mouseY)
        {
            if (aboutHover)
            {
                AppManager.Run<Apps.About>(
                    70,
                    70);

                Taskbar.MenuOpen = false;
            }
            else if (terminalHover)
            {
                AppManager.Run<Apps.Terminal>(
                    50,
                    50);

                Taskbar.MenuOpen = false;
            }
            else if (fileManagerHover)
            {
                var fileManager =
                    new Apps.FileManager(
                        UserManager.HomeDirectory);

                AppManager.Run(fileManager);

                Taskbar.MenuOpen = false;
            }
            else if (calculatorHover)
            {
                AppManager.Run<Apps.Calculator>(
                    50,
                    50);

                Taskbar.MenuOpen = false;
            }
            else if (notepadHover)
            {
                var notepad =
                    new Apps.Notepad(null);

                AppManager.Run(notepad);

                Taskbar.MenuOpen = false;
            }
            else if (badAppleHover)
            {
                AppManager.Run<Apps.BadApple>(
                    40,
                    40);

                Taskbar.MenuOpen = false;
            }
            else if (taskManagerHover)
            {
                AppManager.Run<Apps.TaskManager>(
                    80,
                    60);

                Taskbar.MenuOpen = false;
            }
            else if (rebootHover)
            {
                Taskbar.MenuOpen = false;
                Power.Reboot();
            }
            else if (shutdownHover)
            {
                Taskbar.MenuOpen = false;
                Power.Shutdown();
            }
        }
    }
}