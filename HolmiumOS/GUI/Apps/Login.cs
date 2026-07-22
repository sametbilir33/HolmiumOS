using System;
using HolmiumOS.GUI.Controls;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI.Apps
{
    public class Login : AppBase
    {
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private Button loginButton;
        private Label statusLabel;
        private bool isSuccess;

        public Login() : base("Giris Yap")
        {
            this.isSuccess = false;
        }

        public override void Load()
        {
            if (this.Window != null)
            {
                this.Window.Title = "Giris Yap";
            }

            Label welcomeLabel = new Label("HolmiumOS'e Hos Geldiniz", 20, 20);

            Label userTitle = new Label("Kullanici Adi:", 20, 55);
            usernameTextBox = new TextBox(20, 75, 200, 25);
            usernameTextBox.Text = "";
            usernameTextBox.MaxLength = 20;

            Label passTitle = new Label("Sifre:", 20, 110);
            passwordTextBox = new TextBox(20, 130, 200, 25);
            passwordTextBox.Text = "";
            passwordTextBox.MaxLength = 20;

            statusLabel = new Label("Lutfen bilgilerinizi giriniz...    ", 20, 165);

            loginButton = new Button("Giris Yap", 20, 195, 200, 32);
            loginButton.ClickAction = OnLoginButtonClick;

            if (this.Window != null)
            {
                this.Window.AddControl(welcomeLabel);
                this.Window.AddControl(userTitle);
                this.Window.AddControl(usernameTextBox);
                this.Window.AddControl(passTitle);
                this.Window.AddControl(passwordTextBox);
                this.Window.AddControl(statusLabel);
                this.Window.AddControl(loginButton);
            }
        }

        private void OnLoginButtonClick()
        {
            if (usernameTextBox == null || passwordTextBox == null || statusLabel == null) return;

            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrEmpty(username))
            {
                statusLabel.Text = "Hata: Kullanici adi bos birakilamaz!";
                return;
            }

            try
            {
                statusLabel.Text = "Dogrulaniyor...";

                if (UserManager.Login(username, password))
                {
                    statusLabel.Text = "Giris Basarili!";
                    isSuccess = true;

                    SendNotification("Sistem", "Giris basarili! Hos geldiniz.", NotificationType.Success, 5);

                    if (this.Window != null)
                    {
                        WindowManager.Remove(this.Window);
                    }

                    AppManager.Close(this);

                    Cosmos.Core.Memory.Heap.Collect();
                }
                else
                {
                    statusLabel.Text = "Hata: Hatali sifre girdiniz!";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Giris Hatasi: " + ex.GetType().Name;
            }
        }

        public override void Close()
        {
        }
    }
}