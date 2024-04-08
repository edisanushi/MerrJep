using System.ComponentModel.DataAnnotations;

namespace MerrJep.ViewModels
{
	public class RegisterVM
	{
		[Required(ErrorMessage = "Ju lutem plotesoni Emrin")]
		[RegularExpression("[ A-Za-z]*", ErrorMessage = "Emri nuk mund te kete numra apo karaktere speciale")]
		public string Emri { get; set; }

		[Required(ErrorMessage = "Ju lutem plotesoni Mbiemrin")]
		[RegularExpression("[ A-Za-z]*", ErrorMessage = "Mbiemri nuk mund te kete numra apo karaktere speciale")]
		public string Mbiemri { get; set; }

		[Required(ErrorMessage = "Ju lutem plotesoni Email")]
		[EmailAddress(ErrorMessage = "Ju lutem vendosni nje adrese te sakte emaili")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Ju lutem plotesoni Fjalekalimin")]
		[Display(Name = "Fjalekalimi")]
		[RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$", ErrorMessage = "Fjalekalimi duhet te kete te pakten 6 karaktere, te pakten nje shkronje te madhe, nje shkronje te vogel, nje numer dhe nje karakter")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required(ErrorMessage = "Ju lutem konfirmoni Fjalekalimin")]
		[Display(Name = "Konfirmo Fjalekalimin")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Fjalekalimi nuk u konfirmua. Rishkruaj konfirmimin e fjalekalimit.")]
		public string KonfirmoFjalekalim { get; set; }
	}
}
