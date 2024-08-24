using Insurancesys.web.Models;
using InsuranceSys.Application;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace Insurancesys.web.Controllers
{
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }
        [Route("companies")]
        public IActionResult CompanyList()
        {
            ViewData["paging_size"] = 10;
            return View("../Masters/Company/CompanyList");
        }

        public async Task<IActionResult> CompanyListInnerContent(int page = 1, int pagesize = 10, string searchval = "")
        {
            Dictionary<string, object> paramCollections = new Dictionary<string, object>();
            paramCollections.Add("page", page);
            paramCollections.Add("pagesize", pagesize);
            paramCollections.Add("searchval", searchval);
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
                                strHTML.Append("<td>" + Convert.ToString(dtContent.Rows[i]["nvarCompanyName"]) + "</td>");
                                strHTML.Append("</tr>");
                            }
                            strHTML.Append("</tbody>");
                            strHTML.Append("</table>");
                        }
                    }

                }
            }
            return Content(strHTML.ToString());
        }

        [HttpGet("companies/Add/{id?}")]
        [HttpGet("companies/Edit/{id?}")]
        public IActionResult AddEditCompany(int id = 0)
        {
            CompanyViewModel model = new CompanyViewModel();
            return View("../Masters/Company/AddEditCompany", model);
        }
        [HttpPost]
        public IActionResult AddEditCompany(CompanyViewModel model, string saveAndExit = "")
        {
            return View("../Masters/Company/AddEditCompany", model);
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
