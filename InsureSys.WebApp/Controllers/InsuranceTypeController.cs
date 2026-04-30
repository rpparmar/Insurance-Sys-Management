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
using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;
using System.Data;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin,AgencyAdmin")] // Use cookie authentication
    public class InsuranceTypeController : Controller
    {
        private readonly IInsuranceTypeService _insuranceTypeService;        
        private readonly IDropDownBinderService _dropDownBinderService;
        private readonly IMapper _mapper;
        public InsuranceTypeController(IInsuranceTypeService insuranceTypeService, IMapper mapper, IDropDownBinderService dropDownBinderService)
        {
            _insuranceTypeService= insuranceTypeService;
            _mapper=mapper;
            _dropDownBinderService = dropDownBinderService;
        }
        [Route("InsuranceTypes")]
        public IActionResult InsuranceTypeList()
        {
            return View("../Masters/InsuranceType/InsuranceTypeList");
        }
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await DataTableHelper.BuildGridResponseAsync(Request, _insuranceTypeService.GetAllAsync);
            return Json(result);
        }

        [HttpGet("InsuranceType/Add")]
        [HttpGet("InsuranceType/Edit/{id}")]
        public async Task<IActionResult> AddEditInsuranceType(string id = "")
        {
            InsuranceTypeViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int _id) && _id > 0)
                {
                    var insurancetypedto = await _insuranceTypeService.GetByIdAsync(Convert.ToInt16(_id));
                    if (insurancetypedto != null)
                    {
                        model = _mapper.Map<InsuranceTypeViewModel>(insurancetypedto);
                        HttpContext.Session.SetString("Original_InsuranceType", model.InsuranceType);
                        model.IsEditMode = true;
                    }
                    else
                    {
                        SetTempDataForNoRecord();
                        HttpContext.Session.SetString("Original_InsuranceType", "");
                        return RedirectToAction("InsuranceTypeList");
                    }
                }
                else
                {
                    SetTempDataForNoRecord();
                    HttpContext.Session.SetString("Original_InsuranceType", "");
                    return RedirectToAction("InsuranceTypeList");
                }
            }
            else
            {
                model.IsEditMode = false;
                model.IsActive = true;
                HttpContext.Session.SetString("Original_InsuranceType", "");
            }
            model = await BindDropdowns(model);
            return View("../Masters/InsuranceType/AddEditInsuranceType", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveInsuranceType(InsuranceTypeViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
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
                return View("../Masters/InsuranceType/AddEditInsuranceType", model);
            }
            var insuranceType = _mapper.Map<InsuranceTypeEntity>(model);

            if (model.IsEditMode)
            {
                #region Update
                var rowsaffected = await _insuranceTypeService.UpdateAsync(insuranceType);
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
                var rowsaffected = await _insuranceTypeService.AddAsync(insuranceType);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = Constants.SuccessMessages.MsgInsertSuccess;
                else
                    TempData["Message"] = Constants.ErrorMessages.MsgInsertFailure;
                #endregion
            }
            if (saveAndExit)
            {
                return RedirectToAction("InsuranceTypeList");
            }
            else if (model.IsEditMode)
            {
                return RedirectToAction("AddEditInsuranceType", new RouteValueDictionary(new { id = model.InsuranceTypeId }));
            }
            else
            {
                return RedirectToAction("AddEditInsuranceType");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            bool status = Convert.ToBoolean(await _insuranceTypeService.DeleteAsync(id));
            return new JsonResult(status);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsInsurancetypeExist(string InsuranceType = "")
        {
            string Original_InsuranceType = HttpContext.Session.GetString("Original_InsuranceType") ?? "";
            bool IsEditMode = !string.IsNullOrEmpty(Original_InsuranceType);
            bool IsExist = await _insuranceTypeService.FindByNameAsync(InsuranceType);
            if (IsEditMode && !string.Equals(Original_InsuranceType, InsuranceType) && IsExist)
                return Json($"Insurance type '{InsuranceType}' is already in use.");
            else if (!IsEditMode && IsExist)
                return Json($"Insurance type '{InsuranceType}' is already in use.");
            return Json(true);
        }
        #region Helper methods
        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        }
        private async Task<InsuranceTypeViewModel> BindDropdowns(InsuranceTypeViewModel model)
        {
            var companies = await _dropDownBinderService.GetCompanyDropdownAsync();            
            model.lstOfCompanies = DropdownMapper.ToSelectListItems(companies ?? new List<DropdownItemDto>(),model.AssociationWithCompanyIDs);            
            return model;
        }        
        #endregion
    }
}
