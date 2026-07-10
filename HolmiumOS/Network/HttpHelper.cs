using System;
using System.Text;
using System.Net.Sockets;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DNS;

namespace HolmiumOS.Network
{
    public static class HttpHelper
    {
        public sealed class HttpResponse
        {
            public int StatusCode { get; set; }
            public string Body { get; set; } = string.Empty;
        }

        public sealed class RegionBlockedException : Exception
        {
            public RegionBlockedException(string message) : base(message) { }
        }

        public static string SimpleHttpGet(string host, string path)
        {
            var response = SimpleHttpGetRaw(host, path);

            CheckForRegionalBlock(response);

            return response.Body;
        }

        public static HttpResponse SimpleHttpGetRaw(string host, string path)
        {
            string serverIP = ResolveDNS(host);

            if (string.IsNullOrEmpty(serverIP))
                throw new Exception("DNS resolution failed");

            using (var tcpClient = new TcpClient())
            {
                tcpClient.Connect(serverIP, 80);

                using (NetworkStream stream = tcpClient.GetStream())
                {
                    string request =
                        "GET " + path + " HTTP/1.1\r\n" +
                        "Host: " + host + "\r\n" +
                        "User-Agent: HolmiumOS\r\n" +
                        "Accept: */*\r\n" +
                        "Connection: close\r\n\r\n";

                    byte[] data = Encoding.ASCII.GetBytes(request);

                    stream.Write(data, 0, data.Length);

                    StringBuilder response = new StringBuilder();

                    byte[] buffer = new byte[8192];
                    int read;

                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        response.Append(
                            Encoding.ASCII.GetString(buffer, 0, read)
                        );
                    }

                    string raw = response.ToString();

                    int headerEnd = raw.IndexOf("\r\n\r\n");

                    if (headerEnd == -1)
                        throw new Exception("Invalid HTTP response");

                    string header = raw.Substring(0, headerEnd);
                    string body = raw.Substring(headerEnd + 4);

                    int statusCode = 0;

                    string statusLine = header.Split(
                        new[] { "\r\n" },
                        StringSplitOptions.None
                    )[0];

                    string[] parts = statusLine.Split(
                        new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    if (parts.Length > 1)
                    {
                        int.TryParse(parts[1], out statusCode);
                    }

                    return new HttpResponse
                    {
                        StatusCode = statusCode,
                        Body = body
                    };
                }
            }
        }

        public static void CheckForRegionalBlock(HttpResponse response)
        {
            if (response.StatusCode == 401)
            {
                throw new RegionBlockedException(
                    "You are connecting from a country where 2k6 Network is not available."
                );
            }
        }

        private static string ResolveDNS(string host)
        {
            var dnsClient = new DnsClient();

            dnsClient.Connect(DNSConfig.DNSNameservers[0]);

            dnsClient.SendAsk(host);

            Address address = dnsClient.Receive();

            dnsClient.Close();

            if (address == null)
            {
                throw new Exception("DNS cevap vermedi");
            }

            return address.ToString();
        }
    }
}