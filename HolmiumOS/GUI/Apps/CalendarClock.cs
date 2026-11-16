
using System;
using HolmiumOS.GUI.Controls;

namespace HolmiumOS.GUI.Apps
{
    public class CalendarClock : AppBase
    {
        private Label titleLabel;
        private Label DateLabel;
        private Label TimeLabel;
        private Button refreshButton;

        public CalendarClock() : base("Sistem Saat ve Takvimi")
        {
        }

        public override void Load()
        {
            if (Window == null)
                return;

            Window.Title = "Sistem Saat ve Takvimi";

            titleLabel = new Label("Tarih ve Saat", 20, 20);
            TimeLabel = new Label("--:--:--", 20, 76);
            DateLabel = new Label("--/--/----", 20, 120);

            refreshButton = new Button("Şimdi Güncelle", 20, 175, 220, 48);
            refreshButton.ClickAction = OnRefreshClick;

            Window.AddControl(titleLabel);
            Window.AddControl(TimeLabel);
            Window.AddControl(DateLabel);
            Window.AddControl(refreshButton);

            UpdateClock();
        }

        private void OnRefreshClick()
        {
            UpdateClock();
        }

        private void UpdateClock()
        {
            if (DateLabel == null || TimeLabel == null)
                return;

            DateTime now = DateTime.Now;

            TimeLabel.Text = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}";
            DateLabel.Text = $"{now.Day:D2}/{now.Month:D2}/{now.Year:D4}";
        }

        public override void Close()
        {
        }
    }
}
