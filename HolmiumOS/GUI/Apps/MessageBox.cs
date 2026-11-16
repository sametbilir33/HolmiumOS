
using System;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class MessageBox : AppBase
    {
        public string Message { get; set; }
        public bool ShowInput { get; set; }

        public string InputText => txtInput?.Text ?? "";

        private Label? lblMessage;
        private TextBox? txtInput;

        private Action<string>? onConfirm;
        private Action? onCancel;

        private string defaultInput;
        private string confirmBtnText;
        private string cancelBtnText;

        public MessageBox(
            string title,
            string message,
            bool showInput = false,
            string defaultInput = "",
            Action<string>? onConfirm = null,
            Action? onCancel = null,
            string confirmBtnText = "Tamam",
            string cancelBtnText = "İptal")
            : base(title ?? "Bilgi")
        {
            Message = message ?? "";
            ShowInput = showInput;
            this.defaultInput = defaultInput ?? "";
            this.onConfirm = onConfirm;
            this.onCancel = onCancel;
            this.confirmBtnText = confirmBtnText;
            this.cancelBtnText = cancelBtnText;
        }

        public override void Load()
        {
            if (Window == null)
                return;

            Window.Title = Name;

            int currentY = 20;

            lblMessage = new Label(Message, 20, currentY);
            currentY += 55;

            if (ShowInput)
            {
                txtInput = new TextBox(20, currentY, 300, 42);
                txtInput.Text = defaultInput;
                txtInput.Focused = true;
                Window.AddControl(txtInput);

                currentY += 60;
            }

            Button btnConfirm = new Button(confirmBtnText, 20, currentY, 145, 48);
            btnConfirm.ClickAction = OnConfirmClicked;

            Button btnCancel = new Button(cancelBtnText, 175, currentY, 145, 48);
            btnCancel.ClickAction = OnCancelClicked;

            Window.AddControl(lblMessage);
            Window.AddControl(btnConfirm);
            Window.AddControl(btnCancel);
        }

        private void OnConfirmClicked()
        {
            onConfirm?.Invoke(InputText);
            CloseApp();
        }

        private void OnCancelClicked()
        {
            onCancel?.Invoke();
            CloseApp();
        }

        private void CloseApp()
        {
            AppManager.Close(this);
        }

        public override void Close()
        {
        }
    }
}