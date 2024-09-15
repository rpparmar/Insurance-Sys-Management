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
        [Route("companies")]
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
                            strHTML.Append("<table class='datatable-bordered datatable-head-custom datatable-table' id='kt_datatable'>");
                            strHTML.Append("<thead class='datatable-head'>");
                            strHTML.Append("<tr class='datatable-row'>");
                            strHTML.Append("<th class='datatable-cell'>Company Name</th>");
                            strHTML.Append("<th class='datatable-cell'>Status</th>");
                            strHTML.Append("<th class='datatable-cell'>Action</th>");
                            strHTML.Append("</tr>");
                            strHTML.Append("</thead>");
                            strHTML.Append("<tbody class='datatable-body'>");
                            for (int i = 0; i < dtContent.Rows.Count; i++)
                            {
                                int CompanyID = Convert.ToInt16(dtContent.Rows[i]["CompanyID"]);
                                string DeleteConfirmationEvent = "DeleteConfirmation('" + CompanyID + "','User','BackOffice','DeleteUser')";
                                string StatusChangeConfirmationEvent = "StatusChangeConfirmation('" + CompanyID + "')";

                                strHTML.Append("<tr>");
                                strHTML.Append("<td>" + Convert.ToString(dtContent.Rows[i]["CompanyName"]) + "</td>");
                                strHTML.Append("<td>");
                                if (Convert.ToBoolean(dtContent.Rows[i]["IsActive"]))
                                {
                                    strHTML.Append("<span class='switch switch-icon'><label><input onclick=" + StatusChangeConfirmationEvent + " type='checkbox' id='chkstatus_" + CompanyID + "' checked><span></span></label></span>");
                                }
                                else
                                {
                                    strHTML.Append("<span class='switch switch-icon'><label><input onclick=" + StatusChangeConfirmationEvent + " type='checkbox' id='chkstatus_" + CompanyID + "'><span></span></label></span>");
                                }
                                strHTML.Append("</td>");
                                strHTML.Append("<td>");
                                strHTML.Append("<a class='btn btn-sm btn-icon btn-lg-light btn-text-primary btn-hover-light-primary mr-3' href= '/companies/Edit/" + CompanyID + "'><i class='flaticon-edit'></i></a>");
                                //if (item.Product_In_Key > 0 || item.Product_In_license > 0)
                                //    strHTML.Append("<a class='btn btn-sm btn-icon' style='cursor: auto;'></a>");
                                //else
                                //strHTML.Append("<a id = 'del_" + item.int_glcode + "' class='btn btn-sm btn-icon btn-lg-light btn-text-danger btn-hover-light-danger' onclick=" + DeleteConfirmationEvent + "><i class='flaticon-delete'></i></a>");
                                strHTML.Append("</td>");
                                strHTML.Append("</tr>");
                            }
                            strHTML.Append("</tbody>");
                            strHTML.Append("</table>");
                        }
                        else
                        {
                            strHTML.Append("<center>No records found</center>");
                        }
                    }

                }
            }
            return Content(strHTML.ToString());
        }

        [HttpGet("companies/Add/{id?}")]
        [HttpGet("companies/Edit/{id?}")]
        public async Task<IActionResult> AddEditCompany(string id = "")
        {
            CompanyViewModel model = new CompanyViewModel();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int _id) && _id > 0)
                {
                    var companydto = await _companyService.GetCompanyById(Convert.ToInt16(_id));
                    if (companydto != null)
                    {
                        model = _mapper.Map<CompanyViewModel>(companydto);
                        model.IsEditMode = true;
                    }
                    else
                    {
                        SetTempDataForNoRecord();
                        return RedirectToAction("CompanyList");
                    }
                }
                else
                {
                    SetTempDataForNoRecord();
                    return RedirectToAction("CompanyList");
                }
            }
            else
            {
                model.IsEditMode = false;
                model.IsActive = true;
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
        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = "No such record exists"; // set by generic way
        }
    }
}
