
using System;
using Cosmos.Kernel.System;
using HolmiumOS.GUI.Controls;
using HolmiumOS.Shell;

namespace HolmiumOS.GUI.Apps
{
    public class Login : AppBase
    {
        private TextBox? usernameTextBox;
        private TextBox? passwordTextBox;
        private Button? loginButton;
        private Button? rebootButton;
        private Button? shutdownButton;
        private Label? statusLabel;

        public Login() : base("Giriş Yap")
        {
        }

        public override void Load()
        {
            if (Window == null)
                return;

            Window.Title = "Giriş Yap";

            Label welcomeLabel = new Label("HolmiumOS", 20, 20);
            Label subtitleLabel = new Label("Hesabınıza giriş yapın", 20, 55);

            Label userTitle = new Label("Kullanıcı adı", 20, 105);
            usernameTextBox = new TextBox(20, 135, 300, 42);
            usernameTextBox.Text = "";
            usernameTextBox.MaxLength = 20;

            Label passTitle = new Label("Şifre", 20, 195);
            passwordTextBox = new TextBox(20, 225, 300, 42);
            passwordTextBox.Text = "";
            passwordTextBox.MaxLength = 20;

            statusLabel = new Label("Giriş bilgilerinizi bekliyorum.", 20, 285);

            loginButton = new Button("Giriş Yap", 20, 330, 300, 50);
            loginButton.ClickAction = OnLoginButtonClick;

            rebootButton = new Button("Yeniden Başlat", 20, 395, 180, 45);
            rebootButton.ClickAction = OnRebootClick;

            shutdownButton = new Button("Kapat", 210, 395, 110, 45);
            shutdownButton.ClickAction = OnShutdownClick;

            Window.AddControl(welcomeLabel);
            Window.AddControl(subtitleLabel);
            Window.AddControl(userTitle);
            Window.AddControl(usernameTextBox);
            Window.AddControl(passTitle);
            Window.AddControl(passwordTextBox);
            Window.AddControl(statusLabel);
            Window.AddControl(loginButton);
            Window.AddControl(rebootButton);
            Window.AddControl(shutdownButton);
        }

        private void OnLoginButtonClick()
        {
            if (usernameTextBox == null || passwordTextBox == null || statusLabel == null)
                return;

            string username = usernameTextBox.Text;
            string password = passwordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                statusLabel.Text = "Kullanıcı adı boş bırakılamaz.";
                return;
            }

            try
            {
                statusLabel.Text = "Doğrulanıyor...";

                if (UserManager.Login(username, password))
                {
                    statusLabel.Text = "Giriş başarılı!";

                    SendNotification("Sistem", "Giriş başarılı! Hoş geldiniz.", NotificationType.Success, 5);

                    if (Window != null)
                        WindowManager.Remove(Window);

                    AppManager.Close(this);
                }
                else
                {
                    statusLabel.Text = "Kullanıcı adı veya şifre hatalı.";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = "Giriş hatası: " + ex.GetType().Name;
            }
        }

        private void OnRebootClick()
        {
            Power.Reboot();
        }

        private void OnShutdownClick()
        {
            Power.Shutdown();
        }

        public override void Close()
        {
        }
    }
}