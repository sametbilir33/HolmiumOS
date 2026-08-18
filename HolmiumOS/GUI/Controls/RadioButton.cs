using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

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

        public RadioButton(Window parentWindow, string text, string groupName, int x, int y)
            : base(x, y, (text ?? "").Length * 8 + 22, 16)
        {
            this._parentWindow = parentWindow;
            this.Text = text ?? "";
            this.GroupName = groupName ?? "DefaultGroup";
            this.Checked = false;
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            int boxSize = 12;
            int boxX = X;
            int boxY = Y + (Height - boxSize) / 2;


            canvas.DrawFilledRectangle(Win9xWhite, boxX + 2, boxY + 2, boxSize - 4, boxSize - 4);
            canvas.DrawFilledRectangle(Win9xWhite, boxX + 1, boxY + 3, boxSize - 2, boxSize - 6);
            canvas.DrawFilledRectangle(Win9xWhite, boxX + 3, boxY + 1, boxSize - 6, boxSize - 2);

            canvas.DrawLine(Win9xDarkGray, boxX + 3, boxY, boxX + boxSize - 3, boxY);
            canvas.DrawLine(Win9xDarkGray, boxX, boxY + 3, boxX, boxY + boxSize - 3);
            canvas.DrawLine(Win9xBlack, boxX + 3, boxY + 1, boxX + boxSize - 3, boxY + 1);
            canvas.DrawLine(Win9xBlack, boxX + 1, boxY + 3, boxX + 1, boxY + boxSize - 3);

            canvas.DrawLine(Win9xWhite, boxX + 3, boxY + boxSize, boxX + boxSize - 3, boxY + boxSize);
            canvas.DrawLine(Win9xWhite, boxX + boxSize, boxY + 3, boxX + boxSize, boxY + boxSize - 3);
            canvas.DrawLine(Win9xGray, boxX + 2, boxY + boxSize - 1, boxX + boxSize - 2, boxY + boxSize - 1);
            canvas.DrawLine(Win9xGray, boxX + boxSize - 1, boxY + 2, boxX + boxSize - 1, boxY + boxSize - 2);

            canvas.DrawPoint(Win9xDarkGray, boxX + 2, boxY + 1);
            canvas.DrawPoint(Win9xDarkGray, boxX + 1, boxY + 2);
            canvas.DrawPoint(Win9xDarkGray, boxX + boxSize - 2, boxY + 1);
            canvas.DrawPoint(Win9xDarkGray, boxX + boxSize - 1, boxY + 2);
            canvas.DrawPoint(Win9xWhite, boxX + 1, boxY + boxSize - 2);
            canvas.DrawPoint(Win9xWhite, boxX + 2, boxY + boxSize - 1);
            canvas.DrawPoint(Win9xWhite, boxX + boxSize - 2, boxY + boxSize - 1);
            canvas.DrawPoint(Win9xWhite, boxX + boxSize - 1, boxY + boxSize - 2);

            if (Checked)
            {
                canvas.DrawFilledRectangle(Win9xBlack, boxX + 4, boxY + 4, 4, 4);
                canvas.DrawPoint(Win9xBlack, boxX + 3, boxY + 5);
                canvas.DrawPoint(Win9xBlack, boxX + 3, boxY + 6);
                canvas.DrawPoint(Win9xBlack, boxX + 8, boxY + 5);
                canvas.DrawPoint(Win9xBlack, boxX + 8, boxY + 6);
                canvas.DrawPoint(Win9xBlack, boxX + 5, boxY + 3);
                canvas.DrawPoint(Win9xBlack, boxX + 6, boxY + 3);
                canvas.DrawPoint(Win9xBlack, boxX + 5, boxY + 8);
                canvas.DrawPoint(Win9xBlack, boxX + 6, boxY + 8);
            }

            canvas.DrawString(Text, PCScreenFont.Default, Win9xBlack, X + 18, Y + 1);
        }

        public override void Click()
        {
            this.Focused = true;
            if (this.Checked) return;

            if (_parentWindow != null)
            {
                int totalControls = _parentWindow.Controls.Count;
                for (int i = 0; i < totalControls; i++)
                {
                    var ctrl = _parentWindow.Controls[i];
                    if (ctrl is RadioButton rb && rb.GroupName == this.GroupName)
                    {
                        rb.Checked = false;
                    }
                }
            }

            this.Checked = true;
        }
    }
}