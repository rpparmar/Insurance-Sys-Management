using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.PolicyForms;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin,AgencyAdmin")]
    public class InsuranceTypeController : Controller
    {
        private readonly IInsuranceTypeService _insuranceTypeService;
        private readonly IDropDownBinderService _dropDownBinderService;
        private readonly IPolicyFormResolver _policyFormResolver;
        private readonly IMapper _mapper;

        public InsuranceTypeController(
            IInsuranceTypeService insuranceTypeService,
            IMapper mapper,
            IDropDownBinderService dropDownBinderService,
            IPolicyFormResolver policyFormResolver)
        {
            _insuranceTypeService = insuranceTypeService;
            _mapper = mapper;
            _dropDownBinderService = dropDownBinderService;
            _policyFormResolver = policyFormResolver;
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

        /// <summary>
        /// Renders add or edit for an insurance type, including form-template and icon catalogs.
        /// </summary>
        /// <param name="id">Insurance type id when editing; empty for create.</param>
        [HttpGet("InsuranceType/Add")]
        [HttpGet("InsuranceType/Edit/{id}")]
        public async Task<IActionResult> AddEditInsuranceType(string id = "")
        {
            InsuranceTypeViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int insuranceTypeId) && insuranceTypeId > 0)
                {
                    var insuranceTypeDto = await _insuranceTypeService.GetByIdAsync(Convert.ToInt16(insuranceTypeId));
                    if (insuranceTypeDto != null)
                    {
                        model = _mapper.Map<InsuranceTypeViewModel>(insuranceTypeDto);
                        HttpContext.Session.SetString("Original_InsuranceType", model.InsuranceType);
                        model.IsEditMode = true;
                        HydrateTemplateDefaults(model);
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
                model.InsuranceTypeCode = PolicyFormCatalog.StandardKey;
                model.IconClass = PolicyFormCatalog.DefaultIconClass;
                HttpContext.Session.SetString("Original_InsuranceType", "");
            }

            model = await BindDropdowns(model);
            return View("../Masters/InsuranceType/AddEditInsuranceType", model);
        }

        /// <summary>
        /// Persists an insurance type and enforces a single active specialized form template.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveInsuranceType(InsuranceTypeViewModel model, bool saveAndExit = true)
        {
            NormalizePostedTemplate(model);

            if (!ModelState.IsValid)
            {
                model = await BindDropdowns(model);
                return View("../Masters/InsuranceType/AddEditInsuranceType", model);
            }

            if (!await TryValidateSpecializedTemplateAsync(model))
            {
                model = await BindDropdowns(model);
                return View("../Masters/InsuranceType/AddEditInsuranceType", model);
            }

            var insuranceType = _mapper.Map<InsuranceTypeEntity>(model);

            if (model.IsEditMode)
            {
                var rowsAffected = await _insuranceTypeService.UpdateAsync(insuranceType);
                TempData["RowsAffected"] = rowsAffected;
                TempData["Message"] = rowsAffected > 0
                    ? Constants.SuccessMessages.MsgUpdateSuccess
                    : Constants.ErrorMessages.MsgUpdateFailure;
            }
            else
            {
                var rowsAffected = await _insuranceTypeService.AddAsync(insuranceType);
                TempData["RowsAffected"] = rowsAffected;
                TempData["Message"] = rowsAffected > 0
                    ? Constants.SuccessMessages.MsgInsertSuccess
                    : Constants.ErrorMessages.MsgInsertFailure;
            }

            if (saveAndExit)
                return RedirectToAction("InsuranceTypeList");

            if (model.IsEditMode)
                return RedirectToAction("AddEditInsuranceType", new RouteValueDictionary(new { id = model.InsuranceTypeId }));

            return RedirectToAction("AddEditInsuranceType");
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
            string originalInsuranceType = HttpContext.Session.GetString("Original_InsuranceType") ?? "";
            bool isEditMode = !string.IsNullOrEmpty(originalInsuranceType);
            bool isExist = await _insuranceTypeService.FindByNameAsync(InsuranceType);
            if (isEditMode && !string.Equals(originalInsuranceType, InsuranceType) && isExist)
                return Json($"Insurance type '{InsuranceType}' is already in use.");
            if (!isEditMode && isExist)
                return Json($"Insurance type '{InsuranceType}' is already in use.");
            return Json(true);
        }

        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        }

        /// <summary>
        /// Loads company, form-template, and icon dropdowns for the master form.
        /// </summary>
        private async Task<InsuranceTypeViewModel> BindDropdowns(InsuranceTypeViewModel model)
        {
            var companies = await _dropDownBinderService.GetCompanyDropdownAsync();
            model.lstOfCompanies = DropdownMapper.ToSelectListItems(companies ?? new List<DropdownItemDto>(), model.AssociationWithCompanyIDs);
            model.FormTemplateSelectList = PolicyFormCatalog.GetTemplateOptions(model.InsuranceTypeCode)
                .Select(o => new SelectListItem { Value = o.Value, Text = o.Text, Selected = o.Selected })
                .ToList();
            model.IconSelectList = PolicyFormCatalog.GetIconOptions(model.IconClass)
                .Select(o => new SelectListItem { Value = o.Value, Text = o.Text, Selected = o.Selected })
                .ToList();
            return model;
        }

        /// <summary>
        /// When a legacy row has an empty template key, pre-select the effective template
        /// so saving persists the decoupled value.
        /// </summary>
        private static void HydrateTemplateDefaults(InsuranceTypeViewModel model)
        {
            var definition = PolicyFormCatalog.ResolveDefinition(model.InsuranceTypeCode, model.InsuranceTypeId);
            model.InsuranceTypeCode = definition.Key;
            if (string.IsNullOrWhiteSpace(model.IconClass))
                model.IconClass = definition.DefaultIconClass;
        }

        private static void NormalizePostedTemplate(InsuranceTypeViewModel model)
        {
            if (PolicyFormCatalog.TryGet(model.InsuranceTypeCode, out var definition))
                model.InsuranceTypeCode = definition.Key;

            if (string.IsNullOrWhiteSpace(model.IconClass))
                model.IconClass = PolicyFormCatalog.DefaultIconClass;
        }

        /// <summary>
        /// Blocks a second active specialized form template (Motor / Health / Life / Personal Accident).
        /// </summary>
        private async Task<bool> TryValidateSpecializedTemplateAsync(InsuranceTypeViewModel model)
        {
            if (!PolicyFormCatalog.TryGet(model.InsuranceTypeCode, out var definition))
            {
                ModelState.AddModelError(nameof(model.InsuranceTypeCode), "Select a valid form template.");
                return false;
            }

            model.InsuranceTypeCode = definition.Key;

            if (!model.IsActive.GetValueOrDefault())
                return true;

            var excludeId = model.IsEditMode && model.InsuranceTypeId > 0 ? model.InsuranceTypeId : (int?)null;
            var conflict = await _policyFormResolver.FindConflictingActiveSpecializedAsync(definition.Key, excludeId);
            if (conflict is null)
                return true;

            ModelState.AddModelError(
                nameof(model.InsuranceTypeCode),
                string.Format(
                    Constants.ErrorMessages.MsgSpecializedTemplateInUse,
                    definition.AdminDisplayName,
                    conflict.DisplayName));
            return false;
        }
    }
}
