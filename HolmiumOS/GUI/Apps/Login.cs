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
        private bool isSuccess = false;

        public Login() : base("HolmiumOS - Sistem Girisi")
        {
            this.isSuccess = false;
        }

        public override void Load()
        {
            if (this.Window != null)
            {
                this.Window.Title = "Sistem Girisi";
            }

            Label welcomeLabel = new Label("HolmiumOS GUI Ortamina Hos Geldiniz", 20, 20);

            Label userTitle = new Label("Kullanici Adi:", 20, 60);
            usernameTextBox = new TextBox(20, 80, 260, 25);
            usernameTextBox.Text = "";
            usernameTextBox.MaxLength = 20;

            Label passTitle = new Label("Sifre:", 20, 115);
            passwordTextBox = new TextBox(20, 135, 260, 25);
            passwordTextBox.Text = "";
            passwordTextBox.MaxLength = 20;

            statusLabel = new Label("Lutfen bilgilerinizi giriniz...                  ", 20, 180);

            loginButton = new Button("Giris Yap", 20, 215, 260, 35);
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
                statusLabel.Text = "Dogrulanıyor...";

                if (UserManager.Login(username, password))
                {
                    statusLabel.Text = "Giris Basarili!";
                    isSuccess = true;

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