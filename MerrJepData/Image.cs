using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerrJepData
{
	public class Image
	{
		public int Id { get; set; }
		public string ImageName { get; set; }
		public byte[] ImageByte { get; set; }
		public byte IsMainImage { get; set; }
		public int ItemId { get; set; }
		public virtual Item Item { get; set; }
	}
}
