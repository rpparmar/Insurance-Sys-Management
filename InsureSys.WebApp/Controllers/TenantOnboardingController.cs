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
    public class TenantOnboardingController : Controller
    {
        private readonly ITenantOnboardingService _onboardingService;

        public TenantOnboardingController(ITenantOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var tenants = await _onboardingService.GetAllTenantsAsync();
            return View(tenants);
        }

        [HttpGet]
        public IActionResult Onboard()
        {
            return View(new TenantOnboardingViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Onboard(TenantOnboardingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdClaim, out var createdByUserId);

            var dto = new OnboardTenantDto
            {
                TenantCode = model.TenantCode,
                AgencyName = model.AgencyName,
                ContactEmail = model.ContactEmail,
                ContactPhone = model.ContactPhone,
                DesiredDatabaseName = model.DesiredDatabaseName,
                AdminUsername = model.AdminUsername,
                AdminPassword = model.AdminPassword,
                Notes = model.Notes
            };

            var (success, message) = await _onboardingService.OnboardTenantAsync(dto, createdByUserId);

            if (success)
            {
                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(List));
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> IsTenantCodeAvailable(string tenantCode)
        {
            var exists = await _onboardingService.IsTenantCodeExistsAsync(tenantCode);
            return Json(exists ? "This tenant code is already taken." : true);
        }

        [HttpGet]
        public async Task<IActionResult> IsDatabaseNameAvailable(string desiredDatabaseName)
        {
            var exists = await _onboardingService.IsDatabaseNameExistsAsync(desiredDatabaseName);
            return Json(exists ? "This database name is already in use." : true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int tenantId)
        {
            var result = await _onboardingService.DeactivateTenantAsync(tenantId);
            if (result)
                TempData["SuccessMessage"] = "Tenant deactivated successfully.";
            else
                TempData["ErrorMessage"] = "Failed to deactivate tenant.";

            return RedirectToAction(nameof(List));
        }
    }
}
