using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.DTO;
using InsuranceSys.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Insurancesys.web.Controllers
{
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
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
        public async Task<IActionResult> CountryListInnerContent(string searchtxt = "", bool status = true, int page = 1, int pagesize = 10)
        {
            var @params = ImmutableDictionary<string, object>.Empty
            .Add("page", page)
            .Add("pagesize", pagesize)
            .Add("searchval", searchtxt)
            .Add("status", status);

            StringBuilder strHTML = new StringBuilder();
            var (countries, totalCount) = await _countryService.GetAllCountries(@params);
            if (countries.Any())
            {
                int RowsCount = totalCount;
                TempData["totalrecords"] = RowsCount;
                TempData["paging_size"] = pagesize;

                strHTML.Append(@"
                <table class='datatable-bordered datatable-head-custom datatable-table' id='kt_datatable'>
                     <thead class='datatable-head'>
                         <tr class='datatable-row'>
                             <th class='datatable-cell'>Country Name</th>
                             <th class='datatable-cell'>Active/InActive</th>
                             <th class='datatable-cell'>Action</th>
                         </tr>
                     </thead>
                     <tbody class='datatable-body custom-scroll'>");
                foreach (var country in countries)
                {
                    string deleteConfirmationEvent = $"DeleteConfirmation('{country.CountryID}', 'Country', 'Country')";
                    string statusChangeEvent = $"StatusChangeConfirmation('{country.CountryID}')";
                    strHTML.Append($@"
                             <tr>
                                 <td>{country.CountryName}</td>
                                 <td>
                                     <span class='switch switch-icon'>
                                         <label>
                                             <input onclick=""{statusChangeEvent}"" type='checkbox' id='chkstatus_{country.CountryID}' {(country.IsActive ? "checked" : "")}>
                                             <span></span>
                                         </label>
                                     </span>
                                 </td>
                                 <td>
                                     <a class='btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3' href='/Countries/Edit/{country.CountryID}'><i class='flaticon-edit'></i></a>
                                     <a id='del_{country.CountryID}' class='btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger' onclick=""{deleteConfirmationEvent}""><i class='flaticon-delete'></i></a>
                                 </td>
                             </tr>");
                }
                strHTML.Append("</tbody></table>");
            }
            else
                strHTML.Append("<center>No records found</center>");

            return Content(strHTML.ToString());
        }

        [HttpGet("Countries/Edit/{id}")]
        [HttpGet("Countries/Add")]
        public async Task<IActionResult> AddEditCountry(string id = "")
        {
            CountryViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int _id) && _id > 0)
                {
                    var countrydto = await _countryService.GetCountryByIdAsync(Convert.ToInt16(_id));
                    if (countrydto != null)
                    {
                        model = _mapper.Map<CountryViewModel>(countrydto);
                        HttpContext.Session.SetString("Original_CountryName", model.CountryName);
                        model.IsEditMode = true;
                    }
                    else
                    {
                        SetTempDataForNoRecord();
                        HttpContext.Session.SetString("Original_CountryName", "");
                        return RedirectToAction("CountryList");
                    }
                }
                else
                {
                    SetTempDataForNoRecord();
                    HttpContext.Session.SetString("Original_CountryName", "");
                    return RedirectToAction("CountryList");
                }
            }
            else
            {
                model.IsEditMode = false;
                model.IsActive = true;
                HttpContext.Session.SetString("Original_CountryName", "");
            }
            return View("../Masters/Country/AddEditCountry", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCountry(CountryViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                foreach (var item in errors)
                {
                    ModelState.AddModelError(item.Key, item.Errors.Select(s => s.ErrorMessage).ToString());
                }
                return View("../Masters/Country/AddEditCountry", model);
            }
            var country = _mapper.Map<CountryDto>(model);

            if (model.IsEditMode)
            {
                #region Update
                int rowsaffected = await _countryService.UpdateCountry(country);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = Constants.SuccessMessages.MsgUpdateSuccess;
                else
                    TempData["Message"] = Constants.ErrorMessages.MsgUpdateFailure;
                #endregion
            }
            else
            {
                #region Insert
                int rowsaffected = await _countryService.AddCountry(country);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = Constants.SuccessMessages.MsgInsertSuccess;
                else
                    TempData["Message"] = Constants.ErrorMessages.MsgInsertFailure;
                #endregion
            }
            if (saveAndExit)
            {
                return RedirectToAction("CountryList");
            }
            else if (model.IsEditMode)
            {
                return RedirectToAction("AddEditCountry", new RouteValueDictionary(new { id = model.CountryID }));
            }
            else
            {
                return RedirectToAction("AddEditCountry");
            }
        }
        [HttpGet]
        public async Task<JsonResult> UpdateStatus(int id, bool status)
        {
            return new JsonResult(Convert.ToBoolean(await _countryService.UpdateStatus(id, status)));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool status = Convert.ToBoolean(await _countryService.DeleteCountry(id));
            return new JsonResult(status);
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsCountryExist(string CountryName = "")
        {
            string Original_CountryName = HttpContext.Session.GetString("Original_CountryName") ?? "";
            bool IsEditMode = !string.IsNullOrEmpty(Original_CountryName);
            int matchCount = await _countryService.GetCountryByName(CountryName);
            if (IsEditMode && !string.Equals(Original_CountryName, CountryName) && matchCount > 0)
                return Json($"Country name '{CountryName}' is already in use.");
            else if (!IsEditMode && matchCount > 0)
                return Json($"Country name '{CountryName}' is already in use.");
            return Json(true);
        }
        public ActionResult CountryCount()
        {
            string result = string.Empty;
            int totals = Convert.ToInt32(TempData["totalrecords"]);

            int pagesize = Convert.ToInt32(TempData["paging_size"]);
            ViewData["paging_size"] = pagesize;

            int noofpages = 1;
            if (totals > 0 && pagesize > 0)
                noofpages = (totals / pagesize) + (totals % pagesize != 0 ? 1 : 0);
            result = "{\"noofpages\":" + noofpages + ",\"NoOfTotalRecords\":" + totals + "}";

            return Content((result));
        }
        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        }
    }
}
