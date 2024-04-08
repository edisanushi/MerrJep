﻿using MerrJep.ViewModels;
using MerrJepData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MerrJep.Controllers
{
	public class AccountController : Controller
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserEmailStore<ApplicationUser> _emailStore;

		public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
		}
		public string ReturnUrl { get; set; }
		[TempData]
		public string ErrorMessage { get; set; }


		[HttpGet]
		public IActionResult Login(string returnUrl = null)
		{
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
					return View();
				}
			}

			// If we got this far, something failed, redisplay form
			return View();
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM Input)
		{
			if (ModelState.IsValid)
			{
				var user = CreateUser();

				await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
					var result = await _userManager.CreateAsync(user, Input.Password);
					var result2 = result;
					if (result.Succeeded)
					{
						var userId = await _userManager.GetUserIdAsync(user);
						await _signInManager.SignInAsync(user, isPersistent: false);
						return Json("true");
					}
			}

			// If we got this far, something failed, redisplay form
			return Json("false");
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
	}
}