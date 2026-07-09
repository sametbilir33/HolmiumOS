using System;
using HolmiumOS.Network;

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
                Console.WriteLine("Kullanim: " + Usage);
                return;
            }

            if (!ParseUrl(args.Trim(), out string host, out string path))
            {
                Console.WriteLine("Gecersiz URL.");
                return;
            }

            try
            {
                string body = HttpHelper.SimpleHttpGet(host, path);
                Console.Write(body);
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
    }
}