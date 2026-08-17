using HolmiumOS.GUI.Controls;
using Cosmos.HAL;

namespace HolmiumOS.GUI.Apps
{
    public class CalendarClock : AppBase
    {
        private Label titleLabel;
        private Label rtcDateLabel;
        private Label rtcTimeLabel;
        private Button refreshButton;

        public CalendarClock() : base("Sistem Saat ve Takvimi")
        {
        }

        public override void Load()
        {
            if (this.Window != null)
            {
                this.Window.Title = "Sistem Saat ve Takvimi";
            }

            titleLabel = new Label("Saat / Tarih", 20, 20);

            string currentRTCDate = $"{RTC.DayOfTheMonth:D2}/{RTC.Month:D2}/{RTC.Year:D4}";
            string currentRTCTime = $"{RTC.Hour:D2}:{RTC.Minute:D2}:{RTC.Second:D2}";

            rtcDateLabel = new Label($"Tarih: {currentRTCDate}", 20, 60);
            rtcTimeLabel = new Label($"Saat: {currentRTCTime}", 20, 95);

            refreshButton = new Button("Saati Guncelle", 20, 140, 160, 30);
            refreshButton.ClickAction = OnRefreshClick;

            if (this.Window != null)
            {
                this.Window.AddControl(titleLabel);
                this.Window.AddControl(rtcDateLabel);
                this.Window.AddControl(rtcTimeLabel);
                this.Window.AddControl(refreshButton);
            }
        }

        private void OnRefreshClick()
        {
            if (rtcDateLabel == null || rtcTimeLabel == null) return;

            string updatedDate = $"{RTC.DayOfTheMonth:D2}/{RTC.Month:D2}/{RTC.Year:D4}";
            string updatedTime = $"{RTC.Hour:D2}:{RTC.Minute:D2}:{RTC.Second:D2}";

            rtcDateLabel.Text = $"Tarih: {updatedDate}";
            rtcTimeLabel.Text = $"Saat: {updatedTime}";
        }
    }
}