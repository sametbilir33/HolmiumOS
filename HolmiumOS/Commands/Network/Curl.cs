using System;
using System.Text;
using HolmiumOS.Network.HTTP;

namespace HolmiumOS.Commands.Network
{
    public class Curl : ICommand
    {
        public string Name => "curl";

        public string Description =>
            "HTTP icerigini gosterir.";

        public string Usage =>
            "curl <url>";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine(Usage);
                return;
            }

            try
            {
                string[] parts =
                    args.Split(
                        ' ',
                        StringSplitOptions.RemoveEmptyEntries);

                string url = parts[0];

                HTTPClient client = new(url);

                HTTPResponse response = client.Get();

                if (response.Body == null ||
                    response.Body.Length == 0)
                {
                    Console.WriteLine(
                        "Sunucu bos cevap dondu.");
                    return;
                }

                Console.WriteLine(
                    Encoding.UTF8.GetString(response.Body));
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"curl error: {ex.Message}");
            }
        }
    }
}