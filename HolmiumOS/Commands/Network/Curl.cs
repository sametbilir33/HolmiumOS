using System;
using System.Text;
using HolmiumOS.Network.HTTP;

namespace HolmiumOS.Commands.Network
{
    public class Curl : ICommand
    {
        public string Name => "curl";
        public string Description => "HTTP icerigini gosterir.";
        public string Usage => "curl <url>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine(Usage);
                return;
            }

            try
            {
                HTTPClient client = new(args);

                byte[] response = client.Get();

                Console.WriteLine(
                    Encoding.UTF8.GetString(response)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"curl error: {ex.Message}");
            }
        }
    }
}