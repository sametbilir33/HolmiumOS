using System;
using System.Net.Sockets;
using System.Text;
using Cosmos.System.Network.IPv4;

namespace HolmiumOS.Network.HTTP;

public class HTTPClient
{
    public HTTPClient(string URL)
    {
        this.URL = new(URL);
    }

    public HTTPClient(URL URL)
    {
        this.URL = URL;
    }

    public HTTPClient()
    {
        URL = new("");
    }

    public byte[] Get()
    {
        int Port = URL.HasPort ? int.Parse(URL.Port) : 80;

        TcpClient Client = new();
        Client.Connect(URL.Address.ToString(), Port);

        NetworkStream Stream = Client.GetStream();

        string Request =
            $"GET {URL.Path} HTTP/1.1\r\n" +
            $"Host: {URL.Address}\r\n" +
            "Connection: close\r\n\r\n";

        byte[] RequestBytes = Encoding.UTF8.GetBytes(Request);

        Stream.Write(RequestBytes, 0, RequestBytes.Length);

        byte[] Buffer = new byte[Client.ReceiveBufferSize];

        int Length = Stream.Read(Buffer, 0, Buffer.Length);

        byte[] Response = new byte[Length];
        Array.Copy(Buffer, Response, Length);

        Stream.Close();
        Client.Close();

        return Response;
    }

    public URL URL;
}