using AutoMapper;
using Insurancesys.web.Models;
using Insurancesys.web.Models.Common;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        public CompanyController(ICompanyService companyService, IMapper mapper)
        {
            _companyService = companyService;
            _mapper = mapper;
        }
        [Route("Companies")]
        public IActionResult CompanyList()
        {
            return View("../Masters/Company/CompanyList");
        }

        [HttpPost]
        public async Task<IActionResult> GetData(DataTableRequest param)
        {
            int page = param.iDisplayStart;
            int pagesize = param.iDisplayLength;

            #region sorting

            StringValues sortDirValues = Request.Form["sSortDir_0"];
            string sortDirection = sortDirValues.Count > 0 ? sortDirValues[0].ToLower() : "desc";

            string sortingField = Request.Form["SortingField"];
            string SortExp = !string.IsNullOrWhiteSpace(sortingField) ? sortingField : "1";
            SortExp += (sortDirection == "asc") ? " asc" : " desc";
            #endregion
            string searchText = Request.Form["searchText"];
            string searchTerm = !string.IsNullOrWhiteSpace(searchText) ? searchText : "";

            var @params = ImmutableDictionary<string, object>.Empty
            .Add("@PageNumber", page)
            .Add("@PageSize", pagesize)
            .Add("@SearchTerm", searchTerm)
            .Add("@SortExp", SortExp);

            using (DataSet ds = await _companyService.GetAllAsync(@params))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    int totalRecords = 1;
                    int.TryParse(Convert.ToString(ds.Tables[0].Rows[0]["TotalRecords"]), out totalRecords);
                    using (DataTable dtContent = ds.Tables[1])
                    {
                        if (dtContent != null && dtContent.Rows.Count > 0)
                        {
                            var response = dtContent.AsEnumerable()
                            .Select(row => dtContent.Columns.Cast<DataColumn>()
                            .ToDictionary(col => col.ColumnName, col => row[col])).ToList();

                            return Json(new
                            {
                                iTotalRecords = totalRecords,
                                iTotalDisplayRecords = totalRecords,
                                data = response
                            });
                        }
                        else
                        {
                            // Handle the case when no rows are returned
                            return Json(new
                            {
                                iTotalRecords = 0,
                                iTotalDisplayRecords = 0,
                                data = new List<object>() // Empty list for no data
                            });
                        }
                    }
                }
                else
                {
                    // Handle the case when no rows are returned
                    return Json(new
                    {
                        iTotalRecords = 0,
                        iTotalDisplayRecords = 0,
                        data = new List<object>() // Empty list for no data
                    });
                }
            }

        }

        [HttpGet("Companies/Add")]
        [HttpGet("Companies/Edit/{id}")]
        public async Task<IActionResult> AddEditCompany(string id = "")
        {
            CompanyViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int _id) && _id > 0)
                {
                    var companydto = await _companyService.GetByIdAsync(Convert.ToInt16(_id));
                    if (companydto != null)
                    {
                        model = _mapper.Map<CompanyViewModel>(companydto);
                        HttpContext.Session.SetString("Original_CompanyName", model.CompanyName);
                        model.IsEditMode = true;
                    }
                    else
                    {
                        SetTempDataForNoRecord();
                        HttpContext.Session.SetString("Original_CompanyName", "");
                        return RedirectToAction("CompanyList");
                    }
                }
                else
                {
                    SetTempDataForNoRecord();
                    HttpContext.Session.SetString("Original_CompanyName", "");
                    return RedirectToAction("CompanyList");
                }
            }
            else
            {
                model.IsEditMode = false;
                model.IsActive = true;
                HttpContext.Session.SetString("Original_CompanyName", "");
            }
            return View("../Masters/Company/AddEditCompany", model);
        }
        [HttpPost]
        public async Task<IActionResult> SaveCompany(CompanyViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value?.Errors.Count > 0).Select(x => new { x.Key, x.Value?.Errors }).ToArray();
                foreach (var item in errors)
                {
                    ModelState.AddModelError(item.Key, item.Errors.Select(s => s.ErrorMessage).ToString());
                }
                return View("../Masters/Company/AddEditCompany", model);
            }
            var company = _mapper.Map<CompanyEntity>(model);

            if (model.IsEditMode)
            {
                #region Update
                var rowsaffected = await _companyService.UpdateAsync(company);
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
                var rowsaffected = await _companyService.AddAsync(company);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = Constants.SuccessMessages.MsgInsertSuccess;
                else
                    TempData["Message"] = Constants.ErrorMessages.MsgInsertFailure;
                #endregion
            }
            if (saveAndExit)
            {
                return RedirectToAction("CompanyList");
            }
            else if (model.IsEditMode)
            {
                return RedirectToAction("AddEditCompany", new RouteValueDictionary(new { id = model.CompanyID }));
            }
            else
            {
                return RedirectToAction("AddEditCompany");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            bool status = Convert.ToBoolean(await _companyService.DeleteAsync(id));
            return new JsonResult(status);
        }

        //[HttpGet]
        //public async Task<JsonResult> UpdateStatus(int id, bool status)
        //{
        //    return new JsonResult(Convert.ToBoolean(await _companyService.UpdateStatusAsync(id, status)));
        //}

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsCompanyExist(string CompanyName = "")
        {
            string Original_CompanyName = HttpContext.Session.GetString("Original_CompanyName") ?? "";
            bool IsEditMode = !string.IsNullOrEmpty(Original_CompanyName);
            bool IsExist=await _companyService.FindByNameAsync(CompanyName);
            if (IsEditMode && !string.Equals(Original_CompanyName, CompanyName) && IsExist)
                return Json($"Company name '{CompanyName}' is already in use.");
            else if (!IsEditMode && IsExist)
                return Json($"Company name '{CompanyName}' is already in use.");
            return Json(true);
        }

        #region Helper methods
        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        } 
        #endregion

    }
}
