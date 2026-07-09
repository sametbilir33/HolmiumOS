using Cosmos.HAL.Drivers.Audio;

namespace HolmiumOS.Sound
{
    public static class AudioManager
    {
        public static AC97 Device;
        public static WavProvider Provider;
        public static bool Playing;

        public static void Play(byte[] data)
        {
            Provider = new WavProvider(data);

            Device = AC97.Initialize(512);
            Device.BufferProvider = Provider;

            Device.Enable();

            Playing = true;
        }
        public static void Pause()
        {
            if (Device != null)
            {
                Device.Disable();
            }

            Playing = false;
        }

        public static void Resume()
        {
            if (Device != null)
            {
                Device.Enable();
            }

            Playing = true;
        }
        public static void Stop()
        {
            if (Device != null)
            {
                Device.Disable();
            }

            Playing = false;
            Provider = null;
        }

        public static void Update()
        {
            if (Playing && Provider != null && Provider.Finished)
            {
                Stop();
            }
        }
    }
}