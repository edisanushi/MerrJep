using System.ComponentModel.DataAnnotations;

namespace MerrJep.ViewModels
{
	public class RegisterVM
	{
		public string Emri { get; set; }
		public string Mbiemri { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string KonfirmoFjalekalim { get; set; }
	}
}
