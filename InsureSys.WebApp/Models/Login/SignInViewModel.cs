using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
	public class SignInViewModel
	{
		[Required(ErrorMessage = "Username is required")]
		public string Username { get; set; }
		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }
		public bool IsRemember { get; set; }
	}
}
