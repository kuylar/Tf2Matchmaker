using System.Net;
using System.Net.Sockets;
using System.Text;
using Serilog;

namespace Tf2Matchmaker.ServerScanner;

public static class MasterServer
{
	private static IPEndPoint? masterServerEndpoint;
	private static UdpClient masterServerClient = new();
	public static State CurrentState { get; private set; } = State.Unknown;

	private static readonly byte[] QueryHeader =
	[
		// Header/Message Type
		0x31,
		// Location - Everywhere
		0xFF
	];

	private static readonly byte[] QueryFilters =
	[
		// \appid\440  - Servers running TF2
		0x5c, 0x61, 0x70, 0x70, 0x69, 0x64, 0x5c, 0x34, 0x34, 0x30,
		// \password\0 - Servers without password protection
		0x5c, 0x70, 0x61, 0x73, 0x73, 0x77, 0x6f, 0x72, 0x64, 0x5c, 0x30, 
		// Null terminator
		0x00
	]; 

	public enum State
	{
		Unknown = 0,
		Idle = 1,
		GettingList = 2
	}

	public static void Init()
	{
		masterServerEndpoint = new IPEndPoint(Dns.GetHostAddresses("hl2master.steampowered.com")[0], 27011);
		CurrentState = State.Idle;
	}
	public static async Task<IPEndPoint[]> GetAllServers()
	{
		List<IPEndPoint> res = [];
		if (CurrentState != State.Idle)
		{
			Log.Error("GetAllServers called while MasterServer.State was {0}", CurrentState);
			return [];
		}

		if (masterServerEndpoint is null)
		{
			Log.Error("masterServerEndpoint not initialized");
			return [];
		}

		// Connection should be recreated between sessions to get new lists?
		// Not sure, see the last line of https://developer.valvesoftware.com/wiki/Master_Server_Query_Protocol#IP:Port
		masterServerClient.Connect(masterServerEndpoint);
		string lastServer = "0.0.0.0:0";
		bool complete = false;
		while (!complete)
		{
			await masterServerClient.SendAsync(GetQueryPacket(lastServer));
			byte[] packet = masterServerClient.Receive(ref masterServerEndpoint);
			IEnumerable<byte[]> ips = packet.Chunk(6);
			foreach (byte[] ipBytes in ips)
			{
				if (ipBytes.Length != 6)
				{
					Log.Warning("Incomplete IP: {0}",
						string.Join(" ",
							Convert.ToHexString(ipBytes).ToUpper().Chunk(2).Select(x => $"0x{new string(x)}")));
					continue;
				}

				if (ipBytes[0] == 0xFF &&
				    ipBytes[1] == 0xFF &&
				    ipBytes[2] == 0xFF &&
				    ipBytes[3] == 0xFF)
				{
					Log.Debug("Got message header");
					continue;
				}

				if (ipBytes[0] == 0x00 &&
				    ipBytes[1] == 0x00 &&
				    ipBytes[2] == 0x00 &&
				    ipBytes[3] == 0x00 &&
				    ipBytes[4] == 0x00 &&
				    ipBytes[5] == 0x00)
				{
					Log.Debug("Got end of list");
					complete = true;
					continue;
				}
				IPEndPoint serverIp = new(new IPAddress(ipBytes[..4]), BitConverter.ToUInt16(ipBytes[4..].Reverse().ToArray()));
				res.Add(serverIp);
				Log.Debug("Found server: {0}:{1}", serverIp.Address.ToString(), serverIp.Port);
				lastServer = serverIp.ToString();
			}
		}
		masterServerClient.Close();
		return res.ToArray();
	}

	private static byte[] GetQueryPacket(string lastIp)
	{
		if (!lastIp.EndsWith('\n'))
			lastIp += '\n';

		byte[] ipBytes = Encoding.ASCII.GetBytes(lastIp);
		return QueryHeader.Concat(ipBytes).Concat(QueryFilters).ToArray();
	}
}