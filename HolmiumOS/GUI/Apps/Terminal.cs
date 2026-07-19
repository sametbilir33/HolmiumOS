using System;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class Terminal : AppBase
    {
        private Label testLabel;
        private Button testButton;
        private TextBox testTextBox;
        private int tiklamaSayisi = 0;

        public Terminal() : base("Terminal Test")
        {
        }

        public override void Load()
        {
            if (this.Window == null)
            {
                this.Window = new Window(this, "Terminal Test", 100, 100, 400, 300);
            }

            testLabel = new Label("Kutuya yazip butona tikla.", 20, 90);

            testTextBox = new TextBox(20, 40, 200, 25);

            testButton = new Button("Metni Aktar", 20, 130, 120, 30);

            testButton.ClickAction = OnTestButtonClick;

            if (this.Window != null)
            {
                this.Window.AddControl(testTextBox);
                this.Window.AddControl(testLabel);
                this.Window.AddControl(testButton);
            }
        }

        private void OnTestButtonClick()
        {
            tiklamaSayisi++;
            if (testLabel != null && testTextBox != null)
            {
                if (string.IsNullOrEmpty(testTextBox.Text))
                {
                    testLabel.Text = "Kutu bos! Tiklama: " + tiklamaSayisi.ToString();
                }
                else
                {
                    testLabel.Text = testTextBox.Text;
                }
            }
            Cosmos.Core.Memory.Heap.Collect();
        }
    }
}