using System;
using Cosmos.System.Network.IPv4;
using HolmiumOS.Network;

namespace HolmiumOS.Commands.Network
{
    public class Ping : ICommand
    {
        public string Name => "ping";
        public string Description => "Bir IP adresine bağlantı testi yapar.";
        public string Usage => "ping <ip>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine(Usage);
                return;
            }

            string[] ip = args.Split('.');

            if (ip.Length != 4)
            {
                Console.WriteLine("Geçersiz IP");
                return;
            }

            Address address = new(
                byte.Parse(ip[0]),
                byte.Parse(ip[1]),
                byte.Parse(ip[2]),
                byte.Parse(ip[3])
            );

            ulong ms = NetworkManager.Ping(address);

            Console.WriteLine($"Reply from {args}: {ms}ms");
        }
    }
}