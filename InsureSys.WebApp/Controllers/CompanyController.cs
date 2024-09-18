using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Application;
using InsuranceSys.Domain.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
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

        public async Task<IActionResult> CompanyListInnerContent(string searchtxt = "", bool status = true, int page = 1, int pagesize = 10)
        {
            var @params = ImmutableDictionary<string, object>.Empty
            .Add("page", page)
            .Add("pagesize", pagesize)
            .Add("searchval", searchtxt)
            .Add("status", status)
            .Add("Count", 0);

            StringBuilder strHTML = new StringBuilder();
            using (DataSet ds = await _companyService.GetAllCompanies(@params))
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    int RowsCount = 1;
                    int.TryParse(Convert.ToString(ds.Tables[0].Rows[0]["RowsCount"]), out RowsCount);

                    TempData["totalrecords"] = RowsCount;
                    TempData["paging_size"] = pagesize;

                    using (DataTable dtContent = ds.Tables[1])
                    {
                        if (dtContent != null && dtContent.Rows.Count > 0)
                        {
                            strHTML.Append(@"
                                <table class='datatable-bordered datatable-head-custom datatable-table' id='kt_datatable'>
                                    <thead class='datatable-head'>
                                        <tr class='datatable-row'>
                                            <th class='datatable-cell'>Company Name</th>
                                            <th class='datatable-cell'>Active/InActive</th>
                                            <th class='datatable-cell'>Action</th>
                                        </tr>
                                    </thead>
                                    <tbody class='datatable-body custom-scroll'>");
                            foreach (DataRow row in dtContent.Rows)
                            {
                                int companyId = Convert.ToInt16(row["CompanyID"]);
                                string companyName = Convert.ToString(row["CompanyName"]) ?? string.Empty;
                                bool isActive = Convert.ToBoolean(row["IsActive"]);

                                string deleteConfirmationEvent = $"DeleteConfirmation('{companyId}', 'Company', 'Company')";
                                string statusChangeEvent = $"StatusChangeConfirmation('{companyId}')";
                                strHTML.Append($@"
                                            <tr>
                                                <td>{companyName}</td>
                                                <td>
                                                    <span class='switch switch-icon'>
                                                        <label>
                                                            <input onclick=""{statusChangeEvent}"" type='checkbox' id='chkstatus_{companyId}' {(isActive ? "checked" : "")}>
                                                            <span></span>
                                                        </label>
                                                    </span>
                                                </td>
                                                <td>
                                                    <a class='btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3' href='/Companies/Edit/{companyId}'><i class='flaticon-edit'></i></a>
                                                    <a id='del_{companyId}' class='btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger' onclick=""{deleteConfirmationEvent}""><i class='flaticon-delete'></i></a>
                                                </td>
                                            </tr>");
                            }
                            strHTML.Append("</tbody></table>");
                        }
                        else
                            strHTML.Append("<center>No records found</center>");
                    }
                }
            }
            return Content(strHTML.ToString());
        }

        [HttpGet("Companies/Add/{id?}")]
        [HttpGet("Companies/Edit/{id?}")]
        public async Task<IActionResult> AddEditCompany(string id = "")
        {
            CompanyViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int _id) && _id > 0)
                {
                    var companydto = await _companyService.GetCompanyById(Convert.ToInt16(_id));
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
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                foreach (var item in errors)
                {
                    ModelState.AddModelError(item.Key, item.Errors.Select(s => s.ErrorMessage).ToString());
                }
                return View("../Masters/Company/AddEditCompany", model);
            }
            var company = _mapper.Map<CompanyDto>(model);

            if (model.IsEditMode)
            {
                #region Update
                var rowsaffected = await _companyService.UpdateCompany(company);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = "Record updated successfully."; // set by generic way
                else
                    TempData["Message"] = "Record not updated,something went wrong"; // set by generic way
                #endregion
            }
            else
            {
                #region Insert
                var rowsaffected = await _companyService.AddCompany(company);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = "Record saved successfully."; // set by generic way
                else
                    TempData["Message"] = "Record not saved,something went wrong"; // set by generic way
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
        public ActionResult CompanyCount()
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

        [HttpGet]
        public async Task<JsonResult> UpdateStatus(int id, bool status)
        {
            return new JsonResult(Convert.ToBoolean(await _companyService.UpdateStatus(id, status)));
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsCompanyExist(string CompanyName = "")
        {
            string Original_CompanyName = HttpContext.Session.GetString("Original_CompanyName") ?? "";
            bool IsEditMode = !string.IsNullOrEmpty(Original_CompanyName);
            int.TryParse(await _companyService.FindByName(CompanyName), out int matchCount);
            if (IsEditMode && !string.Equals(Original_CompanyName, CompanyName) && matchCount > 0)
                return Json($"Company name '{CompanyName}' is already in use.");
            else if (!IsEditMode && matchCount > 0)
                return Json($"Company name '{CompanyName}' is already in use.");
            return Json(true);
        }
        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = "No such record exists"; // set by generic way
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool status = Convert.ToBoolean(await _companyService.DeleteCompany(id));
            return new JsonResult(status);
        }
    }
}
