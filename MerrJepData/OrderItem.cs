using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerrJepData
{
	public class OrderItem
	{
		public int Id { get; set; }
		public int OrderId { get; set; }
		public virtual Order Order { get; set; }
		public int ItemId { get; set; }
		public virtual Item Item { get; set; }
		public int ItemQuantity { get; set; }
	}
}
