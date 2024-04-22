using MerrJep.ViewModels;
using MerrJepData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
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
				var vm = new List<CartVM>();
				List<Cart> cartItems = await _context.Carts.Where(x => x.ApplicationUserId == userId
					 && x.Invalidated == 20)
					.ToListAsync();
				foreach(var cart in cartItems)
				{
					vm.Add(new CartVM
					{
						Cart = cart,
						Item = _context.Items.Where(x => x.Id == cart.ItemId)
											 .Include(x => x.ApplicationUser)
											 .Include(x => x.Images)
											 .Include(x => x.Currency).SingleOrDefault()
					});
				}
				return View("Cart", vm);
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


		[HttpPost]
		public async Task<IActionResult> DeleteItemFromCart(int itemId)
		{
			var user = await _userManager.GetUserAsync(User);
			var userId = await _userManager.GetUserIdAsync(user);
			var cartItem = await _context.Carts.Where(x => x.ItemId == itemId && x.ApplicationUserId == userId && x.Invalidated == 20).FirstOrDefaultAsync();
			if(cartItem == null)
			{
				return Json("false");
			}
			cartItem.Invalidated = 10;
			await _context.SaveChangesAsync();
			return Json("true");
		}


		[HttpPost]
		public async Task<IActionResult> DeleteAllItemsFromCart()
		{
			var user = await _userManager.GetUserAsync(User);
			var userId = await _userManager.GetUserIdAsync(user);
			var cartItems = await _context.Carts.Where(x => x.ApplicationUserId == userId && x.Invalidated == 20).ToListAsync();
			if (cartItems == null)
			{
				return Json("false");
			}
			foreach(var item in cartItems)
			{
				item.Invalidated = 10;
				await _context.SaveChangesAsync();
			}
			return Json("true");
		}
	}
}
