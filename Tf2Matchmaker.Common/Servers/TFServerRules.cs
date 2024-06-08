using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json;

namespace Tf2Matchmaker.Common.Servers;

public class TFServerRules
{
	[Key]
	public string Id { get; set; }
	
	public string Ip { get; set; }
	public int Port { get; set; }
	
	[NotMapped]
	public IPEndPoint Server
	{
		get => new(IPAddress.Parse(Ip), Port);
		set
		{
			Ip = value.Address.ToString();
			Port = value.Port;
		}
	}

	[Column(TypeName = "jsonb")]
	public string RulesString
	{
		get => JsonSerializer.Serialize(Rules);
		set => JsonSerializer.Deserialize<Dictionary<string, string>>(value);
	}
	
	[NotMapped]
	public Dictionary<string, string> Rules { get; set; }

	// Empty ctor required by EFCore
#pragma warning disable CS8618
	public TFServerRules() { }
#pragma warning restore CS8618

	public TFServerRules(IPEndPoint server, BinaryReader reader)
	{
		Id = $"{server}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
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