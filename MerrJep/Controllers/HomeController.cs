using MerrJep.Models;
using MerrJepData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MerrJep.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
		{
			_context = context;
			_logger = logger;
		}

		public IActionResult Index(int? currentPage)
		{
			int itemsPerPage = 9;
			if (currentPage == null || currentPage < 1)
			{
				currentPage = 1;
			}
			//double total = _context.Items.Where(x => x.AvailableQuantity > 0).Count();
			double nrOfPages = 10;
			//double nrOfPages = Math.Ceiling(total / itemsPerPage);
			if(currentPage > nrOfPages)
				currentPage = Convert.ToInt32(nrOfPages);
			var itemsList = _context.Items
				.Where(x => x.AvailableQuantity > 0)
				.Include(x => x.Images).OrderByDescending(x => x.DateAdded)
				.Skip(0)
				.Take(9).ToList();
			ViewBag.CurrentPage = currentPage;
			ViewBag.TotalPages = 10;
			//var itemsList = _context.Items
			//	.Where(x => x.AvailableQuantity > 0)
			//	.Include(x => x.Images).OrderByDescending(x => x.DateAdded)
			//	.Skip((pageNumber-1)*itemsPerPage)
			//	.Take(itemsPerPage).ToList();
			return View(itemsList);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
