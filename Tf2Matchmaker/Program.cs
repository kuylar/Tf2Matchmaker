using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.CreateBootstrapLogger();

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
