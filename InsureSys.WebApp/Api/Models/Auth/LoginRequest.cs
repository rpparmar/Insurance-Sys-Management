using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Api.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool IsRemember { get; set; }
    }
}
