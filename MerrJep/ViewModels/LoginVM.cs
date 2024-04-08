using System.ComponentModel.DataAnnotations;

namespace MerrJep.ViewModels
{
	public class LoginVM
	{
		[Required(ErrorMessage = "Ju lutem plotesoni Username")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Ju lutem plotesoni Fjalekalimin")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
