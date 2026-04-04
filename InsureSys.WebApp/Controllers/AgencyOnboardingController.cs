using Insurancesys.web.Models;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public IActionResult Onboard()
        {
            return View(new AgencyOnboardingViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onboard(AgencyOnboardingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdClaim, out var createdByUserId);

            var dto = new OnboardAgencyDto
            {
                AgencyCode = model.AgencyCode,
                AgencyName = model.AgencyName,
                ContactEmail = model.ContactEmail,
                ContactPhone = model.ContactPhone,
                DesiredDatabaseName = model.DesiredDatabaseName,
                AdminUsername = model.AdminUsername,
                AdminPassword = model.AdminPassword,
                Notes = model.Notes
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

        [HttpGet]
        public async Task<IActionResult> IsAgencyCodeAvailable(string tenantCode)
        {
            var exists = await _onboardingService.IsAgencyCodeExistsAsync(tenantCode);
            return Json(exists ? "This agency code is already in use." : true);
        }

        [HttpGet]
        public async Task<IActionResult> IsDatabaseNameAvailable(string desiredDatabaseName)
        {
            var exists = await _onboardingService.IsDatabaseNameExistsAsync(desiredDatabaseName);
            return Json(exists ? "This database name is already in use." : true);
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
