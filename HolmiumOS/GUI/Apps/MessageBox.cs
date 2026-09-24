using System;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class MessageBox : AppBase
    {
        public string Message { get; set; }
        public bool ShowInput { get; set; }
        public string InputText => txtInput != null ? txtInput.Text : "";

        private Label lblMessage;
        private TextBox txtInput;
        private Action<string> onConfirm;
        private Action onCancel;

        private string defaultInput;
        private string confirmBtnText;
        private string cancelBtnText;

        public MessageBox(string title, string message, bool showInput = false, string defaultInput = "",
                             Action<string> onConfirm = null, Action onCancel = null,
                             string confirmBtnText = "Tamam", string cancelBtnText = "Iptal")
            : base(title ?? "Bilgi")
        {
            this.Message = message ?? "";
            this.ShowInput = showInput;
            this.defaultInput = defaultInput ?? "";
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
            this.confirmBtnText = confirmBtnText;
            this.cancelBtnText = cancelBtnText;
        }

        public override void Load()
        {
            if (this.Window != null)
            {
                this.Window.Title = this.Name;
            }

            int currentY = 15;

            lblMessage = new Label(Message, 15, currentY);
            currentY += 25;

            if (ShowInput)
            {
                txtInput = new TextBox(15, currentY, 250, 25);
                txtInput.Text = defaultInput;
                txtInput.Focused = true;
                currentY += 35;
            }

            Button btnConfirm = new Button(confirmBtnText, 15, currentY, 80, 25);
            btnConfirm.ClickAction = OnConfirmClicked;

            Button btnCancel = new Button(cancelBtnText, 105, currentY, 80, 25);
            btnCancel.ClickAction = OnCancelClicked;

            if (this.Window != null)
            {
                this.Window.AddControl(lblMessage);

                if (ShowInput && txtInput != null)
                {
                    this.Window.AddControl(txtInput);
                }

                this.Window.AddControl(btnConfirm);
                this.Window.AddControl(btnCancel);
            }
        }

        private void OnConfirmClicked()
        {
            if (onConfirm != null)
            {
                onConfirm(InputText);
            }
            CloseApp();
        }

        private void OnCancelClicked()
        {
            if (onCancel != null)
            {
                onCancel();
            }
            CloseApp();
        }

        private void CloseApp()
        {
            AppManager.Close(this);
        }
    }
}