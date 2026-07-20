using System;
using HolmiumOS.Sound;

namespace HolmiumOS.Commands.Sound
{
    public class StopMusic : ICommand
    {
        public string Name => "stopmusic";
        public string Description => "Muzigi durdurur.";
        public string Usage => "stopmusic";

        public void Execute(string args)
        {
            AudioManager.Stop();

            Console.WriteLine("Muzik durduruldu.");
        }
    }
}