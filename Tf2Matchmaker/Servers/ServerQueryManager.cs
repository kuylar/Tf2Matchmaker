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
				byte[] challenge = reader.ReadBytes(4);
				await QuerySingleServer(server, challenge);
				await QueryPlayers(server, challenge);
				await QueryRules(server, challenge);
				break;
			case 'I': // A2S_INFO
				TFServer tfServer = new(server, reader);
				if (tfServer.Players > 0)
					Log.Information("{8,-22} [VAC:{0}] {1,-50}   {2,-20}   {3,3}/{4,3} ({5,2} bots) | {6,-20} | {7}",
						tfServer.Vac, string.Join("", tfServer.Name.Take(48)), string.Join("", tfServer.Game.Take(18)),
						tfServer.Players, tfServer.MaxPlayers, tfServer.Bots, string.Join("", tfServer.Map.Take(19)),
						string.Join("", string.Join(", ", tfServer.Keywords ?? []).Take(50)), server);
				break;
			case 'D': // A2S_PLAYER
				TFServerPlayerList list = new(server, reader);
				Log.Information("{0,-22} Got {1} players:", server, list.Players.Length);
				for (int i = 0; i < list.Players.Length; i++)
				{
					TFServerPlayer player = list.Players[i];
					Log.Information("#[{0}] {1,-16} {2,-3} {3}", i, string.Join("", player.Name.Take(15)), player.Score,
						player.OnlineDuration);
				}
				break;
			case 'E': // A2S_RULES
				TFServerRules rules = new(server, reader);
				Log.Information("[{0}] Got {1} rules", server, rules.Rules.Count);
				foreach ((string? key, string? value) in rules.Rules) 
					Log.Information("{0}: {1}", key, value);
				break;
			case 'l': // ??? randomly got this
				Log.Information("[{0}] Sent message: {1}", server, reader.ReadNullTerminatedString().Trim());
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

	public static async Task QueryPlayers(IPEndPoint serverEndpoint, byte[] challenge)
	{
		byte[] request =
		[
			// -1, Packet isnt split
			0xFF, 0xFF, 0xFF, 0xFF,
			// 'U'
			0x55
		];
		await serverQueryClient.SendAsync(AppendChallenge(request, challenge), serverEndpoint);
	}

	public static async Task QueryRules(IPEndPoint serverEndpoint, byte[] challenge)
	{
		byte[] request =
		[
			// -1, Packet isnt split
			0xFF, 0xFF, 0xFF, 0xFF,
			// 'V'
			0x56
		];
		await serverQueryClient.SendAsync(AppendChallenge(request, challenge), serverEndpoint);
	}

	private static byte[] AppendChallenge(byte[] packet, byte[]? challenge) =>
		packet.Concat(challenge ?? [0xFF, 0xFF, 0xFF, 0xFF]).ToArray();
}