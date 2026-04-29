using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
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
        private const string SessionEditingAgencyId = "EditingAgencyId";
        private const string SessionOriginalAgencyName = "Original_AgencyName";
        private const string SessionOriginalAgencyCode = "Original_AgencyCode";
        private const string SessionOriginalAdminUsername = "Original_AdminUsername";

        private readonly IAgencyOnboardingService _onboardingService;

        public AgencyOnboardingController(IAgencyOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        [Route("Agencies")]
        [HttpGet]
        public IActionResult AgencyList()
        {
            return View("AgencyList");
        }

        [HttpPost]        
        public async Task<IActionResult> GetData()
        {
            var result = await DataTableHelper.BuildGridResponseAsync(Request, _onboardingService.GetAgencyListGridAsync);
            return Json(result);
        }

        [HttpGet("Agencies/Add")]
        public IActionResult AddEditAgency()
        {
            ClearAgencyEditSession();
            return View(new AgencyOnboardingViewModel { IsEditMode = false, IsActive = true });
        }

        [HttpGet("Agencies/Edit/{id:int}")]
        public async Task<IActionResult> AddEditAgency(int id)
        {
            var dto = await _onboardingService.GetAgencyByIdAsync(id);
            if (dto == null)
            {
                SetTempDataForNoRecord();
                return RedirectToAction(nameof(AgencyList));
            }

            HttpContext.Session.SetString(SessionEditingAgencyId, id.ToString());
            HttpContext.Session.SetString(SessionOriginalAgencyName, dto.AgencyName);
            HttpContext.Session.SetString(SessionOriginalAgencyCode, dto.AgencyCode ?? string.Empty);
            var adminUsername = await _onboardingService.GetAgencyAdminUsernameAsync(id) ?? string.Empty;
            HttpContext.Session.SetString(SessionOriginalAdminUsername, adminUsername);
            var adminProfile = await _onboardingService.GetAgencyAdminProfileAsync(id);

            var model = new AgencyOnboardingViewModel
            {
                AgencyId = dto.AgencyId,
                IsEditMode = true,
                AgencyCode = dto.AgencyCode,
                AgencyName = dto.AgencyName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                IsActive = dto.IsActive,
                DatabaseNameDisplay = dto.DatabaseName,
                AdminUsername = adminUsername,
                IsAdminUsernameEditEnabled = false,
                FirstName = adminProfile?.FirstName ?? string.Empty,
                MiddleName = adminProfile?.MiddleName,
                LastName = adminProfile?.LastName ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAgency(AgencyOnboardingViewModel model)
        {
            if (model.IsEditMode)
            {
                ModelState.Remove(nameof(model.AdminPassword));
                ModelState.Remove(nameof(model.ConfirmPassword));

                if (!model.IsAdminUsernameEditEnabled)
                {
                    ModelState.Remove(nameof(model.AdminUsername));
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(model.AdminUsername))
                        ModelState.AddModelError(nameof(model.AdminUsername), "Enter username");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(model.AdminUsername))
                    ModelState.AddModelError(nameof(model.AdminUsername), "Enter username");
                if (string.IsNullOrWhiteSpace(model.AdminPassword))
                    ModelState.AddModelError(nameof(model.AdminPassword), "Enter password");
                else if (model.AdminPassword.Length < 8)
                    ModelState.AddModelError(nameof(model.AdminPassword), "Password must be at least 8 characters");
                if (model.AdminPassword != model.ConfirmPassword)
                    ModelState.AddModelError(nameof(model.ConfirmPassword), "Passwords do not match");
            }

            if (!ModelState.IsValid)
                return View(model);

            if (model.IsEditMode)
            {
                if (model.IsAdminUsernameEditEnabled)
                {
                    var originalAdmin = HttpContext.Session.GetString(SessionOriginalAdminUsername) ?? string.Empty;
                    var desired = (model.AdminUsername ?? string.Empty).Trim();
                    if (!string.Equals(originalAdmin, desired, StringComparison.Ordinal))
                    {
                        var u = await _onboardingService.UpdateAgencyAdminUsernameAsync(model.AgencyId, desired);
                        if (u == -1)
                        {
                            ModelState.AddModelError(nameof(model.AdminUsername), "This username is already in use.");
                            return View(model);
                        }
                    }
                }

                // Update Agency Admin profile fields (First/Middle/Last) without changing existing flows.
                var profileOk = await _onboardingService.UpdateAgencyAdminProfileAsync(
                    model.AgencyId,
                    new AgencyAdminProfileDto
                    {
                        FirstName = model.FirstName,
                        MiddleName = model.MiddleName,
                        LastName = model.LastName
                    });
                if (!profileOk)
                {
                    TempData["RowsAffected"] = 0;
                    TempData["Message"] = Constants.ErrorMessages.MsgUpdateFailure;
                    return View(model);
                }

                var updateDto = new AgencyDetailsDto
                {
                    AgencyId = model.AgencyId,
                    AgencyName = model.AgencyName.Trim(),
                    AgencyCode = string.IsNullOrWhiteSpace(model.AgencyCode) ? null : model.AgencyCode.Trim(),
                    ContactEmail = string.IsNullOrWhiteSpace(model.ContactEmail) ? null : model.ContactEmail.Trim(),
                    ContactPhone = string.IsNullOrWhiteSpace(model.ContactPhone) ? null : model.ContactPhone.Trim(),
                    IsActive = model.IsActive
                };

                var rows = await _onboardingService.UpdateAgencyDetailsAsync(updateDto);
                TempData["RowsAffected"] = rows == 1 ? 1 : 0;
                if (rows == 1)
                {
                    TempData["Message"] = Constants.SuccessMessages.MsgUpdateSuccess;
                    ClearAgencyEditSession();
                    return RedirectToAction(nameof(AgencyList));
                }

                if (rows == -1)
                    ModelState.AddModelError(nameof(model.AgencyName), "An agency with this name already exists.");
                else if (rows == -2)
                    ModelState.AddModelError(nameof(model.AgencyCode), "This agency code is already in use.");
                else if (rows == 0)
                    TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
                else
                    TempData["Message"] = Constants.ErrorMessages.MsgUpdateFailure;

                return View(model);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(userIdClaim, out var createdByUserId);

            var desiredDatabaseName = await GenerateUniqueDatabaseNameAsync(model.AdminUsername!.Trim());
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
                AdminUsername = model.AdminUsername!.Trim(),
                AdminPassword = model.AdminPassword!,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Notes = null
            };

            var (success, message) = await _onboardingService.OnboardAgencyAsync(dto, createdByUserId);

            if (success)
            {
                TempData["Message"] = message;
                TempData["RowsAffected"] = 1;
                return RedirectToAction(nameof(AgencyList));
            }

            TempData["ErrorMessage"] = message;
            return View(model);
        }

        private void ClearAgencyEditSession()
        {
            HttpContext.Session.Remove(SessionEditingAgencyId);
            HttpContext.Session.Remove(SessionOriginalAgencyName);
            HttpContext.Session.Remove(SessionOriginalAgencyCode);
            HttpContext.Session.Remove(SessionOriginalAdminUsername);
        }

        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        }

        private int? GetEditingAgencyIdFromSession()
        {
            var s = HttpContext.Session.GetString(SessionEditingAgencyId);
            return int.TryParse(s, out var id) ? id : null;
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

            var excludeId = GetEditingAgencyIdFromSession();
            var original = HttpContext.Session.GetString(SessionOriginalAgencyCode) ?? string.Empty;
            var trimmed = agencyCode.Trim();
            if (excludeId.HasValue && string.Equals(trimmed, original, StringComparison.Ordinal))
                return Json(true);

            var exists = await _onboardingService.IsAgencyCodeExistsAsync(trimmed, excludeId);
            return Json(exists ? "This agency code is already in use." : (object)true);
        }

        [HttpGet]
        public async Task<IActionResult> IsAgencyNameAvailable(string? agencyName)
        {
            if (string.IsNullOrWhiteSpace(agencyName))
                return Json(true);

            var excludeId = GetEditingAgencyIdFromSession();
            var original = HttpContext.Session.GetString(SessionOriginalAgencyName) ?? string.Empty;
            var trimmed = agencyName.Trim();
            if (excludeId.HasValue && string.Equals(trimmed, original, StringComparison.OrdinalIgnoreCase))
                return Json(true);

            var exists = await _onboardingService.IsAgencyNameExistsAsync(trimmed, excludeId);
            return Json(exists ? "An agency with this name already exists." : (object)true);
        }

        [HttpGet]
        public async Task<IActionResult> IsAdminUsernameAvailable(string? adminUsername)
        {
            if (string.IsNullOrWhiteSpace(adminUsername))
                return Json(true);

            var original = HttpContext.Session.GetString(SessionOriginalAdminUsername) ?? string.Empty;
            var trimmed = adminUsername.Trim();
            if (GetEditingAgencyIdFromSession().HasValue && string.Equals(original, trimmed, StringComparison.Ordinal))
                return Json(true);

            var exists = await _onboardingService.IsAdminUsernameExistsAsync(trimmed);
            return Json(exists ? "This admin username is already in use." : (object)true);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _onboardingService.DeactivateAgencyAsync(id);
            return new JsonResult(ok);
        }
    }
}
