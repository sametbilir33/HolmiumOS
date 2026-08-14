using System;
using Cosmos.HAL.Drivers.Audio;

namespace HolmiumOS.Sound
{
    public static class AudioManager
    {
        public static AC97 Device { get; private set; }
        public static WavProvider Provider { get; private set; }

        public static bool Playing { get; private set; }

        public static void Play(WavFile wav)
        {
            if (wav == null)
                throw new ArgumentNullException("wav");

            if (wav.Data == null || wav.Data.Length == 0)
                throw new Exception("WAV dosyasinda ses verisi yok.");

            if (wav.Channels != 2)
                throw new Exception("AC97 yalnizca stereo ses destekliyor.");

            if (wav.BitsPerSample != 16)
                throw new Exception("AC97 yalnizca 16-bit ses destekliyor.");

            if (wav.AudioFormat != 1)
                throw new Exception("Yalnizca PCM WAV destekleniyor.");

            if (Device != null && Playing)
            {
                Device.Disable();
            }

            Provider = new WavProvider(wav.Data);

            if (Device == null)
            {
                Device = AC97.Initialize(512);
            }

            Device.BufferProvider = Provider;

            Device.Enable();

            Playing = true;
        }

        public static void Pause()
        {
            if (Device == null)
                return;

            Device.Disable();
            Playing = false;
        }

        public static void Resume()
        {
            if (Device == null || Provider == null)
                return;

            if (Provider.Finished)
                return;

            Device.Enable();
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
            if (!Playing)
                return;

            if (Provider == null)
            {
                Playing = false;
                return;
            }

            if (Provider.Finished)
            {
                Stop();
            }
        }
    }
}