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

        int authorityEnd = value.IndexOfAny(new[] { '/', '?', '#' });

        string authority;

        if (authorityEnd >= 0)
        {
            authority = value[..authorityEnd];

            string remaining = value[authorityEnd..];

            if (remaining.StartsWith("?") || remaining.StartsWith("#"))
            {
                Path = "/" + remaining;
            }
            else
            {
                Path = remaining;
            }
        }
        else
        {
            authority = value;
            Path = "/";
        }

        int fragmentIndex = Path.IndexOf('#');

        if (fragmentIndex >= 0)
        {
            Path = Path[..fragmentIndex];
        }

        if (string.IsNullOrEmpty(Path))
        {
            Path = "/";
        }

        if (authority.StartsWith("["))
        {
            int bracketEnd = authority.IndexOf(']');

            if (bracketEnd >= 0)
            {
                Host = authority[1..bracketEnd];

                if (bracketEnd + 1 < authority.Length && authority[bracketEnd + 1] == ':')
                {
                    Port = authority[(bracketEnd + 2)..];
                }
            }
        }
        else
        {
            int portSeparator = authority.LastIndexOf(':');

            if (portSeparator > 0)
            {
                Host = authority[..portSeparator];
                Port = authority[(portSeparator + 1)..];
            }
            else
            {
                Host = authority;
            }
        }

        if (string.IsNullOrEmpty(Port))
        {
            Port = Scheme == "https" ? "443" : "80";
        }
    }

    public override string ToString()
    {
        string authority = Host;

        if (Host.Contains(':'))
        {
            authority = "[" + Host + "]";
        }

        bool defaultHttpPort = Scheme == "http" && Port == "80";
        bool defaultHttpsPort = Scheme == "https" && Port == "443";

        if (!defaultHttpPort && !defaultHttpsPort && HasPort)
        {
            authority += ":" + Port;
        }

        return $"{Scheme}://{authority}{Path}";
    }
}