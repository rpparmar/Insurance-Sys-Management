using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using Insurancesys.web.Models.Common;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
    public class LeadController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IMapper _mapper;
        public LeadController(
             ILeadService leadService,
             IMapper mapper)
        {
            _leadService = leadService;
            _mapper = mapper;
        }
        [Route("Leads")]
        public IActionResult ListOfLeads()
        {            
            return View("../Customer/ListOfLeads");
        }

        [HttpPost]
        public async Task<IActionResult> GetData(DataTableRequest param)
        {
            int page = param.iDisplayStart;
            int pagesize = param.iDisplayLength;

            string sortDirection = CommonHelper.SearchSortValue(Request.Form, "sSortDir_0", "desc").ToLowerInvariant();
            string sortField = CommonHelper.SearchSortValue(Request.Form, "SortingField", "1");
            string SortExp = $"{sortField} {(sortDirection == "asc" ? "asc" : "desc")}";
            string searchTerm = CommonHelper.SearchSortValue(Request.Form, "searchText");

            var @params = ImmutableDictionary<string, object>.Empty
            .Add("@PageNumber", page)
            .Add("@PageSize", pagesize)
            .Add("@SearchTerm", searchTerm)
            .Add("@SortExp", SortExp);

            using (DataSet ds = await _leadService.GetAllAsync(@params))
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
                                .ToDictionary(
                                    col => col.ColumnName,
                                    col => CommonHelper.FormatCellValue(row[col]) // format logic for null/blank
                                )
                            ).ToList();
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

        [HttpGet("Leads/Add")]
        [HttpGet("Leads/Edit/{id}")]
        public async Task<ActionResult> AddEditLeads(string id = "")
        {
            LeadViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int _id) && _id > 0)
                {
                    var leaddto = await _leadService.GetByIdAsync(Convert.ToInt16(_id));
                    if (leaddto != null)
                    {
                        model = _mapper.Map<LeadViewModel>(leaddto);
                        //HttpContext.Session.SetString("Original_CountryName", model.CountryName);
                        model.IsEditMode = true;
                    }
                    else
                    {
                        //SetTempDataForNoRecord();
                        //HttpContext.Session.SetString("Original_CountryName", "");
                        return RedirectToAction("ListOfLeads");
                    }
                }
                else
                {
                    //SetTempDataForNoRecord();
                    //HttpContext.Session.SetString("Original_CountryName", "");
                    return RedirectToAction("ListOfLeads");
                }
            }
            else
            {
                model.IsEditMode = false;
                model.IsActive = true;
                //HttpContext.Session.SetString("Original_CountryName", "");
            }
            model = BindDropdowns(model);
            return View("../Customer/AddEditLeads", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLead(LeadViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
                model = BindDropdowns(model);
                // Extract all model state errors
                var errors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .SelectMany(x => x.Value!.Errors
                        .Select(error => new { Key = x.Key, ErrorMessage = error.ErrorMessage }))
                    .ToList();
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Key, error.ErrorMessage);
                }
                return View("../Customer/AddEditLeads", model);
            }
            var lead = _mapper.Map<LeadEntity>(model);
            if (model.IsEditMode)
            {
                #region Update
                int rowsaffected = await _leadService.UpdateAsync(lead);
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
                int rowsaffected = await _leadService.AddAsync(lead);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = Constants.SuccessMessages.MsgInsertSuccess;
                else
                    TempData["Message"] = Constants.ErrorMessages.MsgInsertFailure;
                #endregion
            }
            if (saveAndExit)
            {
                return RedirectToAction("ListOfLeads");
            }
            else if (model.IsEditMode)
            {
                return RedirectToAction("AddEditLeads", new RouteValueDictionary(new { id = model.LeadID }));
            }
            else
            {
                return RedirectToAction("AddEditLeads");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            bool status = Convert.ToBoolean(await _leadService.DeleteAsync(id));
            return new JsonResult(status);
        }
        #region Helper methods
        private LeadViewModel BindDropdowns(LeadViewModel model)
        {
            model.lstCompanies = GetCompanyDropdown();
            model.lstInsuranceType = GetInsuranceTypeDropdown();
            model.lstUsers = GetUserDropdown();
            model.lstLeadStatus = GetLeadStatusDropdown();
            return model;
        }
        private List<SelectListItem> GetCompanyDropdown()
        {
            return new List<SelectListItem>
            {
                new() { Text = "Bajaj Allienz", Value = "1" },
                new() { Text = "Care Insurance", Value = "2" },
                new() { Text = "Edelwiess", Value = "3" }
            };
        }

        private List<SelectListItem> GetInsuranceTypeDropdown()
        {
            return new List<SelectListItem>
            {
                new() { Text = "Health Insurance", Value = "1" },
                new() { Text = "Motor Insurance", Value = "2" },
                new() { Text = "General Insurance", Value = "3" }
            };
        }

        private List<SelectListItem> GetUserDropdown()
        {
            return new List<SelectListItem>
            {
                new() { Text = "Default User", Value = "1", Selected = true },
                new() { Text = "User 2", Value = "2" }
            };
        }

        private List<SelectListItem> GetLeadStatusDropdown()
        {
            return new List<SelectListItem>
            {
                new() { Text = "Contacted", Value = "1", Selected = true },
                new() { Text = "Qualified", Value = "2" },
                new() { Text = "Proposal Sent", Value = "3" },
                new() { Text = "Negotiation", Value = "4" },
                new() { Text = "Closed/Won", Value = "5" },
                new() { Text = "Closed/Lost", Value = "6" }
            };
        }
        
        #endregion
    }
}
