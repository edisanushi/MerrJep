using MerrJep.ViewModels;
using MerrJepData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MerrJep.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserEmailStore<ApplicationUser> _emailStore;
		private readonly ApplicationDbContext _context;

		public AccountController(SignInManager<ApplicationUser> signInManager, 
			UserManager<ApplicationUser> userManager, 
			IUserStore<ApplicationUser> userStore,
			ApplicationDbContext context)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_context = context;
		}
		public string ReturnUrl { get; set; }
		[TempData]
		public string ErrorMessage { get; set; }


		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginVM Input)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: false);
				if (result.Succeeded)
				{
                    return Json(new { valid = "true" });
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Json(new {valid="false", message = "Ju lutem vendosni kredencialet e sakta" });
				}
			}
			return Json(new {success = "false", message = "Ka nodhur nje gabim" });
		}

		[HttpGet]
		public IActionResult Register()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM Input)
		{
			if (ModelState.IsValid)
			{
				var user = CreateUser();
				user.FirstName = Input.Emri;
				user.LastName = Input.Mbiemri;
				await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
				var result = await _userManager.CreateAsync(user, Input.Password);
				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(user, isPersistent: false);
					return Json(new
					{
						success = "true",
					});
				}
				else
				{
					return Json(new
					{
						success = "false",
						message = "Ndodhi nje gabim gjate regjistrimit"
					});
				}
			}


			return Json(new
			{
				success = false,
				message = "Ndodhi nje gabim gjate regjistrimit"
			}); ;
		}


		private ApplicationUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<ApplicationUser>();
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
					$"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}


		private IUserEmailStore<ApplicationUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<ApplicationUser>)_userStore;
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Account", "Login");
		}


		[HttpGet]
		public async Task<IActionResult> GetMyAccount()
		{
			try
			{
				var user = await _userManager.GetUserAsync(User);
				var userId = await _userManager.GetUserIdAsync(user);
				var orders = _context.Orders.Include(x => x.Currency).Where(x => x.ApplicationUserId == userId).ToList();
				var accountVM = new AccountVM();
				var orderList = new List<OrderVM>();
				foreach(var order in orders)
				{
					var orderVm = new OrderVM();
					orderVm.Order = order;
					orderVm.OrderItemList = _context.OrderItems
						.Include(x => x.Item)
						.ThenInclude(x => x.Currency)
						.Where(x => x.OrderId == order.Id).ToList();
					orderList.Add(orderVm);
				}
				accountVM.Orders = orderList;
				var itemList = new List<Item>();
				var items = _context.Items.Include(x => x.Currency)
					.Include(x => x.Images)
					.Include(x => x.ApplicationUser)
					.Where(x => x.ApplicationUserId == userId && x.Invalidated == 20).ToList();
				accountVM.Items = items;
				return View("MyAccount", accountVM);
			}
			catch (Exception ex)
			{
				return View("MyAccount", new AccountVM { 
					Orders = new List<OrderVM>(),
				    Items = new List<Item>()});
			}
		}

		
	}
}
