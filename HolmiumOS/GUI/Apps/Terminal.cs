using System.Linq;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class Terminal : AppBase
    {
        private TextBox inputTextBox;
        private CheckBox termCheckBox;
        private Button submitButton;
        private Label statusLabel;

        private RadioButton radioLightMode;
        private RadioButton radioDarkMode;

        private ProgressBar systemProgressBar;
        private ListBox itemsListBox;
        private ComboBox optionsComboBox;

        public Terminal() : base("HolmiumOS Master Terminal")
        {
        }

        public override void Load()
        {
            if (this.Window != null)
            {
                this.Window.Title = "HolmiumOS Master Terminal";
            }

            Label inputTitle = new Label("Kullanici Adi:", 20, 20);
            inputTextBox = new TextBox(20, 40, 200, 25);
            inputTextBox.MaxLength = 20;

            termCheckBox = new CheckBox("Beni Hatirla", 20, 75);

            radioLightMode = new RadioButton(this.Window, "Acik Tema", "ThemeGroup", 20, 115);
            radioDarkMode = new RadioButton(this.Window, "Koyu Tema", "ThemeGroup", 20, 140);
            radioLightMode.Checked = true;

            Label comboTitle = new Label("Kabuk Secimi:", 240, 20);
            optionsComboBox = new ComboBox(240, 40, 150);
            optionsComboBox.Items.Add("Bash Shell");
            optionsComboBox.Items.Add("Zsh Shell");
            optionsComboBox.Items.Add("Holmium Core");

            Label listTitle = new Label("Aktif Surecler:", 240, 75);
            itemsListBox = new ListBox(240, 95, 150, 100);
            itemsListBox.AddItem("Kernel.bin");
            itemsListBox.AddItem("GUI_Server");
            itemsListBox.AddItem("FileSystem");
            itemsListBox.AddItem("Network.sys");

            Label progressTitle = new Label("Bellek Durumu:", 20, 215);
            systemProgressBar = new ProgressBar(20, 235, 370, 20);
            systemProgressBar.Value = 45;

            statusLabel = new Label("Sistem hazir. Tiklama bekleniyor...                 ", 20, 270);

            submitButton = new Button("Verileri Isle ve Derle", 20, 305, 180, 35);
            submitButton.ClickAction = OnSubmitButtonClick;

            if (this.Window != null)
            {
                this.Window.AddControl(inputTitle);
                this.Window.AddControl(inputTextBox);
                this.Window.AddControl(termCheckBox);

                this.Window.AddControl(radioLightMode);
                this.Window.AddControl(radioDarkMode);

                this.Window.AddControl(comboTitle);
                this.Window.AddControl(optionsComboBox);
                this.Window.AddControl(listTitle);
                this.Window.AddControl(itemsListBox);

                this.Window.AddControl(progressTitle);
                this.Window.AddControl(systemProgressBar);
                this.Window.AddControl(statusLabel);
                this.Window.AddControl(submitButton);
            }
        }

        private void OnSubmitButtonClick()
        {
            if (statusLabel == null || inputTextBox == null || termCheckBox == null ||
                radioLightMode == null || optionsComboBox == null || systemProgressBar == null)
                return;

            string username = string.IsNullOrEmpty(inputTextBox.Text) ? "Anonim" : inputTextBox.Text;
            string rememberMe = termCheckBox.Checked ? "Evet" : "Hayir";

            string shell = optionsComboBox.SelectedIndex >= 0 ? optionsComboBox.Items[optionsComboBox.SelectedIndex] : "Bilinmiyor";
            string theme = radioLightMode.Checked ? "Acik" : "Koyu";

            systemProgressBar.Value = 85;
            systemProgressBar.BarColor = System.Drawing.Color.Orange;

            statusLabel.Text = $"U: {username} | T: {theme} | S: {shell} | H: {rememberMe}";

            Cosmos.Core.Memory.Heap.Collect();
        }
    }
}