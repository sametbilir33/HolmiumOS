namespace HolmiumOS.GUI
{
    public enum NotificationType
    {
        Success,
        Warning,
        Error,
        Info
    }

    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public int DurationSeconds { get; set; }
        public byte StartSecond { get; set; }
        public int ElapsedSeconds { get; private set; }
        private byte lastSecond;

        public Notification(string title, string message, NotificationType type = NotificationType.Info, int durationSeconds = 5)
        {
            Title = title ?? "Bildirim";
            Message = message ?? "";
            Type = type;
            DurationSeconds = durationSeconds;
            StartSecond = Cosmos.HAL.RTC.Second;
            lastSecond = StartSecond;
            ElapsedSeconds = 0;
        }

        public void UpdateTimer()
        {
            byte currentSecond = Cosmos.HAL.RTC.Second;
            if (currentSecond != lastSecond)
            {
                ElapsedSeconds++;
                lastSecond = currentSecond;
            }
        }

        public bool IsExpired => ElapsedSeconds >= DurationSeconds;
    }
}