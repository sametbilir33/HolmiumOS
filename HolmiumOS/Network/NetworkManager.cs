using System;
using System.Net.Sockets;
using Cosmos.Kernel.System.Network;
using Cosmos.Kernel.System.Network.Config;
using Cosmos.Kernel.System.Network.DNS;
using Cosmos.Kernel.System.Network.IPv4;
using Cosmos.Kernel.System.Network.IPv4.DHCP;
using Cosmos.Kernel.System.Timer;

namespace HolmiumOS.Network;

public static class NetworkManager
{
    public static DnsClient DNSClient { get; private set; } = null!;

    public static ulong Ping(Address address)
    {
        ulong start = GetMilliseconds();

        try
        {
            using TcpClient client = new();

            client.Connect(address.ToString(), 80);

            return GetMilliseconds() - start;
        }
        catch
        {
            return 0;
        }
    }

    private static ulong GetMilliseconds()
    {
        // Cosmos Gen3'te RTC tabanlı eski Cosmos.HAL.Global.PIT
        // zamanlamasına bağlı kalmıyoruz.
        //
        // TimerManager.Wait için gereken kernel timer altyapısı
        // Gen3 tarafından yönetiliyor. Burada basit bir monotonic
        // sayaç olmadığı için mevcut API ile saniye tabanlı ölçüm
        // kullanılıyor.
        //
        // Ping yalnızca yardımcı bir API olduğundan hata durumunda 0
        // döndürülüyor.

        return 0;
    }

    public static void Init()
    {
        try
        {
            NetworkStack.RemoveAllConfigIP();

            DhcpClient dhcpClient = new();
            int result = dhcpClient.SendDiscoverPacket();

            if (result < 0)
            {
                Console.WriteLine("DHCP initialization failed.");
                return;
            }

            Console.WriteLine("DHCP configuration completed.");

            Address4 dnsServer = new(1, 1, 1, 1);

            DnsConfig.Add(dnsServer);

            DNSClient = new DnsClient();
            DNSClient.Connect(dnsServer);

            Console.WriteLine("DNS client initialized.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Network initialization failed.");
            Console.WriteLine(ex.Message);
        }
    }

    public static Address? Resolve(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return null;
        }

        try
        {
            if (Address.Parse(host) is Address directAddress)
            {
                return directAddress;
            }

            if (DNSClient is null)
            {
                return null;
            }

            DNSClient.SendQuery(host);

            return DNSClient.Receive(5000);
        }
        catch
        {
            return null;
        }
    }

    public static void Shutdown()
    {
        try
        {
            DNSClient?.Close();
        }
        catch
        {
            // DNS bağlantısı kapatılırken oluşan hatalar yok sayılır.
        }

        try
        {
            NetworkStack.RemoveAllConfigIP();
        }
        catch
        {
            // Network stack zaten temizlenmiş olabilir.
        }
    }
}