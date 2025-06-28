using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using Insurancesys.web.Models.Common;
using InsuranceSys.Application;
using InsuranceSys.Application.DTO;
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
        private readonly IDropDownBinderService _dropDownBinderService;
        private readonly IMapper _mapper;
        public LeadController(
             ILeadService leadService,
             IMapper mapper,
             IDropDownBinderService dropDownBinderService)
        {
            _leadService = leadService;
            _mapper = mapper;
            _dropDownBinderService = dropDownBinderService;
        }
        [Route("Leads")]
        public IActionResult ListOfLeads()
        {            
            return View("../Customer/ListOfLeads");
        }

        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await DataTableHelper.BuildGridResponseAsync(Request
                , _leadService.GetAllAsync
                /* Add extra filter parameters here */
                //, request =>
                //{
                //    var extra = new Dictionary<string, object>();                
                //    var _statusId = DataTableHelper.SearchSortValue(request.Form, "StatusId");
                //    extra.Add("@StatusId", _statusId);
                //    return extra;
                //}
                );
            return Json(result);
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
            model = await BindDropdowns(model);
            return View("../Customer/AddEditLeads", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLead(LeadViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
                model = await BindDropdowns(model);
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
        [HttpPost]        
        public async Task<List<SelectListItem>> GetCompaniesByPolicyType(int insuranceTypeId)
        {
            return await GetCompanyDropdown(insuranceTypeId);
        }
        #region Helper methods
        private async Task<LeadViewModel> BindDropdowns(LeadViewModel model)
        {
            int.TryParse(model.PolicyTypeID, out int insuranceTypeId);
            model.lstCompanies = await GetCompanyDropdown(insuranceTypeId);
            model.lstInsuranceType = await GetInsuranceTypeDropdown(model);
            model.lstLeadStatus = GetLeadStatusDropdown();
            model.lstUsers = GetUserDropdown();
            return model;
        }
        private async Task<List<SelectListItem>> GetCompanyDropdown(int insuranceTypeId)
        {
            var companies = await _dropDownBinderService.GetCompanyMappedWithInsuranceType(insuranceTypeId);
            return DropdownMapper.ToSelectListItems(companies ?? new List<DropdownItemDto>());            
        }

        private async Task<List<SelectListItem>> GetInsuranceTypeDropdown(LeadViewModel model)
        {            
            var insuranceTypes = await _dropDownBinderService.GetInsuranceTypeDropdownAsync();
            return DropdownMapper.ToSelectListItems(insuranceTypes ?? new List<DropdownItemDto>());
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
