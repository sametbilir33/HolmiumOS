using System;
using HolmiumOS.Commands;
using HolmiumOS.Sound;

namespace HolmiumOS.Commands.Sound
{
    public class MusicStatus : ICommand
    {
        public string Name => "musicstatus";
        public string Description => "Ses durumunu gosterir.";
        public string Usage => "musicstatus";

        public void Execute(string args)
        {
            if (AudioManager.Playing)
            {
                Console.WriteLine("Muzik caliyor.");
            }
            else
            {
                Console.WriteLine("Muzik durdu.");
            }
        }
    }
}