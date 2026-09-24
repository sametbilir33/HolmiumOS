using System;
using System.Net.Sockets;
using System.Text;

namespace HolmiumOS.Network.HTTP;

public class HTTPClient
{
    public HTTPClient(string url)
    {
        URL = new URL(url);
    }

    public HTTPClient(URL url)
    {
        URL = url;
    }

    public HTTPClient()
    {
        URL = new URL(string.Empty);
    }

    public byte[] Get()
    {
        if (URL.Address is null)
        {
            throw new InvalidOperationException(
                $"Host çözümlenemedi: {URL.Host}");
        }

        int port = URL.HasPort
            ? int.Parse(URL.Port)
            : 80;

        using TcpClient client = new();

        client.Connect(
            URL.Address.ToString(),
            port);

        using NetworkStream stream = client.GetStream();

        string path = URL.Path;

        if (string.IsNullOrEmpty(path))
        {
            path = "/";
        }

        string request =
            $"GET {path} HTTP/1.1\r\n" +
            $"Host: {URL.Host}\r\n" +
            "Connection: close\r\n\r\n";

        byte[] requestBytes =
            Encoding.UTF8.GetBytes(request);

        stream.Write(
            requestBytes,
            0,
            requestBytes.Length);

        byte[] buffer =
            new byte[client.ReceiveBufferSize];

        int length = stream.Read(
            buffer,
            0,
            buffer.Length);

        byte[] response =
            new byte[length];

        Array.Copy(
            buffer,
            response,
            length);

        return response;
    }

    public URL URL;
}