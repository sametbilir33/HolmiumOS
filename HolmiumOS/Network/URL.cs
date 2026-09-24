using System;
using Cosmos.Kernel.System.Network;

namespace HolmiumOS.Network;

public sealed class URL
{
    public string Original { get; }

    public string Scheme { get; private set; } = "http";

    public string Host { get; private set; } = string.Empty;

    public string Port { get; private set; } = string.Empty;

    public string Path { get; private set; } = "/";

    public bool HasPort => !string.IsNullOrEmpty(Port);

    public Address? Address
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Host))
            {
                return null;
            }

            return NetworkManager.Resolve(Host);
        }
    }

    public URL(string url)
    {
        Original = url ?? string.Empty;
        Parse(Original);
    }

    private void Parse(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        string value = url.Trim();

        int schemeSeparator = value.IndexOf("://", StringComparison.Ordinal);

        if (schemeSeparator >= 0)
        {
            Scheme = value[..schemeSeparator].ToLowerInvariant();
            value = value[(schemeSeparator + 3)..];
        }

        int pathIndex = value.IndexOf('/');

        string authority;

        if (pathIndex >= 0)
        {
            authority = value[..pathIndex];
            Path = value[pathIndex..];
        }
        else
        {
            authority = value;
            Path = "/";
        }

        int portSeparator = authority.LastIndexOf(':');

        if (portSeparator > 0 &&
            portSeparator < authority.Length - 1 &&
            int.TryParse(authority[(portSeparator + 1)..], out _))
        {
            Host = authority[..portSeparator];
            Port = authority[(portSeparator + 1)..];
        }
        else
        {
            Host = authority;
            Port = Scheme == "https" ? "443" : "80";
        }
    }

    public override string ToString()
    {
        string authority = Host;

        if (HasPort)
        {
            bool defaultHttpPort =
                Scheme == "http" && Port == "80";

            bool defaultHttpsPort =
                Scheme == "https" && Port == "443";

            if (!defaultHttpPort && !defaultHttpsPort)
            {
                authority += ":" + Port;
            }
        }

        return $"{Scheme}://{authority}{Path}";
    }
}