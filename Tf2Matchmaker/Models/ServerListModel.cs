using Tf2Matchmaker.Common.Servers;

namespace Tf2Matchmaker.Models;

public class ServerListModel
{
	public ServerFilters Filters { get; set; }
	public TFServer[] Servers { get; set; }
}