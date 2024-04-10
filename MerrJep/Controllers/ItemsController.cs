﻿using MerrJep.ViewModels;
using MerrJepData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MerrJep.Controllers
{
	public class ItemsController : Controller
	{

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ApplicationDbContext _context;

		public ItemsController(UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
		{
			_userManager = userManager;
			_context = dbContext;
		}


		[HttpGet]
		public IActionResult AddItem()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddItem(NewItemVM item)
		{
			try
			{
				var newItem = new Item();
				newItem.Name = item.Name;
				newItem.Description = item.Description;
				newItem.Price = item.Price;
				newItem.AvailableQuantity = item.AvailableQuantity;
				newItem.DateAdded = DateTime.Now;
				var user = await _userManager.GetUserAsync(User);
				var userId = await _userManager.GetUserIdAsync(user);
				newItem.ApplicationUserId = await _userManager.GetUserIdAsync(user);
				_context.Items.Add(newItem);
				_context.SaveChanges();

				var mainImage = new Image();
				mainImage.ImageName = item.Thumbnail.FileName;
				mainImage.ImageByte = System.Convert.FromBase64String(item.Thumbnail.Content.Split(',')[1]);
				mainImage.IsMainImage = 1;
				mainImage.ItemId = newItem.Id;
				_context.Images.Add(mainImage);
				_context.SaveChanges();
				if(item.Images != null)
				{
					foreach (var image in item.Images)
					{
						var addImage = new Image();
						addImage.ImageName = image.FileName;
						addImage.ImageByte = System.Convert.FromBase64String(image.Content.Split(',')[1]);
						addImage.IsMainImage = 0;
						addImage.ItemId = newItem.Id;
						_context.Images.Add(addImage);
						_context.SaveChanges();
					}
				}
				return Json(new { valid = "true" });
			}
			catch (Exception ex)
			{
				return Json(new {valid="false", message = "Ndodhi nje gabim gjate shtimit te artikullit. Ju lutem provoni perseri" });
			}
		}

		public IActionResult ItemAdded()
		{
			return View();
		}
	}
}