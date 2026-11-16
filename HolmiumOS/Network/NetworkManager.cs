using System;
using CosmosNetworkManager = Cosmos.Kernel.System.Network.NetworkManager;
using Cosmos.Kernel.System.Network;

namespace HolmiumOS.Network;

public static class NetworkManager
{
    public static DnsClient DNSClient { get; private set; } = null!;

    public static int Ping(Address address, int timeout = 5000)
    {
        IcmpClient? icmp = null;

        try
        {
            icmp = new IcmpClient();
            icmp.Connect(address);
            icmp.SendEcho();

            Cosmos.Kernel.System.Network.EndPoint source = new Cosmos.Kernel.System.Network.EndPoint(address, 0);

            return icmp.Receive(ref source, timeout);
        }
        catch
        {
            return -1;
        }
        finally
        {
            try
            {
                icmp?.Close();
            }
            catch
            {
            }
        }
    }

    public static void Init()
    {
        try
        {
            try
            {
                DNSClient?.Close();
            }
            catch
            {
            }

            DNSClient = null!;

            if (CosmosNetworkManager.DeviceCount == 0 || !CosmosNetworkManager.Ready)
            {
                throw new InvalidOperationException("No ready network adapter found.");
            }

            NetworkStack.RemoveAllConfigIP();

            DhcpClient dhcpClient = new();

            int result = dhcpClient.SendDiscoverPacket();

            if (result == -1)
            {
                throw new InvalidOperationException("DHCP request timed out.");
            }

            IPConfig? config = CosmosNetworkManager.Primary.IPConfig;

            if (config is null)
            {
                throw new InvalidOperationException("DHCP did not configure the primary adapter.");
            }

            Address4 dnsServer = new(1, 1, 1, 1);

            DnsConfig.Add(dnsServer);

            DNSClient = new DnsClient();
            DNSClient.Connect(dnsServer);

        }
        catch (Exception ex)
        {
            Console.WriteLine("[Network] Initialization failed.");
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
            Address? address = Address.Parse(host);

            if (address is not null)
            {
                return address;
            }
        }
        catch
        {
            // Hostname, continue with DNS.
        }

        try
        {
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
        }

        DNSClient = null!;

        try
        {
            NetworkStack.RemoveAllConfigIP();
        }
        catch
        {
        }

        Console.WriteLine("[Network] Shutdown completed.");
    }
}