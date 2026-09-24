using System;
using System.IO;
using HolmiumOS.Network.HTTP;

namespace HolmiumOS.Commands.Network
{
    public class Wget : ICommand
    {
        public string Name => "wget";
        public string Description => "HTTP sunucusundan dosya indirir.";
        public string Usage => "wget <url> [dosya]";

        public void Execute(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
            {
                Console.WriteLine(Usage);
                return;
            }

            try
            {
                string[] parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                string url = parts[0];
                string fileName = parts.Length > 1
                    ? parts[1]
                    : GetFileName(url);

                HTTPClient client = new(url);

                byte[] data = client.Get();

                File.WriteAllBytes(fileName, data);

                Console.WriteLine($"İndirildi: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"wget error: {ex.Message}");
            }
        }

        private string GetFileName(string url)
        {
            int index = url.LastIndexOf('/');

            if (index == -1 || index == url.Length - 1)
                return "index.html";

            return url[(index + 1)..];
        }
    }
}