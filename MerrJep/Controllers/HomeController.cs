using MerrJep.Models;
using MerrJepData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace MerrJep.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_logger = logger;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index(int? currentPage)
		{
			int itemsPerPage = 9;
			if (currentPage == null || currentPage < 1)
			{
				currentPage = 1;
			}
			var user = await _userManager.GetUserAsync(User);
			var userId = await _userManager.GetUserIdAsync(user);
			double total = _context.Items.Where(x => x.AvailableQuantity > 0 && x.ApplicationUserId != userId).Count();
			double nrOfPages = Math.Ceiling(total / itemsPerPage);
			if(currentPage > nrOfPages)
			{
				currentPage = Convert.ToInt32(nrOfPages);
			}
			var offset = Convert.ToInt32((currentPage - 1) * itemsPerPage);
			var itemsList = _context.Items
				.Where(x => x.AvailableQuantity > 0 && x.ApplicationUserId != userId)
				.Include(x => x.Images)
				.Include(x => x.ApplicationUser)
				.Include(x => x.Currency)
				.OrderByDescending(x => x.DateAdded)
				.Skip(offset)
				.Take(itemsPerPage).ToList();
			ViewBag.CurrentPage = currentPage;
			ViewBag.TotalPages = nrOfPages;
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
