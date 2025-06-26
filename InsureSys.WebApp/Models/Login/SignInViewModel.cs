using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
	public class SignInViewModel
	{
		[Required(ErrorMessage = "Username is required")]
		public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
		public string Password { get; set; } = string.Empty;
        public bool IsRemember { get; set; }
	}
}
