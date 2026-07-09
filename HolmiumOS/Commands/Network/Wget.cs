using System;
using HolmiumOS.Network;
using HolmiumOS.Shell;

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
                Console.WriteLine("Kullanim: " + Usage);
                return;
            }

            string[] parts = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!ParseUrl(parts[0], out string host, out string path))
            {
                Console.WriteLine("Gecersiz URL.");
                return;
            }

            string output;

            if (parts.Length >= 2)
            {
                output = parts[1];
            }
            else
            {
                output = GetFileName(path);
            }

            try
            {
                Console.WriteLine("Baglaniyor...");

                string body = HttpHelper.SimpleHttpGet(host, path);

                FileSystemManager.WriteFile(output, body);

                Console.WriteLine("Kaydedildi: " + output);
            }
            catch (HttpHelper.RegionBlockedException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }
        }

        private static bool ParseUrl(string url, out string host, out string path)
        {
            host = "";
            path = "/";

            if (!url.StartsWith("http://"))
                return false;

            url = url.Substring(7);

            int slash = url.IndexOf('/');

            if (slash == -1)
            {
                host = url;
            }
            else
            {
                host = url.Substring(0, slash);
                path = url.Substring(slash);

                if (path.Length == 0)
                    path = "/";
            }

            return host.Length > 0;
        }

        private static string GetFileName(string path)
        {
            if (path == "/")
                return "index.html";

            int slash = path.LastIndexOf('/');

            if (slash == path.Length - 1)
                return "index.html";

            string file = path.Substring(slash + 1);

            int query = file.IndexOf('?');

            if (query != -1)
                file = file.Substring(0, query);

            if (string.IsNullOrEmpty(file))
                file = "index.html";

            return file;
        }
    }
}