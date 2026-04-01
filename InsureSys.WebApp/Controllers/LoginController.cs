using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Insurancesys.web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IMasterLoginService _masterLoginService;

        public LoginController(IMasterLoginService masterLoginService)
        {
            _masterLoginService = masterLoginService;
        }

        public IActionResult Login()
        {
            SignInViewModel model = new SignInViewModel();
            if (HttpContext.Request.Cookies.TryGetValue("username", out var username))
            {
                model = new SignInViewModel
                {
                    Username = username,
                    IsRemember = true
                };
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(SignInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _masterLoginService.AuthenticateAsync(model.Username, model.Password);
            if (user == null)
            {                
                ModelState.AddModelError("LoginError", "Invalid username or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("DisplayName", user.DisplayName ?? user.Username)
            };

            if (user.TenantId.HasValue)
                claims.Add(new Claim("TenantId", user.TenantId.Value.ToString()));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.IsRemember // This enables "Remember Me"
            };
            if (model.IsRemember)
                authProperties.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            await _masterLoginService.UpdateLastLoginAsync(user.UserId);

            if (model.IsRemember)
            {
                HttpContext.Response.Cookies.Append("username", model.Username, BuildUsernameCookieOptions(expiresUtc: DateTime.UtcNow.AddDays(7)));
            }
            else
            {
                HttpContext.Response.Cookies.Delete("username", BuildUsernameCookieOptions(expiresUtc: null));
            }

            if (user.Role == "SuperAdmin")
                return RedirectToAction("List", "TenantOnboarding");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }

        /// <summary>
        /// Username pre-fill cookie: HttpOnly + SameSite; Secure only when the request is HTTPS (localhost HTTP still works).
        /// </summary>
        private CookieOptions BuildUsernameCookieOptions(DateTime? expiresUtc)
        {
            var options = new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Strict,
                HttpOnly = true,
                Secure = Request.IsHttps
            };
            if (expiresUtc.HasValue)
                options.Expires = expiresUtc.Value;
            return options;
        }
    }
}
