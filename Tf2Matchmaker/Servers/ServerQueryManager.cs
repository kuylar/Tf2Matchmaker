using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace Tf2Matchmaker.Servers;

public static class ServerQueryManager
{
	private static UdpClient serverQueryClient = new();

	public static async Task Listen()
	{
		serverQueryClient.Client.Bind(new IPEndPoint(IPAddress.Any, 0));
		while (true)
		{
			UdpReceiveResult udpReceiveResult = await serverQueryClient.ReceiveAsync();
			//Log.Information("Got message from {0}:{1}", 
			//	udpReceiveResult.RemoteEndPoint.Address.ToString(), udpReceiveResult.RemoteEndPoint.Port);
			_ = HandlePacket(udpReceiveResult.RemoteEndPoint, udpReceiveResult.Buffer);
		}
		// ReSharper disable once FunctionNeverReturns
	}

	private static async Task HandlePacket(IPEndPoint server, byte[] buffer)
	{
		using MemoryStream stream = new(buffer);
		using BinaryReader reader = new(stream);
		if (reader.ReadInt32() != -1)
		{
			Log.Debug("Not a simple packet, skipping");
			return;
		}

		char header = reader.ReadChar();
		switch (header)
		{
			case 'A': // S2C_CHALLENGE
				await QuerySingleServer(server, reader.ReadBytes(4));
				break;
			case 'I': // A2S_INFO
				//Log.Information("Got A2S_INFO");
				TFServer tfServer = new TFServer(server, reader);
/*
				Log.Information(
					"Server Info:\nprotocol: {0}\nname: {1}\nmap: {2}\nfolder: {3}\ngame: {4}\nappId: {5}\nplayers: {6}\nmaxPlayers: {7}\nbots: {8}\ntype: {9}\nenvironment: {10}\nvisibility: {11}\nvac: {12}\nversion: {13}\nport: {15}\nserverSteamId: {16}\nspectatorPort: {17}\nspectatorServerName: {18}\nkeywords: {19}\ngameId: {20}",
					tfServer.Protocol,
					tfServer.Name,
					tfServer.Map,
					tfServer.Folder,
					tfServer.Game,
					tfServer.AppId,
					tfServer.Players,
					tfServer.MaxPlayers,
					tfServer.Bots,
					tfServer.Type,
					tfServer.Environment,
					tfServer.Visibility,
					tfServer.Vac,
					tfServer.Version,
					tfServer.Port ?? -1,
					tfServer.ServerSteamId ?? -1,
					tfServer.SpectatorPort ?? -1,
					tfServer.SpectatorServerName ?? "<null>",
					string.Join(", ", tfServer.Keywords ?? []),
					tfServer.GameId ?? -1);*/
				if (tfServer.Players > 0)
					Log.Information("[VAC:{0}] {1,-50}   {2,-20}   {3,3}/{4,3} ({5,2} bots) | {6,-20} | {7}",
						tfServer.Vac, string.Join("", tfServer.Name.Take(48)), string.Join("", tfServer.Game.Take(18)),
						tfServer.Players, tfServer.MaxPlayers, tfServer.Bots, string.Join("", tfServer.Map.Take(19)),
						string.Join("", string.Join(", ", tfServer.Keywords ?? []).Take(50)));
				break;
			default:
				Log.Warning("Unknown header: 0x{0:X2}/{1}", (byte)header, header);
				Log.Warning("Packet data: {0}", Convert.ToBase64String(buffer));
				break;
		}
	}

	public static async Task QuerySingleServer(IPEndPoint serverEndpoint)
	{
		byte[] request =
		[
			// -1, Packet isnt split
			0xFF, 0xFF, 0xFF, 0xFF,
			// Source Engine Query
			0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E,
			0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00,
		];
		await serverQueryClient.SendAsync(request, serverEndpoint);
	}

	public static async Task QuerySingleServer(IPEndPoint serverEndpoint, byte[] challenge)
	{
		byte[] request =
		[
			// -1, Packet isnt split
			0xFF, 0xFF, 0xFF, 0xFF,
			// Source Engine Query
			0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E,
			0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00
		];
		await serverQueryClient.SendAsync(AppendChallenge(request, challenge), serverEndpoint);
	}

	private static byte[] AppendChallenge(byte[] packet, byte[]? challenge) =>
		packet.Concat(challenge ?? [0xFF, 0xFF, 0xFF, 0xFF]).ToArray();
}