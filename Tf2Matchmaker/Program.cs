using System.Net;
using Serilog;
using Serilog.Events;
using Tf2Matchmaker.Servers;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateBootstrapLogger();

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
foreach (IPEndPoint server in servers)
{
	ServerQueryManager.QuerySingleServer(server);
}

await Task.Delay(-1);
/*
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSerilog();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
*/