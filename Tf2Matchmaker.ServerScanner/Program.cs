using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Tf2Matchmaker.Database;
using Tf2Matchmaker.ServerScanner;

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
List<IPEndPoint> servers = [];

DatabaseContext db = new();
db.Database.Migrate();
db.Dispose();

ServerQueryManager.OnServerReceived += (_, server) =>
{
	DatabaseContext database = new();
	try
	{
		database.Servers.Upsert(server);
		database.SaveChanges();
	}
	catch (Exception e)
	{
		Log.Error(e, "Failed to save changes for TFServer {0} to the server", server.Id);
	}
};

ServerQueryManager.OnServerRulesReceived += (_, rules) =>
{
	DatabaseContext database = new();
	try
	{
		database.Rules.Add(rules);
		database.SaveChanges();
	}
	catch (Exception e)
	{
		Log.Error(e, "Failed to save changes for TFServerRules {0} to the server", rules.Id);
	}
};

ServerQueryManager.OnServerPlayerListReceived += (_, playersList) =>
{
	DatabaseContext database = new();
	try
	{
		database.PlayerLists.Add(playersList);
		database.SaveChanges();
	}
	catch (Exception e)
	{
		Log.Error(e, "Failed to save changes for TFServerPlayerList {0} to the server", playersList.Id);
	}
};

Timer masterServerRefreshTimer = new(async _ =>
{
	Log.Information("Querying master server...");
	IPEndPoint[] newServers = await MasterServer.GetAllServers();
	servers.Clear();
	servers.AddRange(newServers);
	Log.Information("Found {0} servers", servers.Count);
}, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));

Timer refreshTimer = new(async _ =>
{
	Log.Information("Querying all servers...");
	Stopwatch sp = Stopwatch.StartNew();
	foreach (IPEndPoint server in servers)
		await ServerQueryManager.QuerySingleServer(server);
	Log.Information("Sent query packets to all {0} servers in {1}.", servers.Count, sp.Elapsed);
}, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));

await Task.Delay(-1);