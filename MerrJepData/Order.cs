using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerrJepData
{
	public class Order
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double TotalPrice { get; set; }
		public DateTime DateOrdered { get; set; }
		public int CurrencyId { get; set; }
		public virtual Currency Currency { get; set; }
		public string ApplicationUserId { get; set; }
		public virtual ApplicationUser ApplicationUser { get; set; }
	}
}
