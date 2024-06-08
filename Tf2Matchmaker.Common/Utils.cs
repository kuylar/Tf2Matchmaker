using System.Web;

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

		return res.TrimEnd('\0');
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

	public static Dictionary<string, string> FromQuery(this string encoded) =>
		encoded
			.Split('&')
			.Select(x => x.Split('='))
			.Where(x => x.Length >= 2)
			.ToDictionary(x => HttpUtility.UrlDecode(x[0]), x => HttpUtility.UrlDecode(x[1]));

	public static string ToQuery(this Dictionary<string, string> dictionary) =>
		string.Join('&',
			dictionary.Select(x => $"{HttpUtility.UrlEncode(x.Key)}={HttpUtility.UrlEncode(x.Value)}"));
}