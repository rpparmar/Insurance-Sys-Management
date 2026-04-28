namespace Insurancesys.web.Api.Models.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? AgencyId { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
}
