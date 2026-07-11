using Cosmos.System.Network.IPv4;

namespace HolmiumOS.Network;
public class URL
{
	public URL(string FullURL)
	{
		this.FullURL = FullURL;
	}
	public bool HasProtocol => FullURL.Contains(Delimiter);

	public bool HasPort => FullURL.Contains(':');

	public Address Address
	{
		get
		{
			NetworkManager.DNSClient.SendAsk(Host);
			return NetworkManager.DNSClient.Receive();
		}
	}

	public string Protocol
	{
		get
		{
			if (!HasProtocol)
			{
				return string.Empty;
			}

			return FullURL[..FullURL.IndexOf(Delimiter)];
		}
		set
		{
			if (!HasProtocol)
			{
				return;
			}

			FullURL = FullURL.Replace(Protocol + Delimiter, value + Delimiter);
		}
	}

	public string Host
	{
		get
		{
			string Temp = FullURL;

			if (HasProtocol)
			{
				Temp = Temp.Replace(Protocol + Delimiter, string.Empty);
			}

			return Temp.Split('/')[0].Split(':')[0];
		}
		set
		{
			FullURL = FullURL.Replace(Delimiter + Host, Delimiter + value);
		}
	}

	public string Path
	{
		get
		{
			string Temp = FullURL;

			if (HasProtocol)
			{
				Temp = Temp.Replace(Protocol + Delimiter, string.Empty);
			}

			return Temp.Split(Host + /*':' + Port +*/ '/')[1];
		}
		set
		{
			FullURL = FullURL.Replace(Host + /*':' + Port +*/ '/' + Path, Host + /*':' + Port +*/ '/' + value);
		}
	}

	public string Port
	{
		get
		{
			string Temp = FullURL;

			if (HasProtocol)
			{
				Temp = Temp.Replace(Protocol + Delimiter, string.Empty);
			}

			return Temp.Split('/')[0].Split(':')[1];
		}
	}
	public const string Delimiter = "://";

	public string FullURL;

}