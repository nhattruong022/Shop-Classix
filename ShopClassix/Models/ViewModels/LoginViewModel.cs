using System.ComponentModel.DataAnnotations;

namespace Shop_Classix.Models.ViewModels
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Please enter your email.")]
		[EmailAddress(ErrorMessage = "Invalid email address.")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[MinLength(8, ErrorMessage = "Password must have at least 8 characters.")]
		public string Password { get; set; }
	}
}
