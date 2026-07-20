using System;
using System.Drawing;
using Cosmos.System.Graphics;
using Cosmos.System.Graphics.Fonts;

namespace HolmiumOS.GUI.Controls
{
    public class CheckBox : Control
    {
        public string Text;
        public bool Checked { get; set; } = false;
        public Action<bool> OnCheckedChanged;

        public CheckBox(string text, int x, int y)
            : base(x, y, 150, 16)
        {
            Text = text ?? "";
        }

        public override void Draw(Canvas canvas)
        {
            if (!Visible || canvas == null) return;

            Color boxBgColor = Color.White;
            Color boxBorderColor = Focused ? Color.Blue : Color.Black;

            canvas.DrawFilledRectangle(boxBgColor, X, Y, 16, 16);

            canvas.DrawLine(boxBorderColor, X, Y, X + 16, Y);
            canvas.DrawLine(boxBorderColor, X, Y + 16, X + 16, Y + 16);
            canvas.DrawLine(boxBorderColor, X, Y, X, Y + 16);
            canvas.DrawLine(boxBorderColor, X + 16, Y, X + 16, Y + 16);

            if (Checked)
            {
                canvas.DrawFilledRectangle(Color.Black, X + 4, Y + 4, 8, 8);
            }

            canvas.DrawString(Text, PCScreenFont.Default, Color.Black, X + 22, Y);
        }

        public override void Click()
        {
            this.Focused = true;
            this.Checked = !this.Checked;

            if (OnCheckedChanged != null)
            {
                OnCheckedChanged.Invoke(this.Checked);
            }
        }
    }
}