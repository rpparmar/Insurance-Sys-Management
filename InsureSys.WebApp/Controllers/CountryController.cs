using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using InsuranceSys.Application;
using InsuranceSys.Domain;
using InsuranceSys.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin,AgencyAdmin")]
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public CountryController(ICountryService countryService, IMapper mapper)
        {
            _countryService = countryService;
            _mapper = mapper;
        }

        [Route("Countries")]
        public IActionResult CountryList()
        {
            return View("../Masters/Country/CountryList");
        }

        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await DataTableHelper.BuildGridResponseAsync(Request, _countryService.GetAllAsync);
            return Json(result);
        }

        [HttpGet("Countries/Add")]
        [HttpGet("Countries/Edit/{id}")]
        public async Task<IActionResult> AddEditCountry(string id = "")
        {
            CountryViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int countryId) && countryId > 0)
                {
                    var dto = await _countryService.GetByIdAsync(countryId);
                    if (dto != null)
                    {
                        model = _mapper.Map<CountryViewModel>(dto);
                        model.IsEditMode = true;
                        HttpContext.Session.SetString("Original_CountryName", model.CountryName);
                        HttpContext.Session.SetString("Original_CountryCode", model.CountryCode);
                    }
                    else
                    {
                        SetTempDataForNoRecord();
                        return RedirectToAction(nameof(CountryList));
                    }
                }
                else
                {
                    SetTempDataForNoRecord();
                    return RedirectToAction(nameof(CountryList));
                }
            }
            else
            {
                model.IsEditMode = false;
                model.IsActive = true;
                HttpContext.Session.SetString("Original_CountryName", string.Empty);
                HttpContext.Session.SetString("Original_CountryCode", string.Empty);
            }

            return View("../Masters/Country/AddEditCountry", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCountry(CountryViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
                return View("../Masters/Country/AddEditCountry", model);
            }

            var entity = _mapper.Map<CountryEntity>(model);
            int rowsAffected;
            if (model.IsEditMode)
            {
                rowsAffected = await _countryService.UpdateAsync(entity);
                TempData["Message"] = rowsAffected > 0 ? Constants.SuccessMessages.MsgUpdateSuccess : Constants.ErrorMessages.MsgUpdateFailure;
            }
            else
            {
                rowsAffected = await _countryService.AddAsync(entity);
                TempData["Message"] = rowsAffected > 0 ? Constants.SuccessMessages.MsgInsertSuccess : Constants.ErrorMessages.MsgInsertFailure;
            }

            TempData["RowsAffected"] = rowsAffected;
            if (saveAndExit)
            {
                return RedirectToAction(nameof(CountryList));
            }

            return model.IsEditMode
                ? RedirectToAction(nameof(AddEditCountry), new RouteValueDictionary(new { id = model.CountryID }))
                : RedirectToAction(nameof(AddEditCountry));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            bool status = Convert.ToBoolean(await _countryService.DeleteAsync(id));
            return new JsonResult(status);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsCountryNameExist(string CountryName = "")
        {
            string original = HttpContext.Session.GetString("Original_CountryName") ?? string.Empty;
            bool isEditMode = !string.IsNullOrEmpty(original);
            bool isExist = await _countryService.FindByNameAsync(CountryName);

            if (isEditMode && !string.Equals(original, CountryName, StringComparison.OrdinalIgnoreCase) && isExist)
                return Json($"Country name '{CountryName}' is already in use.");
            if (!isEditMode && isExist)
                return Json($"Country name '{CountryName}' is already in use.");
            return Json(true);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsCountryCodeExist(string CountryCode = "")
        {
            string original = HttpContext.Session.GetString("Original_CountryCode") ?? string.Empty;
            bool isEditMode = !string.IsNullOrEmpty(original);
            bool isExist = await _countryService.FindByCodeAsync(CountryCode);

            if (isEditMode && !string.Equals(original, CountryCode, StringComparison.OrdinalIgnoreCase) && isExist)
                return Json($"Country code '{CountryCode}' is already in use.");
            if (!isEditMode && isExist)
                return Json($"Country code '{CountryCode}' is already in use.");
            return Json(true);
        }

        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        }
    }
}
