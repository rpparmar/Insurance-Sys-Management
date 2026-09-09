using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Insurancesys.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMasterLoginService _masterLoginService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IMasterLoginService masterLoginService,
            ILogger<AccountController> logger)
        {
            _masterLoginService = masterLoginService;
            _logger = logger;
        }

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

        /// <summary>
        /// Displays the change-password form for the signed-in user.
        /// </summary>
        [Authorize(
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
            Roles = "SuperAdmin,AgencyAdmin,Agent")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        /// <summary>
        /// Validates the form, verifies the current password, and updates the stored hash.
        /// </summary>
        /// <param name="model">Posted current and new password values.</param>
        [Authorize(
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme,
            Roles = "SuperAdmin,AgencyAdmin,Agent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (!ModelState.IsValid)
                return View(model);

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
            {
                ModelState.AddModelError(string.Empty, Constants.ErrorMessages.MsgPasswordChangeFailed);
                return View(model);
            }

            var (success, message) = await _masterLoginService.ChangePasswordAsync(
                userId,
                model.OldPassword,
                model.NewPassword);

            if (!success)
            {
                _logger.LogWarning("Change password failed for user {UserId}", userId);

                if (string.Equals(message, Constants.ErrorMessages.MsgCurrentPasswordIncorrect, StringComparison.Ordinal))
                    ModelState.AddModelError(nameof(model.OldPassword), message);
                else
                    ModelState.AddModelError(string.Empty, message);

                return View(model);
            }

            TempData["RowsAffected"] = 1;
            TempData["Message"] = message;
            return RedirectToAction(nameof(ChangePassword));
        }
    }
}
