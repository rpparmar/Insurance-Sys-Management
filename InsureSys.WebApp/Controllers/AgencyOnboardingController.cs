using Insurancesys.web.Models;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace Insurancesys.web.Controllers
{
    [Authorize(Roles = "SuperAdmin", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class AgencyOnboardingController : Controller
    {
        private readonly IAgencyOnboardingService _onboardingService;

        public AgencyOnboardingController(IAgencyOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tenants = await _onboardingService.GetAllAgencyAsync();
            return View(tenants);
        }

        [HttpGet]
        public IActionResult AddEditAgency()
        {
            return View(new AgencyOnboardingViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditAgency(AgencyOnboardingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdClaim, out var createdByUserId);

            var desiredDatabaseName = await GenerateUniqueDatabaseNameAsync(model.AdminUsername);
            if (desiredDatabaseName == null)
            {
                TempData["ErrorMessage"] = "Could not allocate a unique database name. Please try again.";
                return View(model);
            }

            var dto = new OnboardAgencyDto
            {
                AgencyCode = string.IsNullOrWhiteSpace(model.AgencyCode) ? null : model.AgencyCode.Trim(),
                AgencyName = model.AgencyName.Trim(),
                ContactEmail = string.IsNullOrWhiteSpace(model.ContactEmail) ? null : model.ContactEmail.Trim(),
                ContactPhone = string.IsNullOrWhiteSpace(model.ContactPhone) ? null : model.ContactPhone.Trim(),
                DesiredDatabaseName = desiredDatabaseName,
                AdminUsername = model.AdminUsername.Trim(),
                AdminPassword = model.AdminPassword,
                Notes = null
            };

            var (success, message) = await _onboardingService.OnboardAgencyAsync(dto, createdByUserId);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(List));
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }

        /// <summary>
        /// Middle segment for DB name: letters, digits, underscore; @ becomes underscore. Matches Insuresys_{segment}_{timestamp}.
        /// </summary>
        private static string SanitizeForDatabaseNameSegment(string adminUsername)
        {
            var s = adminUsername.Trim();
            var sb = new StringBuilder(s.Length);
            foreach (var c in s)
            {
                if (char.IsAsciiLetterOrDigit(c) || c == '_')
                    sb.Append(c);
                else if (c == '@')
                    sb.Append('_');
            }

            var result = sb.ToString();
            return string.IsNullOrEmpty(result) ? "agency" : result;
        }

        private async Task<string?> GenerateUniqueDatabaseNameAsync(string adminUsername)
        {
            var segment = SanitizeForDatabaseNameSegment(adminUsername);
            if (segment.Length > 80)
                segment = segment[..80];

            for (var attempt = 0; attempt < 10; attempt++)
            {
                var ts = DateTime.UtcNow.AddSeconds(attempt).ToString("yyyyMMddHHmmss");
                var name = $"Insuresys_{segment}_{ts}";
                if (name.Length > 128)
                    name = name[..128];

                if (!await _onboardingService.IsDatabaseNameExistsAsync(name))
                    return name;
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> IsAgencyCodeAvailable(string? agencyCode)
        {
            if (string.IsNullOrWhiteSpace(agencyCode))
                return Json(true);

            var exists = await _onboardingService.IsAgencyCodeExistsAsync(agencyCode.Trim());
            return Json(exists ? "This agency code is already in use." : (object)true);
        }

        [HttpGet]
        public async Task<IActionResult> IsAgencyNameAvailable(string? agencyName)
        {
            if (string.IsNullOrWhiteSpace(agencyName))
                return Json(true);

            var exists = await _onboardingService.IsAgencyNameExistsAsync(agencyName.Trim());
            return Json(exists ? "An agency with this name already exists." : (object)true);
        }

        [HttpGet]
        public async Task<IActionResult> IsAdminUsernameAvailable(string? adminUsername)
        {
            if (string.IsNullOrWhiteSpace(adminUsername))
                return Json(true);

            var exists = await _onboardingService.IsAdminUsernameExistsAsync(adminUsername.Trim());
            return Json(exists ? "This admin username is already in use." : (object)true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int agencyId)
        {
            var result = await _onboardingService.DeactivateAgencyAsync(agencyId);
            if (result)
                TempData["SuccessMessage"] = "Agency deactivated successfully.";
            else
                TempData["ErrorMessage"] = "Failed to deactivate agency.";

            return RedirectToAction(nameof(List));
        }
    }
}
