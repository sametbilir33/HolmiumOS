using System;
using Cosmos.Kernel.System.Network;
using HolmiumNetworkManager = HolmiumOS.Network.NetworkManager;

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

            try
            {
                string input = args.Trim();

                Address? address =
                    Address.Parse(input);

                if (address is null)
                {
                    Console.WriteLine(
                        "Gecersiz IP adresi.");

                    return;
                }

                Console.WriteLine(
                    $"PING {address}");

                int elapsed =
    HolmiumNetworkManager.Ping(address);

                if (elapsed >= 0)
                {
                    Console.WriteLine(
                        $"Reply from {address}: time={elapsed}ms");
                }
                else
                {
                    Console.WriteLine(
                        "Request timed out.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"ping error: {ex.Message}");
            }
        }
    }
}