using Insurancesys.web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurancesys.web.Controllers;

/// <summary>
/// Serves the application Settings page. Phase 1 is display-only; save and test-email posts are deferred.
/// </summary>
[Authorize(
    AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
    Roles = "SuperAdmin,AgencyAdmin")]
public class SettingController : Controller
{
    /// <summary>
    /// Renders the Settings page with an empty email-configuration model.
    /// </summary>
    [HttpGet]
    [Route("Settings")]
    public IActionResult Index()
    {
        return View(new SettingsViewModel());
    }
}
