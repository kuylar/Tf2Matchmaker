using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json;

namespace Tf2Matchmaker.Common.Servers;

public class TFServerPlayerList
{
	[Key]
	public string Id { get; set; }
	
	public string Ip { get; set; }
	public int ServerPort { get; set; }
	
	[NotMapped]
	public IPEndPoint ServerHost
	{
		get => new(IPAddress.Parse(Ip), ServerPort);
		set
		{
			Ip = value.Address.ToString();
			ServerPort = value.Port;
		}
	}

	[Column(TypeName = "jsonb")]
	public string PlayersJson {
		get => JsonSerializer.Serialize(Players);
		set => JsonSerializer.Deserialize<TFServerPlayer[]>(value);
	}

	[NotMapped]
	public TFServerPlayer[] Players { get; set; }

	// Empty ctor required by EFCore
#pragma warning disable CS8618
	public TFServerPlayerList() { }
#pragma warning restore CS8618

	public TFServerPlayerList(IPEndPoint server, BinaryReader reader)
	{
		Id = $"{server}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
		ServerHost = server;
		byte playerCount = reader.ReadByte();
		Players = new TFServerPlayer[playerCount];
		for (int i = 0; i < playerCount; i++) 
			Players[i] = new TFServerPlayer(reader);
	}
}