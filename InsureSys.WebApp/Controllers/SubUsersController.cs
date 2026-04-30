using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.Enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Insurancesys.web.Controllers
{
    [Authorize(Roles = "AgencyAdmin", AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class SubUsersController : Controller
    {
        private const string SessionOriginalUsername = "Original_SubUserUserName";

        private readonly ISubUsersService _subUsersService;

        public SubUsersController(ISubUsersService subUsersService)
        {
            _subUsersService = subUsersService;
        }

        [Route("SubUsers")]
        [HttpGet]
        public IActionResult SubUsersList()
        {
            return View("SubUsersList");
        }

        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var agencyId = GetAgencyIdOrDefault();
            if (agencyId <= 0)
                return Json(new { iTotalRecords = 0, iTotalDisplayRecords = 0, data = new List<object>() });

            var result = await DataTableHelper.BuildGridResponseAsync(
                Request,
                _subUsersService.GetSubUsersGridAsync,
                extraParamsFunc: _ => new Dictionary<string, object> { ["@AgencyId"] = agencyId }
            );

            return Json(result);
        }

        [HttpGet("SubUsers/Add")]
        [HttpGet("SubUsers/Edit/{id:int}")]
        public async Task<IActionResult> AddEditSubUser(int id = 0)
        {
            var agencyId = GetAgencyIdOrDefault();
            if (agencyId <= 0)
            {
                SetTempDataForNoRecord();
                return RedirectToAction(nameof(SubUsersList));
            }

            if (id <= 0)
            {
                HttpContext.Session.SetString(SessionOriginalUsername, "");
                var model = new SubUserViewModel
                {
                    IsEditMode = false,
                    IsActive = true,
                    Role = (int)Roles.Agent
                };
                ViewBag.RoleSelectList = BuildRoleSelectList();
                return View("AddEditSubUser", model);
            }

            var entity = await _subUsersService.GetSubUserByIdAsync(id, agencyId);
            if (entity == null)
            {
                SetTempDataForNoRecord();
                return RedirectToAction(nameof(SubUsersList));
            }

            HttpContext.Session.SetString(SessionOriginalUsername, entity.Username);
            var vm = new SubUserViewModel
            {
                UserId = entity.UserId,
                UserName = entity.Username,
                Role = entity.Role,
                FirstName = entity.FirstName ?? string.Empty,
                MiddleName = entity.MiddleName,
                LastName = entity.LastName ?? string.Empty,
                Email = entity.Email,
                IsActive = entity.IsActive,
                IsEditMode = true
            };

            ViewBag.RoleSelectList = BuildRoleSelectList();
            return View("AddEditSubUser", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSubUser(SubUserViewModel model, bool saveAndExit = true)
        {
            var agencyId = GetAgencyIdOrDefault();
            if (agencyId <= 0)
            {
                SetTempDataForNoRecord();
                return RedirectToAction(nameof(SubUsersList));
            }

            if (model.IsEditMode)
            {
                // Password is optional on edit only when left blank.
                // If user enters a new password, keep ModelState keys so validation runs.
                if (string.IsNullOrWhiteSpace(model.Password) && string.IsNullOrWhiteSpace(model.ConfirmPassword))
                {
                    ModelState.Remove(nameof(model.Password));
                    ModelState.Remove(nameof(model.ConfirmPassword));
                }
            }

            if (model.Role < (int)Roles.Agent)
                ModelState.AddModelError(nameof(SubUserViewModel.Role), "Invalid role selection.");

            if (!ModelState.IsValid)
            {
                ViewBag.RoleSelectList = BuildRoleSelectList();
                return View("AddEditSubUser", model);
            }

            var dto = new SubUserUpsertDto
            {
                UserId = model.UserId,
                UserName = model.UserName,
                Role = model.Role,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                IsActive = model.IsActive
            };

            if (model.IsEditMode)
            {
                var (success, message) = await _subUsersService.UpdateSubUserAsync(dto, agencyId);
                TempData["RowsAffected"] = success ? 1 : 0;
                TempData["Message"] = success ? Constants.SuccessMessages.MsgUpdateSuccess : message;
            }
            else
            {
                var (success, message, _) = await _subUsersService.CreateSubUserAsync(dto, agencyId);
                TempData["RowsAffected"] = success ? 1 : 0;
                TempData["Message"] = success ? Constants.SuccessMessages.MsgInsertSuccess : message;
            }

            if (saveAndExit)
                return RedirectToAction(nameof(SubUsersList));

            if (model.IsEditMode)
                return RedirectToAction(nameof(AddEditSubUser), new { id = model.UserId });

            return RedirectToAction(nameof(AddEditSubUser));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var agencyId = GetAgencyIdOrDefault();
            if (agencyId <= 0 || id <= 0)
                return new JsonResult(false);

            var ok = await _subUsersService.SoftDeleteSubUserAsync(id, agencyId);
            return new JsonResult(ok);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActive(int id, bool isActive)
        {
            var agencyId = GetAgencyIdOrDefault();
            if (agencyId <= 0 || id <= 0)
                return Json(new { success = false });

            var ok = await _subUsersService.SetSubUserActiveAsync(id, agencyId, isActive);
            return Json(new { success = ok });
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsUserNameAvailable(string? userName, int userId = 0)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return Json(true);

            var original = HttpContext.Session.GetString(SessionOriginalUsername) ?? string.Empty;
            var trimmed = userName.Trim();

            if (userId > 0 && string.Equals(original, trimmed, StringComparison.Ordinal))
                return Json(true);

            var exists = await _subUsersService.IsUsernameExistsAsync(trimmed, userId > 0 ? userId : null);
            return Json(exists ? "This username is already in use." : (object)true);
        }

        private int GetAgencyIdOrDefault()
        {
            var claim = User.FindFirstValue("AgencyId");
            return int.TryParse(claim, out var id) ? id : 0;
        }

        private static List<SelectListItem> BuildRoleSelectList()
        {
            var roles = Enum.GetValues(typeof(Roles))
                .Cast<Roles>()
                .Select(r => new { Value = (int)r, Name = r.ToString() })
                .Where(x => x.Value >= (int)Roles.Agent)
                .OrderBy(x => x.Value)
                .ToList();

            return roles.Select(x => new SelectListItem
            {
                Value = x.Value.ToString(),
                Text = x.Name
            }).ToList();
        }

        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        }
    }
}

