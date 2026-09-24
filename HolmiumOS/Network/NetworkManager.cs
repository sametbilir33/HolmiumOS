using System;
using CosmosNetworkManager = Cosmos.Kernel.System.Network.NetworkManager;
using Cosmos.Kernel.System.Network;
using Cosmos.Kernel.System.Network.Config;
using Cosmos.Kernel.System.Network.DNS;
using Cosmos.Kernel.System.Network.IPv4;
using Cosmos.Kernel.System.Network.IPv4.DHCP;

namespace HolmiumOS.Network;

public static class NetworkManager
{
    public static DnsClient DNSClient { get; private set; } = null!;

    public static int Ping(Address address, int timeout = 5000)
    {
        try
        {
            IcmpClient icmp = new();

            icmp.Connect(address);
            icmp.SendEcho();

            Cosmos.Kernel.System.Network.EndPoint source =
                new Cosmos.Kernel.System.Network.EndPoint(address, 0);

            int elapsed = icmp.Receive(ref source, timeout);

            icmp.Close();

            return elapsed;
        }
        catch
        {
            return -1;
        }
    }

    public static void Init()
    {
        try
        {
            Console.WriteLine("Initializing network...");

            NetworkStack.RemoveAllConfigIP();

            if (CosmosNetworkManager.DeviceCount == 0)
            {
                Console.WriteLine("No network device found.");
                return;
            }

            Console.WriteLine("Network device found.");
            Console.WriteLine("Device: " + CosmosNetworkManager.Name);
            Console.WriteLine("MAC: " + CosmosNetworkManager.MacAddress);
            Console.WriteLine("Link up: " + CosmosNetworkManager.LinkUp);
            Console.WriteLine("Ready: " + CosmosNetworkManager.Ready);

            DhcpClient dhcpClient = new();

            int result = dhcpClient.SendDiscoverPacket();

            if (result < 0)
            {
                Console.WriteLine("DHCP initialization failed.");
                return;
            }

            Console.WriteLine("DHCP configuration completed.");

            IPConfig? config =
                CosmosNetworkManager.Primary.IPConfig;

            if (config is not null)
            {
                Console.WriteLine("IP address: " + config.Address);
                Console.WriteLine("Subnet: " + config.SubnetMask);
                Console.WriteLine("Gateway: " + config.DefaultGateway);
            }

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

        try
        {
            NetworkStack.RemoveAllConfigIP();
        }
        catch
        {
        }
    }
}