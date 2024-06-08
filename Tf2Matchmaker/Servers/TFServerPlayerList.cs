using System.Net;

namespace Tf2Matchmaker.Servers;

public class TFServerPlayerList
{
	public IPEndPoint Server { get; set; }
	public TFServerPlayer[] Players { get; set; }

	public TFServerPlayerList(IPEndPoint server, BinaryReader reader)
	{
		Server = server;
		byte playerCount = reader.ReadByte();
		Players = new TFServerPlayer[playerCount];
		for (int i = 0; i < playerCount; i++) 
			Players[i] = new TFServerPlayer(reader);
	}
}