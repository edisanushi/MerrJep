using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerrJepData
{
	public class Item
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int AvailableQuantity { get; set; }
		public double Price { get; set; }
		public string Description { get; set; }
		public DateTime DateAdded { get; set; }

		[MaxLength(450)]
		public string ApplicationUserId { get; set; }
		public virtual ApplicationUser ApplicationUser { get; set; }
		public ICollection<Image> Images { get; set; }
		public int CurrencyId { get; set; }
		public virtual Currency Currency { get; set; }
		public ICollection<Cart> Carts { get; set; }
	}
}
