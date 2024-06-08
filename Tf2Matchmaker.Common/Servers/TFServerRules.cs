using System.Net;

namespace Tf2Matchmaker.Servers;

public class TFServerRules
{
	public IPEndPoint Server { get; set; }
	public Dictionary<string, string> Rules { get; set; }

	public TFServerRules(IPEndPoint server, BinaryReader reader)
	{
		Server = server;
		short rulesCount = reader.ReadInt16();
		Rules = new Dictionary<string, string>();
		for (int i = 0; i < rulesCount; i++)
		{
			string key = reader.ReadNullTerminatedString();
			string value = reader.ReadNullTerminatedString();
			Rules[key] = value;
		}
	}
}