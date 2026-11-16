
using System;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class TaskManager : AppBase
    {
        private ListBox? applicationList;
        private Label? applicationCountLabel;
        private Label? windowCountLabel;
        private Label? selectedLabel;
        private Button? closeButton;

        public TaskManager() : base("Gorev Yoneticisi")
        {
        }

        public override void Load()
        {
            if (Window == null)
                return;

            Window.Title = "Gorev Yoneticisi";
            BuildInterface();
            RefreshTaskList();
        }

        private void BuildInterface()
        {
            if (Window == null)
                return;

            Label titleLabel = new Label("Gorev Yoneticisi", 20, 15);
            Label subtitleLabel = new Label("Calisan uygulamalari goruntuleyin ve yonetin.", 20, 48);

            applicationCountLabel = new Label("Uygulamalar: 0", 20, 95);
            windowCountLabel = new Label("Pencereler: 0", 200, 95);

            applicationList = new ListBox(20, 135, 440, 285);
            applicationList.OnSelectedIndexChanged = OnApplicationSelected;

            selectedLabel = new Label("Secili uygulama: Yok", 20, 435);

            closeButton = new Button("Gorevi Sonlandir", 20, 480, 200, 48);
            closeButton.ClickAction = CloseSelectedApplication;

            Button refreshButton = new Button("Yenile", 235, 480, 110, 48);
            refreshButton.ClickAction = RefreshTaskList;

            Window.AddControl(titleLabel);
            Window.AddControl(subtitleLabel);
            Window.AddControl(applicationCountLabel);
            Window.AddControl(windowCountLabel);
            Window.AddControl(applicationList);
            Window.AddControl(selectedLabel);
            Window.AddControl(closeButton);
            Window.AddControl(refreshButton);

            Window.UpdateSize();
        }

        private void RefreshTaskList()
        {
            if (applicationList == null || Window == null)
                return;

            string? selectedName = applicationList.GetSelectedItem();

            applicationList.Clear();

            int applicationCount = 0;

            for (int i = 0; i < AppManager.apps.Count; i++)
            {
                AppBase? app = AppManager.apps[i];

                if (app == null || app.Window == null)
                    continue;

                string state = app.Window.Active ? "Calisiyor" : "Arka planda";
                applicationList.AddItem(app.Name + "    |    " + state);
                applicationCount++;
            }

            int windowCount = WindowManager.GetWindows().Count;

            if (applicationCountLabel != null)
                applicationCountLabel.Text = "Uygulamalar: " + applicationCount;

            if (windowCountLabel != null)
                windowCountLabel.Text = "Pencereler: " + windowCount;

            if (selectedLabel != null)
                selectedLabel.Text = "Secili uygulama: Yok";

            if (!string.IsNullOrEmpty(selectedName))
            {
                for (int i = 0; i < applicationList.Items.Count; i++)
                {
                    if (applicationList.Items[i] == selectedName)
                    {
                        applicationList.SelectIndex(i);
                        break;
                    }
                }
            }

            Window.UpdateSize();
        }

        private void OnApplicationSelected(int index, string name)
        {
            if (selectedLabel != null)
                selectedLabel.Text = "Secili uygulama: " + name;
        }

        private void CloseSelectedApplication()
        {
            if (applicationList == null)
                return;

            int index = applicationList.SelectedIndex;

            if (index < 0 || index >= applicationList.Items.Count)
            {
                SendNotification("Gorev Yoneticisi", "Once bir uygulama secin.", NotificationType.Warning);
                return;
            }

            AppBase? targetApp = GetApplicationByListIndex(index);

            if (targetApp == null)
                return;

            if (targetApp == this)
            {
                AppManager.Close(this);
                return;
            }

            string appName = targetApp.Name;

            AppManager.Close(targetApp);

            SendNotification("Gorev Yoneticisi", appName + " kapatildi.", NotificationType.Info);
            RefreshTaskList();
        }

        private AppBase? GetApplicationByListIndex(int listIndex)
        {
            int currentIndex = 0;

            for (int i = 0; i < AppManager.apps.Count; i++)
            {
                AppBase? app = AppManager.apps[i];

                if (app == null || app.Window == null)
                    continue;

                if (currentIndex == listIndex)
                    return app;

                currentIndex++;
            }

            return null;
        }

        public override void Close()
        {
        }
    }
}