using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Application;
using InsuranceSys.Domain.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics.Metrics;
using System.Text;

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
            ViewData["paging_size"] = 10;
            return View("../Masters/Company/CompanyList");
        }

        public async Task<IActionResult> CompanyListInnerContent(string searchtxt = "", bool status = true, int page = 1, int pagesize = 10)
        {
            Dictionary<string, object> paramCollections = new Dictionary<string, object>();
            paramCollections.Add("page", page);
            paramCollections.Add("pagesize", pagesize);
            paramCollections.Add("searchval", searchtxt);
            paramCollections.Add("status", status);
            paramCollections.Add("Count", 0);

            StringBuilder strHTML = new StringBuilder();
            using (DataSet ds = await _companyService.GetAllCompanies(paramCollections))
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
                            strHTML.Append("</tr>");
                            strHTML.Append("</thead>");
                            strHTML.Append("<tbody class='datatable-body'>");
                            for (int i = 0; i < dtContent.Rows.Count; i++)
                            {
                                strHTML.Append("<tr>");
                                strHTML.Append("<td>" + Convert.ToString(dtContent.Rows[i]["CompanyName"]) + "</td>");
                                strHTML.Append("</tr>");
                            }
                            strHTML.Append("</tbody>");
                            strHTML.Append("</table>");
                        }
                        else
                        {
                            strHTML.Append("<center>No data available in table</center>");
                        }
                    }

                }
            }
            return Content(strHTML.ToString());
        }

        [HttpGet("companies/Add/{id?}")]
        [HttpGet("companies/Edit/{id?}")]
        public async Task<IActionResult> AddEditCompany(int id = 0)
        {
            CompanyViewModel model = new CompanyViewModel();
            if (id > 0)
            {
                var companydto = await _companyService.GetCompanyById(id);
                if (companydto != null)
                {
                    model = _mapper.Map<CompanyViewModel>(companydto);
                    model.IsEditMode = true;
                }
                else
                {
                    TempData["RowsAffected"] = 0;
                    TempData["Message"] = "No such record exist";
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
        public async Task<IActionResult> SaveCompany(CompanyViewModel model, string saveAndExit = "")
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
                    TempData["Message"] = "Record updated successfully.";
                else
                    TempData["Message"] = "Record not updated,something went wrong";
                #endregion
            }
            else
            {
                #region Insert
                var rowsaffected = await _companyService.AddCompany(company);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = "Record saved successfully.";
                else
                    TempData["Message"] = "Record not saved,something went wrong";
                #endregion
            }
            if (!string.IsNullOrEmpty(saveAndExit))
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
    }
}
