using Insurancesys.web.Api.Models.Auth;
using Insurancesys.web.Services;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Insurancesys.web.Api
{    
    [ApiVersion("1.0")]    
    [Route("api/v{version:apiVersion}/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMasterLoginService _masterLoginService;
        private readonly JwtService _jwtService;
        private readonly IConfiguration _config;

        public AuthController(
            IMasterLoginService masterLoginService,
            JwtService jwtService,
            IConfiguration config)
        {
            _masterLoginService = masterLoginService;
            _jwtService = jwtService;
            _config = config;
        }

        /// <summary>
        /// Validates credentials and returns a signed JWT on success.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _masterLoginService.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username or password." });

            const string inactiveMessage = "Your account is inactive. Please contact system administrator.";

            if (!user.IsActive)
                return Unauthorized(new { message = inactiveMessage });

            if (user.AgencyId.HasValue)
            {
                var agency = user.AgencyDetails;
                if (agency == null || !agency.IsActive || agency.IsDeleted)
                    return Unauthorized(new { message = inactiveMessage });
            }

            await _masterLoginService.UpdateLastLoginAsync(user.UserId);

            var token = _jwtService.GenerateToken(user);
            var expiryMinutes = int.TryParse(_config["JwtSettings:ExpiryMinutes"], out var mins) ? mins : 60;

            var roleName = Enum.GetName(typeof(Roles), user.Role) ?? string.Empty;

            var response = new LoginResponse
            {
                Token = token,
                Username = user.Username,
                DisplayName = user.DisplayName ?? user.Username,
                Role = roleName,
                AgencyId = user.AgencyId,
                ExpiresInSeconds = expiryMinutes * 60
            };

            return Ok(response);
        }

        /// <summary>
        /// Stateless logout — the client discards the JWT.
        /// Retained as an endpoint for future token-blacklist or audit-log integration.
        /// </summary>
        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully." });
        }

        /// <summary>
        /// Returns the authenticated user's profile from the validated JWT claims.
        /// Used by the Angular app to restore session state on page refresh.
        /// </summary>
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var displayName = User.FindFirstValue("DisplayName");
            var agencyIdClaim = User.FindFirstValue("AgencyId");
            int? agencyId = agencyIdClaim != null && int.TryParse(agencyIdClaim, out var aid) ? aid : null;

            return Ok(new
            {
                userId,
                username,
                role,
                displayName,
                agencyId
            });
        }
    }
}
