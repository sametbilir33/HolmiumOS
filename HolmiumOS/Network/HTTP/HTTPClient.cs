using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

using Cosmos.Kernel.System.Timer;

using HolmiumOS.Network;

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
            if (_url.Scheme != "http")
            {
                throw new NotSupportedException(
                    "Only HTTP is supported. HTTPS is not available because Cosmos Gen3 has no TLS support.");
            }

            var address = _url.Address;

            if (address is null)
            {
                throw new Exception(
                    "Failed to resolve host: " + _url.Host);
            }

            int port = 80;

if (!string.IsNullOrWhiteSpace(_url.Port))
{
    if (!int.TryParse(_url.Port, out port))
    {
        throw new Exception(
            "Invalid port: " + _url.Port);
    }
}

            Console.WriteLine(
                "[HTTP] Connecting to " +
                address.ToString() +
                ":" +
                port);

            TcpClient client = new TcpClient();

            try
            {
                client.Connect(
                    address.ToString(),
                    port);

                NetworkStream stream =
                    client.GetStream();

                string request =
                    "GET " + _url.Path + " HTTP/1.1\r\n" +
                    "Host: " + _url.Host + "\r\n" +
                    "User-Agent: HolmiumOS\r\n" +
                    "Accept: */*\r\n" +
                    "Connection: close\r\n" +
                    "\r\n";

                byte[] requestBytes =
                    Encoding.ASCII.GetBytes(request);

                stream.Write(
                    requestBytes,
                    0,
                    requestBytes.Length);

                Console.WriteLine(
                    "[HTTP] Request sent.");

                byte[] rawResponse =
                    ReadResponse(stream);

                Console.WriteLine(
                    "[HTTP] Received " +
                    rawResponse.Length +
                    " bytes.");

                return ParseResponse(rawResponse);
            }
            finally
            {
                try
                {
                    client.Close();
                }
                catch
                {
                }
            }
        }

        private byte[] ReadResponse(NetworkStream stream)
{
    MemoryStream response =
        new MemoryStream();

    byte[] buffer =
        new byte[4096];

    int totalBytes = 0;

    while (true)
    {
        TimerManager.Wait(10);

        int bytesRead =
            stream.Read(
                buffer,
                0,
                buffer.Length);

        Console.WriteLine(
            "[HTTP] Read returned: " +
            bytesRead);

        if (bytesRead <= 0)
            break;

        response.Write(
            buffer,
            0,
            bytesRead);

        totalBytes += bytesRead;

        Console.WriteLine(
            "[HTTP] Total received: " +
            totalBytes);
    }

    return response.ToArray();
}

        private HTTPResponse ParseResponse(
            byte[] raw)
        {
            int headerEnd =
                FindHeaderEnd(raw);

            if (headerEnd < 0)
            {
                throw new Exception(
                    "Invalid HTTP response: headers not found.");
            }

            int bodyStart =
                headerEnd + 4;

            string headerText =
                Encoding.ASCII.GetString(
                    raw,
                    0,
                    headerEnd);

            string[] lines =
                headerText.Split(
                    new[] { "\r\n" },
                    StringSplitOptions.None);

            if (lines.Length == 0)
            {
                throw new Exception(
                    "Invalid HTTP response.");
            }

            string statusLine =
                lines[0];

            string[] statusParts =
                statusLine.Split(
                    new[] { ' ' },
                    3);

            if (statusParts.Length < 2)
            {
                throw new Exception(
                    "Invalid HTTP status line.");
            }

            int statusCode;

            if (!int.TryParse(
                    statusParts[1],
                    out statusCode))
            {
                throw new Exception(
                    "Invalid HTTP status code.");
            }

            string statusText = "";

            if (statusParts.Length >= 3)
            {
                statusText =
                    statusParts[2];
            }

            Dictionary<string, string> headers =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase);

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrEmpty(line))
                    continue;

                int separator =
                    line.IndexOf(':');

                if (separator <= 0)
                    continue;

                string name =
                    line.Substring(
                        0,
                        separator).Trim();

                string value =
                    line.Substring(
                        separator + 1).Trim();

                headers[name] = value;
            }

            int bodyLength =
                raw.Length - bodyStart;

            if (bodyLength < 0)
                bodyLength = 0;

            byte[] body =
                new byte[bodyLength];

            if (bodyLength > 0)
            {
                Buffer.BlockCopy(
                    raw,
                    bodyStart,
                    body,
                    0,
                    bodyLength);
            }

            return new HTTPResponse(
                statusCode,
                statusText,
                headers,
                body);
        }

        private int FindHeaderEnd(
            byte[] data)
        {
            for (
                int i = 0;
                i < data.Length - 3;
                i++)
            {
                if (
                    data[i] == 13 &&
                    data[i + 1] == 10 &&
                    data[i + 2] == 13 &&
                    data[i + 3] == 10)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}