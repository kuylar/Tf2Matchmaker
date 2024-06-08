using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Tf2Matchmaker.Common.Servers;

// ReSharper disable once InconsistentNaming
public class TFServer
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

	public byte Protocol { get; set; }
	public string Name { get; set; }
	public string Map { get; set; }
	public string Folder { get; set; }
	public string Game { get; set; }
	public short AppId { get; set; }
	public byte Players { get; set; }
	public byte MaxPlayers { get; set; }
	public byte Bots { get; set; }
	public ServerType Type { get; set; }
	public ServerEnvironment Environment { get; set; }
	public ServerVisibility Visibility { get; set; }
	public VacSecurityLevel Vac { get; set; }
	public string Version { get; set; }
	public short? Port { get; set; }
	public long? ServerSteamId { get; set; }
	public short? SpectatorPort { get; set; }
	public string? SpectatorServerName { get; set; }
	public string[]? Keywords { get; set; }
	public long? GameId { get; set; }
	public DateTimeOffset LastFetched { get; set; }

	// Empty ctor required by EFCore
#pragma warning disable CS8618
	public TFServer() { }
#pragma warning restore CS8618

	public TFServer(IPEndPoint ip, BinaryReader reader)
	{
		Id = $"{ip.Address.ToString()}:{ip.Port}";
		ServerHost = ip;
		Protocol = reader.ReadByte();
		Name = reader.ReadNullTerminatedString();
		Map = reader.ReadNullTerminatedString();
		Folder = reader.ReadNullTerminatedString();
		Game = reader.ReadNullTerminatedString();
		AppId = reader.ReadInt16();
		Players = reader.ReadByte();
		MaxPlayers = reader.ReadByte();
		Bots = reader.ReadByte();
		Type = reader.ReadChar().GetServerType();
		Environment = reader.ReadChar().GetServerEnvironment();
		Visibility = (ServerVisibility)reader.ReadByte();
		Vac = (VacSecurityLevel)reader.ReadByte();
		Version = reader.ReadNullTerminatedString();
		
		byte edf = reader.ReadByte();

		if ((edf & 0x80) != 0)
		{
			Port = reader.ReadInt16();
		}

		if ((edf & 0x10) != 0)
		{
			ServerSteamId = reader.ReadInt64();
		}

		if ((edf & 0x40) != 0)
		{
			SpectatorPort = reader.ReadInt16();
			SpectatorServerName = reader.ReadNullTerminatedString();
		}

		if ((edf & 0x20) != 0)
		{
			Keywords = reader.ReadNullTerminatedString().Split(",");
		}

		if ((edf & 0x01) != 0)
		{
			GameId = reader.ReadInt64();
		}
		LastFetched = DateTimeOffset.UtcNow;
	}
}