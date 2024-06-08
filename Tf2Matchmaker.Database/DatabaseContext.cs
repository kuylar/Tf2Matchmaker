using Microsoft.EntityFrameworkCore;
using Tf2Matchmaker.Common.Servers;

namespace Tf2Matchmaker.Database;

public class DatabaseContext : DbContext
{
	public DbSet<TFServer> Servers { get; set; }
	public DbSet<TFServerPlayerList> PlayerLists { get; set; }
	public DbSet<TFServerRules> Rules { get; set; }
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseNpgsql(GetConnectionString());

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		/*modelBuilder.Entity<TFServerRules>()
			.OwnsOne(c => c.Rules, d =>
			{
				d.ToJson();
			});
		modelBuilder.Entity<TFServerPlayerList>()
			.OwnsOne(c => c.Players, d =>
			{
				d.ToJson();
				d.OwnsMany(x => x);
			});*/
	}

	public static string GetConnectionString() =>
		Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ??
		"Host=localhost;Database=tfmatchmaker;Username=tfmatchmaker;Password=tfmatchmaker";
}