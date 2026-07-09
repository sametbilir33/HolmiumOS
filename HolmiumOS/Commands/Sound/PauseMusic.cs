using System;
using HolmiumOS.Commands;
using HolmiumOS.Sound;

namespace HolmiumOS.Commands.Sound
{
    public class PauseMusic : ICommand
    {
        public string Name => "pausemusic";
        public string Description => "Muzigi duraklatir.";
        public string Usage => "pausemusic";

        public void Execute(string args)
        {
            AudioManager.Pause();

            Console.WriteLine("Muzik duraklatildi.");
        }
    }
}