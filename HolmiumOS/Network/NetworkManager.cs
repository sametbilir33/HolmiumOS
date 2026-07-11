using System;
using System.Net.Sockets;
using Cosmos.HAL;
using Cosmos.System.Network.IPv4;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using Cosmos.System.Network.IPv4.UDP.DNS;
namespace HolmiumOS.Network;

public static class NetworkManager
{
    public static ulong Ping(Address A)
    {
        ulong start = GetMilliseconds();

        using TcpClient client = new();
        client.Connect(A.ToString(), 80);

        return GetMilliseconds() - start;
    }

    private static ulong GetMilliseconds()
    {
        return (ulong)(
            ((RTC.Hour * 3600) +
            (RTC.Minute * 60) +
            RTC.Second) * 1000
        );
    }

    public static void Init()
    {
        try
        {
            Console.WriteLine("Initializing network...");

            _ = new DHCPClient().SendDiscoverPacket();

            DNSClient.Connect(new(1, 1, 1, 1));
        }
        catch
        {
            Console.WriteLine("Network initialization failed.");
        }
    }

    public static DnsClient DNSClient = null!;
}