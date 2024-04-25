using MerrJep.ViewModels;
using MerrJepData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.Design;
using System.Linq;

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

		[HttpPost]
		public async Task<IActionResult> PlaceOrder(OrderVM order) {
			try
			{
				var user = await _userManager.GetUserAsync(User);
				var userId = await _userManager.GetUserIdAsync(user);
				if (user != null)
				{
					if(order != null)
					{
						var newOrder = new Order();
						var dateOrdered = DateTime.Now;
						newOrder.Name = $"{user.FirstName}_{user.LastName}_{dateOrdered}";
						newOrder.TotalPrice = order.TotalPrice;
						newOrder.DateOrdered = dateOrdered;
						newOrder.CurrencyId = await _context.Currencies.Where(x => x.Code == order.currencyCode).Select(x => x.Id).FirstOrDefaultAsync();
						newOrder.ApplicationUserId = userId;
						await _context.Orders.AddAsync(newOrder);
						await _context.SaveChangesAsync();

						foreach(var orderItem in order.OrderItems)
						{
							var newOrderItem = new OrderItem();
							newOrderItem.OrderId = newOrder.Id;
							newOrderItem.ItemId = orderItem.ItemID;
							newOrderItem.ItemQuantity = orderItem.Quantity;
							await _context.OrderItems.AddAsync(newOrderItem);
							await _context.SaveChangesAsync();

							var item = await _context.Items.Where(x => x.Id == orderItem.ItemID).FirstOrDefaultAsync();
							if(item != null)
							{
								item.AvailableQuantity -= orderItem.Quantity;
								await _context.SaveChangesAsync();
							}

							var cartItem = await _context.Carts.Where(x => x.ItemId == orderItem.ItemID && x.ApplicationUserId == userId && x.Invalidated == 20).FirstOrDefaultAsync();
							if(cartItem != null)
							{
								cartItem.Invalidated = 10;
								await _context.SaveChangesAsync();
							}
						}

						return Json("true");
					}
					return Json("false");
					
				}
				return Json("false");
			}
			catch (Exception ex)
			{
				return Json("false");
			}
		}

		[HttpPost]
		public async Task<IActionResult> ReplaceOrder (int OrderId)
		{
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = await _userManager.GetUserIdAsync(user);
                var previousOrder = await _context.Orders.Where(x => x.Id == OrderId).FirstOrDefaultAsync();
                if (previousOrder != null)
                {
					var orderItems = await _context.OrderItems.Include(x => x.Item).Where(x => x.OrderId == previousOrder.Id).ToListAsync();
					foreach(var item in orderItems)
					{
						if (item.Item.AvailableQuantity < item.ItemQuantity)
						{
							return Json(new
							{
								success = "false",
								message = $"Nuk ka mjaftueshem gjendje nga produkti {item.Item.Name}"
							});
						}
					}

					var newOrder = new Order();

                    var dateOrdered = DateTime.Now;
                    newOrder.Name = $"{user.FirstName}_{user.LastName}_{dateOrdered}";
                    newOrder.TotalPrice = previousOrder.TotalPrice;
                    newOrder.DateOrdered = dateOrdered;
					newOrder.CurrencyId = previousOrder.CurrencyId;
                    newOrder.ApplicationUserId = userId;
                    await _context.Orders.AddAsync(newOrder);
                    await _context.SaveChangesAsync();

                    foreach (var orderItem in orderItems)
                    {
                        var newOrderItem = new OrderItem();
                        newOrderItem.OrderId = newOrder.Id;
                        newOrderItem.ItemId = orderItem.ItemId;
                        newOrderItem.ItemQuantity = orderItem.ItemQuantity;
                        await _context.OrderItems.AddAsync(newOrderItem);
                        await _context.SaveChangesAsync();

                        var item = await _context.Items.Where(x => x.Id == orderItem.ItemId).FirstOrDefaultAsync();
                        if (item != null)
                        {
                            item.AvailableQuantity -= orderItem.ItemQuantity;
                            await _context.SaveChangesAsync();
                        }
                    }

                    return Json(new
					{
						success = "true",
						message = "Porosia u krye me sukses"
                    });
                }
                return Json(new
                {
                    success = "false",
                    message = "Ndodhi nje gabim gjate kryerjes se porosise"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = "false",
                    message = "Ndodhi nje gabim gjate kryerjes se porosise"
                });
            }
        }


    }
}
