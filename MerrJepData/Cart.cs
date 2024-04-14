using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerrJepData
{
	public class Cart
	{
		public int Id { get; set; }
		public int ItemId {get; set;}
		public virtual Item Item { get; set; }
		[MaxLength(450)]
		public string ApplicationUserId { get; set; }
		public virtual ApplicationUser ApplicationUser { get; set; }
		public int AmountOfProduct { get; set; }
		public DateTime DateAdded { get; set; }
		public byte Invalidated { get; set; }
		public byte Ordered { get; set; }
	}
}
