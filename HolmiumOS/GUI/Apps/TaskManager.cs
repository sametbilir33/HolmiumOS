using System;
using System.Drawing;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class TaskManager : AppBase
    {
        private ListBox applicationList;
        private Label selectedLabel;
        private Label applicationCountLabel;
        private Label windowCountLabel;
        private ProgressBar applicationBar;
        private Button closeButton;

        public TaskManager()
            : base("Gorev Yoneticisi")
        {
        }

        public override void Load()
        {
            BuildInterface();
            RefreshTaskList();
        }

        private void BuildInterface()
        {
            Label titleLabel =
                new Label(
                    "Calisan Uygulamalar",
                    15,
                    15);

            Window.AddControl(titleLabel);

            applicationList =
                new ListBox(
                    15,
                    55,
                    400,
                    290);

            applicationList.OnSelectedIndexChanged =
                OnApplicationSelected;

            Window.AddControl(
                applicationList);

            selectedLabel =
                new Label(
                    "Secili: Yok",
                    15,
                    360);

            Window.AddControl(
                selectedLabel);

            applicationCountLabel =
                new Label(
                    "Uygulamalar: 0",
                    15,
                    400);

            Window.AddControl(
                applicationCountLabel);

            windowCountLabel =
                new Label(
                    "Pencereler: 0",
                    15,
                    440);

            Window.AddControl(
                windowCountLabel);

            Label usageLabel =
                new Label(
                    "Uygulama Yukü",
                    15,
                    480);

            Window.AddControl(
                usageLabel);

            applicationBar =
                new ProgressBar(
                    15,
                    520,
                    400,
                    20);

            applicationBar.BarColor =
                Color.DodgerBlue;

            applicationBar.BackgroundColor =
                Color.LightGray;

            Window.AddControl(
                applicationBar);

            closeButton =
                new Button(
                    "Gorevi Sonlandir",
                    15,
                    560,
                    220,
                    55);

            closeButton.ClickAction =
                CloseSelectedApplication;

            Window.AddControl(
                closeButton);

            Button refreshButton =
                new Button(
                    "Yenile",
                    245,
                    560,
                    100,
                    55);

            refreshButton.ClickAction =
                RefreshTaskList;

            Window.AddControl(
                refreshButton);

            Window.UpdateSize();
        }

        private void RefreshTaskList()
        {
            if (applicationList == null)
                return;

            string selectedName =
                applicationList.GetSelectedItem();

            applicationList.Clear();

            int applicationCount = 0;
            int activeCount = 0;

            for (
                int i = 0;
                i < AppManager.apps.Count;
                i++)
            {
                AppBase app =
                    AppManager.apps[i];

                if (app == null ||
                    app.Window == null)
                {
                    continue;
                }

                string state =
                    app.Window.Active
                        ? "AKTIF"
                        : "ACIK";

                applicationList.AddItem(
                    app.Name +
                    " - " +
                    state);

                applicationCount++;

                if (app.Window.Active)
                {
                    activeCount++;
                }
            }

            applicationCountLabel.Text =
                "Uygulamalar: " +
                applicationCount;

            windowCountLabel.Text =
                "Pencereler: " +
                WindowManager
                    .GetWindows()
                    .Count;

            if (applicationCount > 0)
            {
                int usage =
                    (activeCount * 100) /
                    applicationCount;

                if (usage > 100)
                    usage = 100;

                applicationBar.Value =
                    usage;
            }
            else
            {
                applicationBar.Value = 0;
            }

            selectedLabel.Text =
                "Secili: Yok";

            if (!string.IsNullOrEmpty(
                    selectedName))
            {
                for (
                    int i = 0;
                    i < applicationList.Items.Count;
                    i++)
                {
                    if (applicationList.Items[i] ==
                        selectedName)
                    {
                        applicationList.SelectIndex(
                            i);

                        break;
                    }
                }
            }

            Window.UpdateSize();
        }

        private void OnApplicationSelected(
            int index,
            string name)
        {
            selectedLabel.Text =
                "Secili: " +
                name;
        }

        private void CloseSelectedApplication()
        {
            if (applicationList == null)
                return;

            int index =
                applicationList.SelectedIndex;

            if (index < 0 ||
                index >=
                applicationList.Items.Count)
            {
                SendNotification(
                    "Gorev Yoneticisi",
                    "Once bir uygulama secin.",
                    NotificationType.Warning
                );

                return;
            }

            AppBase targetApp =
                GetApplicationByListIndex(
                    index);

            if (targetApp == null)
                return;

            if (targetApp == this)
            {
                AppManager.Close(this);
                return;
            }

            string appName =
                targetApp.Name;

            AppManager.Close(
                targetApp);

            SendNotification(
                "Gorev Yoneticisi",
                appName + " kapatildi.",
                NotificationType.Info
            );

            RefreshTaskList();
        }

        private AppBase GetApplicationByListIndex(
            int listIndex)
        {
            int currentIndex = 0;

            for (
                int i = 0;
                i < AppManager.apps.Count;
                i++)
            {
                AppBase app =
                    AppManager.apps[i];

                if (app == null ||
                    app.Window == null)
                {
                    continue;
                }

                if (currentIndex ==
                    listIndex)
                {
                    return app;
                }

                currentIndex++;
            }

            return null;
        }

        public override void Close()
        {
        }
    }
}