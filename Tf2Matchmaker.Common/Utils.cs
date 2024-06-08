namespace Tf2Matchmaker;

public static class Utils
{
	public static string ReadNullTerminatedString(this BinaryReader reader)
	{
		string res = "";
		char c;
		do
		{
			c = reader.ReadChar();
			res += c;
		} while (c != 0x00);

		return res;
	}

	public static ServerType GetServerType(this char b)
	{
		return b switch
		{
			'd' => ServerType.Dedicated,
			'l' => ServerType.NonDedicated,
			'p' => ServerType.SourceTv,
			_ => ServerType.Unknown
		};
	}

	public static ServerEnvironment GetServerEnvironment(this char b)
	{
		return b switch
		{
			'l' => ServerEnvironment.Linux,
			'w' => ServerEnvironment.Windows,
			'm' => ServerEnvironment.MacOs,
			'o' => ServerEnvironment.MacOs,
			_ => ServerEnvironment.Unknown
		};
	}
}