using MerrJepData;

namespace MerrJep.ViewModels
{
	public class OrderVM
	{
		public List<OrderItemVM> OrderItems { get; set; }
		public Order Order { get; set; }
		public List<OrderItem> OrderItemList { get; set; }
		public double TotalPrice { get; set; }
		public string currencyCode { get; set; }
	}

	public class OrderItemVM
	{
		public int ItemID { get; set; }
		public int Quantity { get; set; }
		public double TotalItemPrice { get; set; }

	}
}
