using System.Net;
using Serilog;
using Serilog.Events;
using Tf2Matchmaker.Servers;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateLogger();

_ = Task.Run(async () =>
{
	while (true)
	{
		try
		{
			await ServerQueryManager.Listen();
		}
		catch (Exception e)
		{
			Log.Error(e, "ServerQueryManager.Listen stopped, restarting...");
		}
	}
	// ReSharper disable once FunctionNeverReturns
});

MasterServer.Init();
IPEndPoint[] servers = await MasterServer.GetAllServers();
Log.Information("Found {0} servers", servers.Length);
// TODO: do this every 30 seconds/1 minutes/5 minuets/idk
foreach (IPEndPoint server in servers)
{
	ServerQueryManager.QuerySingleServer(server);
}

await Task.Delay(-1);