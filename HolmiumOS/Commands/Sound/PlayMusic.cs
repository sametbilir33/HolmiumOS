using System;
using HolmiumOS.Sound;

namespace HolmiumOS.Commands.Sound
{
    public class PlayMusic : ICommand
    {
        public string Name => "playmusic";
        public string Description => "WAV dosyasi calar.";
        public string Usage => "playmusic <dosya.wav>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine(Usage);
                return;
            }

            try
            {
                WavFile wav = WavFile.Load(args.Trim());

                AudioManager.Play(wav);

                Console.WriteLine("Caliniyor: " + args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ses hatasi: " + ex.Message);
            }
        }
    }
}