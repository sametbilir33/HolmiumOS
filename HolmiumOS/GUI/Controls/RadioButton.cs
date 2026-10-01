using System.Drawing;
using Cosmos.Kernel.System.Graphics;
using Cosmos.Kernel.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class RadioButton : Control
    {
        private Window _parentWindow;
        public string Text;
        public string GroupName;
        public bool Checked { get; set; }

        private static readonly Color Win9xWhite = Color.FromArgb(255, 255, 255);
        private static readonly Color Win9xDarkGray = Color.FromArgb(128, 128, 128);
        private static readonly Color Win9xBlack = Color.FromArgb(0, 0, 0);
        private static readonly Color Win9xGray = Color.FromArgb(192, 192, 192);

        public RadioButton(
            Window parentWindow,
            string text,
            string groupName,
            int x,
            int y)
            : base(
                x,
                y,
                (text ?? "").Length * 16 + 40,
                32)
        {
            this._parentWindow = parentWindow;
            this.Text = text ?? "";
            this.GroupName = groupName ?? "DefaultGroup";
            this.Checked = false;
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            Font font = PCScreenFont.DefaultFont;

            int boxSize = 24;
            int boxX = X;
            int boxY = Y + (Height - boxSize) / 2;

            canvas.DrawFilledRectangle(
                Win9xWhite,
                boxX + 4,
                boxY + 4,
                boxSize - 8,
                boxSize - 8);

            canvas.DrawFilledRectangle(
                Win9xWhite,
                boxX + 2,
                boxY + 6,
                boxSize - 4,
                boxSize - 12);

            canvas.DrawFilledRectangle(
                Win9xWhite,
                boxX + 6,
                boxY + 2,
                boxSize - 12,
                boxSize - 4);

            canvas.DrawLine(
                Win9xDarkGray,
                boxX + 6,
                boxY,
                boxX + boxSize - 6,
                boxY);

            canvas.DrawLine(
                Win9xDarkGray,
                boxX,
                boxY + 6,
                boxX,
                boxY + boxSize - 6);

            canvas.DrawLine(
                Win9xBlack,
                boxX + 6,
                boxY + 1,
                boxX + boxSize - 6,
                boxY + 1);

            canvas.DrawLine(
                Win9xBlack,
                boxX + 1,
                boxY + 6,
                boxX + 1,
                boxY + boxSize - 6);

            canvas.DrawLine(
                Win9xWhite,
                boxX + 6,
                boxY + boxSize,
                boxX + boxSize - 6,
                boxY + boxSize);

            canvas.DrawLine(
                Win9xWhite,
                boxX + boxSize,
                boxY + 6,
                boxX + boxSize,
                boxY + boxSize - 6);

            canvas.DrawLine(
                Win9xGray,
                boxX + 4,
                boxY + boxSize - 1,
                boxX + boxSize - 4,
                boxY + boxSize - 1);

            canvas.DrawLine(
                Win9xGray,
                boxX + boxSize - 1,
                boxY + 4,
                boxX + boxSize - 1,
                boxY + boxSize - 4);

            if (Checked)
            {
                int dotSize = 10;
                int dotX = boxX + (boxSize - dotSize) / 2;
                int dotY = boxY + (boxSize - dotSize) / 2;

                canvas.DrawFilledRectangle(
                    Win9xBlack,
                    dotX,
                    dotY,
                    dotSize,
                    dotSize);
            }

            int textX = X + boxSize + 10;
            int textY = Y + (Height - font.Height) / 2;

            canvas.DrawString(
                Text,
                font,
                Win9xBlack,
                textX,
                textY);
        }

        public override void Click()
        {
            this.Focused = true;

            if (this.Checked)
                return;

            if (_parentWindow != null)
            {
                int totalControls = _parentWindow.Controls.Count;

                for (int i = 0; i < totalControls; i++)
                {
                    var ctrl = _parentWindow.Controls[i];

                    if (ctrl is RadioButton rb &&
                        rb.GroupName == this.GroupName)
                    {
                        rb.Checked = false;
                    }
                }
            }

            this.Checked = true;
        }
    }
}