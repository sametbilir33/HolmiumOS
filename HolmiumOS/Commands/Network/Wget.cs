using System;
using HolmiumOS.Network.HTTP;
using HolmiumOS.Shell;

namespace HolmiumOS.Commands.Network
{
    public class Wget : ICommand
    {
        public string Name => "wget";

        public string Description =>
            "HTTP sunucusundan dosya indirir.";

        public string Usage =>
            "wget <url> [dosya]";

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

                string fileName =
                    parts.Length > 1
                        ? parts[1]
                        : GetFileName(url);

                HTTPClient client = new(url);

                HTTPResponse response = client.Get();

                if (response.Body == null ||
                    response.Body.Length == 0)
                {
                    Console.WriteLine(
                        "Sunucu bos cevap dondu.");
                    return;
                }

                FileSystemManager.WriteBytes(
                    fileName,
                    response.Body);

                string resolved =
                    FileSystemManager.ResolvePath(fileName);

                Console.WriteLine(
                    $"İndirildi: {resolved}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"wget error: {ex.Message}");
            }
        }

        private string GetFileName(string url)
        {
            // Query string'i ayır.
            int queryIndex = url.IndexOf('?');

            if (queryIndex >= 0)
                url = url.Substring(0, queryIndex);

            // Fragment'i ayır.
            int fragmentIndex = url.IndexOf('#');

            if (fragmentIndex >= 0)
                url = url.Substring(0, fragmentIndex);

            int index = url.LastIndexOf('/');

            if (index == -1 ||
                index == url.Length - 1)
            {
                return "index.html";
            }

            string fileName =
                url[(index + 1)..];

            if (string.IsNullOrWhiteSpace(fileName))
                return "index.html";

            return fileName;
        }
    }
}