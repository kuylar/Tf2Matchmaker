using Microsoft.AspNetCore.Mvc;
using Tf2Matchmaker.Common.Servers;
using Tf2Matchmaker.Database;
using Tf2Matchmaker.Models;

namespace Tf2Matchmaker.Controllers;

[Route("/database")]
public class DatabaseController(DatabaseContext db) : Controller
{
	[Route("list")]
	public async Task<IActionResult> ListServers()
	{
		IQueryable<TFServer> query = db.Servers.AsQueryable();
		return View(new ServerListModel
		{
			Filters = null,
			Servers = query.ToArray()
		});
	}
}