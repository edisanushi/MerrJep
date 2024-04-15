using MerrJep.ViewModels;
using MerrJepData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace MerrJep.Controllers
{
	public class CartController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> GetUserCart()
		{
			var user = await _userManager.GetUserAsync(User);
			var userId = await _userManager.GetUserIdAsync(user);
			if (userId != null)
			{
				var cartItems = _context.Carts.Where(x => x.ApplicationUserId == userId
					 && x.Invalidated == 20)
					.ToList();
				return View("Cart", cartItems);
			}
			return RedirectToAction("Home", "Index");
		}



		[HttpPost]
		public async Task<IActionResult> AddItemToCart(NewCartItemVM vm)
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				var userId = await _userManager.GetUserIdAsync(user);
				if (user != null)
				{
					var newCartItem = new Cart();
					newCartItem.ApplicationUserId = userId;
					newCartItem.ItemId = vm.ItemId;
					newCartItem.AmountOfProduct = vm.Amount;
					newCartItem.DateAdded = DateTime.Now;
					newCartItem.Invalidated = 20;
					newCartItem.Ordered = 20;
					await _context.Carts.AddAsync(newCartItem);
					await _context.SaveChangesAsync();

					var item = await _context.Items.Where(x => x.Id == vm.ItemId).FirstOrDefaultAsync();
					if(item == null) {
						return Json("false");
					}
					item.AvailableQuantity -= vm.Amount;
					await _context.SaveChangesAsync();

					return Json("true");
				}
				return Json("false");
			}
			catch (Exception ex)
			{
				return Json("false");
			}
			
		}
	}
}
