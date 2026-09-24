using System;

namespace HolmiumOS.Commands.Network
{
    public class Ping : ICommand
    {
        public string Name => "ping";

        public string Description =>
            "Bir IP adresine baglanti testi yapar.";

        public string Usage =>
            "ping <ip>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine(Usage);
                return;
            }

            Console.WriteLine(
                "Ping, Cosmos Gen3'te mevcut NetworkManager API'si tarafindan desteklenmiyor.");

            Console.WriteLine(
                "ICMP ping desteği daha sonra eklenebilir.");
        }
    }
}