namespace Tf2Matchmaker.Common.Servers;

public class TFServerPlayer
{
	public string Name { get; set; }
	public int Score { get; set; }
	public TimeSpan OnlineDuration { get; set; }

	// Empty ctor required by EFCore
#pragma warning disable CS8618
	public TFServerPlayer() { }
#pragma warning restore CS8618

	public TFServerPlayer(BinaryReader reader)
	{
		// Player index, *always* returns 0x00 for some reason
		reader.ReadByte();
		Name = reader.ReadNullTerminatedString();
		Score = BitConverter.ToInt32(reader.ReadBytes(4));
		OnlineDuration = TimeSpan.FromSeconds(BitConverter.ToSingle(reader.ReadBytes(4)));
	}
}