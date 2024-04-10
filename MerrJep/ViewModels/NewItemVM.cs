namespace MerrJep.ViewModels
{
	public class NewItemVM
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public int AvailableQuantity { get; set; }
		public double Price { get; set; }
		public ImageObj Thumbnail { get; set; }
		public List<ImageObj> Images { get; set; }
		public string UserId { get; set; }
	}

	public class ImageObj
	{
		public string Content { get; set; }
		public string FileName { get; set; }
	}
}
