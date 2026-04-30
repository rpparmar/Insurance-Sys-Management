using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurancesys.web.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet("/Account/AccessDenied")]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            // Cookie auth uses ReturnUrl by default; also accept returnUrl for resilience.
            returnUrl ??= Request.Query["ReturnUrl"].ToString();

            // Prevent open redirect vulnerabilities: only allow local URLs.
            var safeReturnUrl = !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? returnUrl
                : null;

            ViewData["SafeReturnUrl"] = safeReturnUrl;
            return View();
        }
    }
}

