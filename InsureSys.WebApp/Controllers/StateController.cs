using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using InsuranceSys.Application;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin,AgencyAdmin")]
    public class StateController : Controller
    {
        private readonly IStateService _stateService;
        private readonly IDropDownBinderService _dropDownBinderService;
        private readonly IMapper _mapper;

        public StateController(IStateService stateService, IDropDownBinderService dropDownBinderService, IMapper mapper)
        {
            _stateService = stateService;
            _dropDownBinderService = dropDownBinderService;
            _mapper = mapper;
        }

        [Route("States")]
        public IActionResult StateList()
        {
            return View("../Masters/State/StateList");
        }

        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await DataTableHelper.BuildGridResponseAsync(Request, _stateService.GetAllAsync);
            return Json(result);
        }

        [HttpGet("States/Add")]
        [HttpGet("States/Edit/{id}")]
        public async Task<IActionResult> AddEditState(string id = "")
        {
            StateViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int stateId) && stateId > 0)
                {
                    var dto = await _stateService.GetByIdAsync(stateId);
                    if (dto != null)
                    {
                        model = _mapper.Map<StateViewModel>(dto);
                        model.IsEditMode = true;
                        HttpContext.Session.SetString("Original_StateName", model.StateName);
                        HttpContext.Session.SetString("Original_StateCode", model.StateCode);
                    }
                    else
                    {
                        SetTempDataForNoRecord();
                        return RedirectToAction(nameof(StateList));
                    }
                }
                else
                {
                    SetTempDataForNoRecord();
                    return RedirectToAction(nameof(StateList));
                }
            }
            else
            {
                model.IsEditMode = false;
                model.IsActive = true;
                HttpContext.Session.SetString("Original_StateName", string.Empty);
                HttpContext.Session.SetString("Original_StateCode", string.Empty);
            }

            model.CountrySelectList = await GetCountryListAsync(model.CountryID);
            return View("../Masters/State/AddEditState", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveState(StateViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
                model.CountrySelectList = await GetCountryListAsync(model.CountryID);
                return View("../Masters/State/AddEditState", model);
            }

            var entity = _mapper.Map<StateEntity>(model);
            int rowsAffected;
            if (model.IsEditMode)
            {
                rowsAffected = await _stateService.UpdateAsync(entity);
                TempData["Message"] = rowsAffected > 0 ? Constants.SuccessMessages.MsgUpdateSuccess : Constants.ErrorMessages.MsgUpdateFailure;
            }
            else
            {
                rowsAffected = await _stateService.AddAsync(entity);
                TempData["Message"] = rowsAffected > 0 ? Constants.SuccessMessages.MsgInsertSuccess : Constants.ErrorMessages.MsgInsertFailure;
            }

            TempData["RowsAffected"] = rowsAffected;
            if (saveAndExit)
            {
                return RedirectToAction(nameof(StateList));
            }

            return model.IsEditMode
                ? RedirectToAction(nameof(AddEditState), new RouteValueDictionary(new { id = model.StateID }))
                : RedirectToAction(nameof(AddEditState));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            bool status = Convert.ToBoolean(await _stateService.DeleteAsync(id));
            return new JsonResult(status);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsStateNameExist(int CountryID, string StateName = "")
        {
            string original = HttpContext.Session.GetString("Original_StateName") ?? string.Empty;
            bool isEditMode = !string.IsNullOrEmpty(original);
            bool isExist = await _stateService.FindByNameAsync(CountryID, StateName);

            if (isEditMode && !string.Equals(original, StateName, StringComparison.OrdinalIgnoreCase) && isExist)
                return Json($"State name '{StateName}' is already in use for selected country.");
            if (!isEditMode && isExist)
                return Json($"State name '{StateName}' is already in use for selected country.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsStateCodeExist(int CountryID, string StateCode = "")
        {
            string original = HttpContext.Session.GetString("Original_StateCode") ?? string.Empty;
            bool isEditMode = !string.IsNullOrEmpty(original);
            bool isExist = await _stateService.FindByCodeAsync(CountryID, StateCode);

            if (isEditMode && !string.Equals(original, StateCode, StringComparison.OrdinalIgnoreCase) && isExist)
                return Json($"State code '{StateCode}' is already in use for selected country.");
            if (!isEditMode && isExist)
                return Json($"State code '{StateCode}' is already in use for selected country.");
            return Json(true);
        }

        private async Task<List<SelectListItem>> GetCountryListAsync(int selectedCountryId = 0)
        {
            List<DropdownItemDto> countries = await _dropDownBinderService.GetCountryDropdownAsync();
            return countries.Select(x => new SelectListItem
            {
                Value = x.Value,
                Text = x.Text,
                Selected = x.Value == selectedCountryId.ToString()
            }).ToList();
        }

        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        }
    }
}
