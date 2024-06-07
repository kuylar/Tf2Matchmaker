using System.Net;

namespace Tf2Matchmaker.Servers;

// ReSharper disable once InconsistentNaming
public class TFServer
{
	public string Id => ServerHost.ToString();
	public IPEndPoint ServerHost { get; set; }
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

	public TFServer(IPEndPoint ip, BinaryReader reader)
	{
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
	}
}