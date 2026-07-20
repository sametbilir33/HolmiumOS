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

        public RadioButton(Window parentWindow, string text, string groupName, int x, int y)
            : base(x, y, (text ?? "").Length * 8 + 20, 16)
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

            canvas.DrawFilledRectangle(Color.White, boxX + 1, boxY + 1, boxSize - 1, boxSize - 1);

            canvas.DrawLine(Color.Black, boxX, boxY, boxX + boxSize, boxY);
            canvas.DrawLine(Color.Black, boxX, boxY + boxSize, boxX + boxSize, boxY + boxSize);
            canvas.DrawLine(Color.Black, boxX, boxY, boxX, boxY + boxSize);
            canvas.DrawLine(Color.Black, boxX + boxSize, boxY, boxX + boxSize, boxY + boxSize);

            if (Checked)
            {
                canvas.DrawFilledRectangle(Color.Blue, boxX + 3, boxY + 3, boxSize - 5, boxSize - 5);
            }

            canvas.DrawString(Text, PCScreenFont.Default, Color.Black, X + 18, Y);
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