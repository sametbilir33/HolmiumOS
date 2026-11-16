using System;
using System.Collections.Generic;
using System.Text;
using Cosmos.Network.Http;

namespace HolmiumOS.Network.HTTP
{
    public class HTTPClient
    {
        private readonly URL _url;

        public HTTPClient(string url)
        {
            _url = new URL(url);
        }

        public HTTPResponse Get()
        {
            if (_url.Scheme != "http" && _url.Scheme != "https")
            {
                throw new NotSupportedException("Only HTTP and HTTPS protocols are supported.");
            }

            if (string.IsNullOrWhiteSpace(_url.Host))
            {
                throw new ArgumentException("Invalid URL: Host is missing.");
            }

            if (!int.TryParse(_url.Port, out int port) || port < 1 || port > 65535)
            {
                throw new ArgumentException("Invalid URL port: " + _url.Port);
            }

            Console.WriteLine("[HTTP] Requesting " + _url.ToString());

            using HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            using HttpResponseMessage response = client.Get(_url.ToString());

            string content = response.Content.ReadAsString();

            Dictionary<string, string> headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            string? contentType = response.Content.Headers.ContentType?.ToString();

            if (!string.IsNullOrEmpty(contentType))
            {
                headers["Content-Type"] = contentType;
            }

            byte[] body = Encoding.UTF8.GetBytes(content);

            Console.WriteLine("[HTTP] Status: " + (int)response.StatusCode + " " + response.ReasonPhrase);

            Console.WriteLine("[HTTP] Received " + body.Length + " bytes.");

            return new HTTPResponse((int)response.StatusCode, response.ReasonPhrase ?? string.Empty, headers, body);
        }
    }
}