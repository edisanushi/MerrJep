﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerrJepData
{
	public class ApplicationUser : IdentityUser
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public ICollection<Item> Items { get; set; }
		public ICollection<Cart> Carts { get; set; }
		public ICollection<Order> Orders { get; set; }
	}
}
