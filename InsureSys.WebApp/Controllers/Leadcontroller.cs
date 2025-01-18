using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Insurancesys.web.Controllers
{
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
    public class Leadcontroller : Controller
    {
        private readonly ILeadService _leadService;
        private readonly IMapper _mapper;
        public Leadcontroller(
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
        [HttpGet("Leads/Edit/{id}")]
        [HttpGet("Leads/Add")]
        public async Task<ActionResult> AddEditLeads(string id = "")
        {
            LeadViewModel model = new();
            List<SelectListItem> companies = new List<SelectListItem>();
            companies.Add(new SelectListItem { Text = "Bajaj Allienz", Value = "1" });
            companies.Add(new SelectListItem { Text = "Care Insurance", Value = "2" });
            companies.Add(new SelectListItem { Text = "Edelwiess", Value = "3" });
            model.lstCompanies = companies;

            List<SelectListItem> typeofinsurance = new List<SelectListItem>();
            typeofinsurance.Add(new SelectListItem { Text = "Health Insurance", Value = "1" });
            typeofinsurance.Add(new SelectListItem { Text = "Motor Insurance", Value = "2" });
            typeofinsurance.Add(new SelectListItem { Text = "General Insurance", Value = "3" });
            model.lstInsuranceType = typeofinsurance;

            List<SelectListItem> assignee = new List<SelectListItem>();
            assignee.Add(new SelectListItem { Text = "Default User", Value = "1", Selected = true });
            assignee.Add(new SelectListItem { Text = "User 2", Value = "2" });
            model.lstUsers = assignee;

            List<SelectListItem> leadstatus = new List<SelectListItem>();
            leadstatus.Add(new SelectListItem { Text = "Contacted", Value = "1", Selected = true });
            leadstatus.Add(new SelectListItem { Text = "Qualified", Value = "2"});
            leadstatus.Add(new SelectListItem { Text = "Proposal Sent", Value = "3"});
            leadstatus.Add(new SelectListItem { Text = "Negotiation", Value = "4"});
            leadstatus.Add(new SelectListItem { Text = "Closed/Won", Value = "5"});
            leadstatus.Add(new SelectListItem { Text = "Closed/Lost", Value = "6"});
            model.lstLeadStatus= leadstatus;

            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int _id) && _id > 0)
                {
                    var leaddto = await _leadService.GetLeadByIdAsync(Convert.ToInt16(_id));
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
            return View("../Customer/AddEditLeads", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLead(LeadViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                foreach (var item in errors)
                {
                    ModelState.AddModelError(item.Key, item.Errors.Select(s => s.ErrorMessage).ToString());
                }
                return View("../Customer/AddEditLeads", model);
            }
            var lead = _mapper.Map<LeadDto>(model);
            if (model.IsEditMode)
            {
                #region Update
                int rowsaffected = await _leadService.UpdateLead(lead);
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
                int rowsaffected = await _leadService.AddLead(lead);
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
                return RedirectToAction("AddEditLead", new RouteValueDictionary(new { id = model.LeadID }));
            }
            else
            {
                return RedirectToAction("AddEditLead");
            }
        }
    }
}
