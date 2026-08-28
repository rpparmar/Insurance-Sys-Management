using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using Insurancesys.web.PolicyForms;
using Insurancesys.web.Utility;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Application.PolicyForms;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.PolicyForms;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "SuperAdmin,AgencyAdmin")]
    public class CustomerController(
        IMapper mapper,
        ICustomerService customerService,
        IDropDownBinderService dropDownBinderService,
        IInsuranceTypeService insuranceTypeService,
        IPolicyFormResolver policyFormResolver,
        IPolicyFormRegistry policyFormRegistry) : Controller
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICustomerService _customerService = customerService;
        private readonly IDropDownBinderService _dropDownBinderService = dropDownBinderService;
        private readonly IInsuranceTypeService _insuranceTypeService = insuranceTypeService;
        private readonly IPolicyFormResolver _policyFormResolver = policyFormResolver;
        private readonly IPolicyFormRegistry _policyFormRegistry = policyFormRegistry;

        [Route("Customers")]
        public IActionResult ListOfCustomers()
        {
            return View("../Customer/ListOfCustomers");
        }

        [Route("Customer/Policies")]
        public async Task<IActionResult> ManagePolicies(string? cid = null)
        {
            var model = new PolicyDetailsViewModel
            {
                InsuranceTypesForGrid = await BuildInsuranceTypesForGridAsync()
            };

            if (!string.IsNullOrWhiteSpace(cid))
            {
                _ = int.TryParse(Cryptography.DecryptUtf16(cid), out int customerId);
                if (customerId <= 0)
                {
                    ViewBag.InvalidRequest = true;
                    return View("../Customer/CustomerPolicies", model);
                }
                model = await BuildPolicyDetailsViewModelAsync(customerId);
            }

            return View("../Customer/CustomerPolicies", model);
        }

        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await DataTableHelper.BuildGridResponseAsync(Request, _customerService.GetAllAsync);
            return Json(result);
        }

        /// <summary>
        /// Soft-deletes a customer. <paramref name="id"/> is the encrypted token (same as <c>EncryptedCustomerId</c>).
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new JsonResult(false);

            string? plain;
            try
            {
                plain = Cryptography.DecryptUtf16(id);
            }
            catch
            {
                return new JsonResult(false);
            }

            if (string.IsNullOrEmpty(plain) || !int.TryParse(plain, out var customerId) || customerId <= 0)
                return new JsonResult(false);

            var rows = await _customerService.DeleteAsync(customerId);
            return new JsonResult(Convert.ToBoolean(rows));
        }

        [HttpGet("Customer/Add")]
        [HttpGet("Customer/Edit/{id}")]
        public async Task<IActionResult> AddEditCustomer(string? id = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                var newModel = new CustomerViewModel
                {
                    IsEditMode = false,
                    IsActive = true
                };
                await PopulateCountryStateDropdownsAsync(newModel);
                return View("../Customer/AddEditCustomer", newModel);
            }

            _ = int.TryParse(Cryptography.DecryptUtf16(id), out int customerId);
            if (customerId <= 0)
            {
                TempData["Message"] = "Invalid customer reference.";
                TempData["RowsAffected"] = "0";
                return RedirectToAction(nameof(ListOfCustomers));
            }

            var entity = await _customerService.GetCustomerByIdAsync(customerId);
            if (entity == null)
            {
                TempData["Message"] = "Customer not found.";
                TempData["RowsAffected"] = "0";
                return RedirectToAction(nameof(ListOfCustomers));
            }

            var model = _mapper.Map<CustomerViewModel>(entity);
            model.IsEditMode = true;
            await PopulateCountryStateDropdownsAsync(model);
            return View("../Customer/AddEditCustomer", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCustomer(CustomerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please correct the errors and try again.";
                TempData["RowsAffected"] = "0";
                await PopulateCountryStateDropdownsAsync(model);
                return View("AddEditCustomer", model);
            }

            var customer = await SaveOrUpdateCustomer(model);
            if (customer is null || customer.CustomerID <= 0)
            {
                TempData["Message"] = "Failed to save customer. Please try again.";
                TempData["RowsAffected"] = "0";
                await PopulateCountryStateDropdownsAsync(model);
                return View("AddEditCustomer", model);
            }

            if (!model.IsEditMode && customer.CustomerID > 0)
                TempData["Message"] = "Customer saved successfully.";
            else
                TempData["Message"] = "Customer updated successfully.";
            TempData["RowsAffected"] = "1";

            if (string.IsNullOrWhiteSpace(model.SubmitType))
                return RedirectToAction("ListOfCustomers");

            if (model.SubmitType.Equals("customeronly", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("ListOfCustomers");

            if (model.SubmitType.Equals("customerwithpolicies", StringComparison.OrdinalIgnoreCase))
            {
                var cid = !string.IsNullOrEmpty(customer.EncryptedCustomerId)
                    ? customer.EncryptedCustomerId
                    : Cryptography.EncryptUtf16UrlSafe(Convert.ToString(customer.CustomerID));
                return RedirectToAction("ManagePolicies", new { cid });
            }

            return RedirectToAction("ListOfCustomers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePolicies(PolicyDetailsViewModel model)
        {
            int customerId = model.CustomerID;

            var activeTypeIds = await _insuranceTypeService.GetActiveInsuranceTypeIdsAsync();
            if (activeTypeIds == null || activeTypeIds.Count == 0)
            {
                TempData["Message"] = "No active insurance types are configured in the system. Please configure insurance types before saving policies.";
                TempData["RowsAffected"] = "0";
                await RepopulatePolicyDropdownsAsync(model);
                model.InsuranceTypesForGrid = await BuildInsuranceTypesForGridAsync();
                await PopulateCustomerDisplayNameAsync(model);
                return View("CustomerPolicies", model);
            }

            if (!await ValidatePostedPoliciesAgainstMasterAsync(model))
            {
                TempData["Message"] = "One or more policies use an inactive or invalid insurance type.";
                TempData["RowsAffected"] = "0";
                await RepopulatePolicyDropdownsAsync(model);
                model.InsuranceTypesForGrid = await BuildInsuranceTypesForGridAsync();
                await PopulateCustomerDisplayNameAsync(model);
                return View("CustomerPolicies", model);
            }

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please correct the errors and try again.";
                TempData["RowsAffected"] = "0";
                await RepopulatePolicyDropdownsAsync(model);
                model.InsuranceTypesForGrid = await BuildInsuranceTypesForGridAsync();
                await PopulateCustomerDisplayNameAsync(model);
                return View("CustomerPolicies", model);
            }

            foreach (var handler in _policyFormRegistry.All)
                await handler.SavePostedAsync(model, customerId);

            TempData["Message"] = "Policy details saved successfully.";
            TempData["RowsAffected"] = "1";
            return RedirectToAction(nameof(ListOfCustomers));
        }

        #region AJAX calls

        [HttpGet]
        public async Task<IActionResult> GetStatesByCountry(int countryId)
        {
            if (countryId <= 0)
                return Json(new List<SelectListItem>());

            var states = await _dropDownBinderService.GetStateDropdownByCountryAsync(countryId);
            var result = states.Select(x => new SelectListItem { Value = x.Value, Text = x.Text }).ToList();
            return Json(result);
        }

        /// <summary>
        /// Immediately soft-deletes a single existing policy.
        /// Called via AJAX on remove button confirmation.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePolicy(int policyId, int customerId)
        {
            if (policyId <= 0 || customerId <= 0)
                return Json(new { success = false, message = "Invalid request." });

            var deleted = await _customerService.SoftDeletePolicyAsync(policyId, customerId);
            if (!deleted)
                return Json(new { success = false, message = "Policy not found or already removed." });

            return Json(new { success = true });
        }

        /// <summary>Loads the policy partial for a master insurance type using its form-template key.</summary>
        /// <param name="insuranceTypeId">Master primary key (not an enum value).</param>
        /// <param name="index">Model-binding collection index.</param>
        /// <param name="policyNumber">1-based display number.</param>
        [HttpGet]
        public async Task<IActionResult> GetPolicyPartial(int insuranceTypeId, int index, int policyNumber = 1)
        {
            var descriptor = await _policyFormResolver.ResolveAsync(insuranceTypeId);
            if (descriptor is null)
                return BadRequest("Invalid insurance type.");
            if (!descriptor.IsActive)
                return BadRequest("This insurance type is inactive.");

            ApplyDescriptorViewBag(descriptor, index, policyNumber);

            var basic = new PolicyBasicDetailsViewModel { InsuranceTypeID = insuranceTypeId };
            basic.CompanySelectList = await GetCompanySelectListAsync(basic.InsuranceTypeID, basic.Company);

            var handler = _policyFormRegistry.Get(descriptor.FormTemplateKey);
            return PartialView(descriptor.PartialViewName, handler.CreateBlankViewModel(basic));
        }

        [HttpGet]
        public Task<IActionResult> GetMotorPolicyPartial(int index, int policyNumber = 1) =>
            GetLegacyTemplatePartialAsync(PolicyFormCatalog.MotorVehicleKey, index, policyNumber);

        [HttpGet]
        public Task<IActionResult> GetHealthPolicyPartial(int index, int policyNumber = 1) =>
            GetLegacyTemplatePartialAsync(PolicyFormCatalog.HealthKey, index, policyNumber);

        [HttpGet]
        public Task<IActionResult> GetLifePolicyPartial(int index, int policyNumber = 1) =>
            GetLegacyTemplatePartialAsync(PolicyFormCatalog.LifeKey, index, policyNumber);

        [HttpGet]
        public Task<IActionResult> GetPersonalAccidentPolicyPartial(int index, int policyNumber = 1) =>
            GetLegacyTemplatePartialAsync(PolicyFormCatalog.PersonalAccidentKey, index, policyNumber);

        #endregion

        private async Task<CustomerEntity?> SaveOrUpdateCustomer(CustomerViewModel model)
        {
            await HydrateLocationNamesFromIdsAsync(model);
            var customer = _mapper.Map<CustomerEntity>(model);

            if (model.IsEditMode && model.CustomerID > 0)
            {
                customer.CustomerID = model.CustomerID;
                await UpdateCustomerIdForEncryptedValue(customer.CustomerID);
                return await _customerService.UpdateCustomerAsync(customer);
            }

            customer.CustomerID = await _customerService.AddCustomer(customer);
            await UpdateCustomerIdForEncryptedValue(customer.CustomerID);
            return customer;
        }

        private async Task UpdateCustomerIdForEncryptedValue(int customerId = 0)
        {
            if (customerId > 0)
            {
                var encryptedId = Cryptography.EncryptUtf16UrlSafe(Convert.ToString(customerId));
                await _customerService.UpdateEncryptedIdAsync(customerId, encryptedId);
            }
        }

        private async Task<PolicyDetailsViewModel> BuildPolicyDetailsViewModelAsync(int customerId)
        {
            var model = new PolicyDetailsViewModel
            {
                CustomerID = customerId,
                InsuranceTypesForGrid = await BuildInsuranceTypesForGridAsync()
            };
            await PopulateCustomerDisplayNameAsync(model);

            var policies = await _customerService.GetPolicyDetailsByCustomerAsync(customerId);
            if (policies == null || policies.Count == 0)
                return model;

            var typeLookup = (await _insuranceTypeService.GetAllNonDeletedAsync())
                .ToDictionary(x => x.InsuranceTypeId);

            foreach (var policy in policies)
            {
                var basicDetails = _mapper.Map<PolicyBasicDetailsViewModel>(policy);
                var payment = await _customerService.GetPolicyPaymentsByPollicyAsync(policy.PolicyId);
                var paymentVm = payment != null ? _mapper.Map<PolicyPaymentDetailsViewModel>(payment) : null;

                basicDetails.CompanySelectList = await GetCompanySelectListAsync(basicDetails.InsuranceTypeID, basicDetails.Company);

                if (!typeLookup.TryGetValue(policy.InsuranceTypeID, out var typeEntity))
                    continue;

                var descriptor = _policyFormResolver.FromEntity(typeEntity);
                var handler = _policyFormRegistry.Get(descriptor.FormTemplateKey);
                await handler.AppendLoadedPolicyAsync(model, new PolicyFormLoadContext
                {
                    BasicDetails = basicDetails,
                    Payment = paymentVm,
                    PolicyId = policy.PolicyId
                });
            }

            return model;
        }

        private async Task<List<SelectListItem>> GetCompanySelectListAsync(int insuranceTypeId, string? selectedCompanyId)
        {
            var companies = await _dropDownBinderService.GetCompanyMappedWithInsuranceType(insuranceTypeId);
            return DropdownMapper.ToSelectListItems(companies ?? new List<DropdownItemDto>(), selectedCompanyId);
        }

        private static string FormatCustomerDisplayName(CustomerEntity c)
        {
            var first = string.IsNullOrWhiteSpace(c.FirstName) ? string.Empty : c.FirstName.Trim();
            var last = string.IsNullOrWhiteSpace(c.LastName) ? string.Empty : c.LastName.Trim();
            if (string.IsNullOrEmpty(first) && string.IsNullOrEmpty(last))
                return string.Empty;
            return string.IsNullOrEmpty(last) ? first : $"{first} {last}".Trim();
        }

        private async Task PopulateCustomerDisplayNameAsync(PolicyDetailsViewModel model)
        {
            if (model.CustomerID <= 0)
            {
                model.CustomerDisplayName = null;
                return;
            }

            var customer = await _customerService.GetCustomerByIdAsync(model.CustomerID);
            model.CustomerDisplayName = customer == null ? null : FormatCustomerDisplayName(customer);
        }

        private async Task RepopulatePolicyDropdownsAsync(PolicyDetailsViewModel model)
        {
            foreach (var handler in _policyFormRegistry.All)
                await handler.RepopulateDropdownsAsync(model, GetCompanySelectListAsync);
        }

        private async Task<List<InsuranceTypePolicyGridItemViewModel>> BuildInsuranceTypesForGridAsync()
        {
            var rows = await _insuranceTypeService.GetAllNonDeletedAsync();
            var list = new List<InsuranceTypePolicyGridItemViewModel>();
            foreach (var entity in rows.OrderBy(x => x.InsuranceTypeId))
            {
                var descriptor = _policyFormResolver.FromEntity(entity);
                list.Add(new InsuranceTypePolicyGridItemViewModel
                {
                    InsuranceTypeId = descriptor.InsuranceTypeId,
                    FormTemplateKey = descriptor.FormTemplateKey,
                    EnumName = descriptor.FormTemplateKey,
                    DisplayName = descriptor.DisplayName,
                    IsActive = descriptor.IsActive,
                    Slug = descriptor.Slug,
                    IconClass = descriptor.IconClass,
                    FormCollectionPrefix = descriptor.FormCollectionPrefix,
                    BindingPolicyType = descriptor.BindingPolicyType,
                    IsStandardBucket = descriptor.IsStandardBucket
                });
            }

            return list;
        }

        /// <summary>Legacy wrapper endpoints resolve by template key, never by hardcoded identity.</summary>
        private async Task<IActionResult> GetLegacyTemplatePartialAsync(string templateKey, int index, int policyNumber)
        {
            var insuranceTypeId = await _policyFormResolver.GetActiveInsuranceTypeIdByTemplateKeyAsync(templateKey);
            if (insuranceTypeId is null or <= 0)
                return BadRequest("No active insurance type is configured for this form.");

            return await GetPolicyPartial(insuranceTypeId.Value, index, policyNumber);
        }

        private void ApplyDescriptorViewBag(PolicyFormDescriptor descriptor, int index, int policyNumber)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicySlug = descriptor.Slug;
            ViewBag.PolicyType = descriptor.BindingPolicyType;
            ViewBag.PolicyDisplayTitle = descriptor.DisplayName;
        }

        private async Task<bool> ValidatePostedPoliciesAgainstMasterAsync(PolicyDetailsViewModel model)
        {
            var active = await _insuranceTypeService.GetActiveInsuranceTypeIdsAsync();
            var ok = true;

            foreach (var handler in _policyFormRegistry.All)
            {
                foreach (var basic in handler.EnumeratePostedBasics(model))
                {
                    if (basic.InsuranceTypeID <= 0 || basic.PolicyId > 0)
                        continue;
                    if (!active.Contains(basic.InsuranceTypeID))
                        ok = false;
                }
            }

            return ok;
        }

        private async Task PopulateCountryStateDropdownsAsync(CustomerViewModel model)
        {
            var countries = await _dropDownBinderService.GetCountryDropdownAsync();
            model.CountrySelectList = countries.Select(x => new SelectListItem
            {
                Value = x.Value,
                Text = x.Text,
                Selected = model.CountryID.HasValue && x.Value == model.CountryID.Value.ToString()
            }).ToList();

            if (!model.CountryID.HasValue && !string.IsNullOrWhiteSpace(model.Country))
            {
                var selectedCountry = countries.FirstOrDefault(x => string.Equals(x.Value, model.Country, StringComparison.OrdinalIgnoreCase));
                if (selectedCountry != null && int.TryParse(selectedCountry.Value, out int selectedCountryId))
                {
                    model.CountryID = selectedCountryId;
                }
            }

            model.StateSelectList = new List<SelectListItem>();
            if (model.CountryID.HasValue && model.CountryID.Value > 0)
            {
                var states = await _dropDownBinderService.GetStateDropdownByCountryAsync(model.CountryID.Value);
                if (!model.StateID.HasValue && !string.IsNullOrWhiteSpace(model.State))
                {
                    var selectedState = states.FirstOrDefault(x => string.Equals(x.Value, model.State, StringComparison.OrdinalIgnoreCase));
                    if (selectedState != null && int.TryParse(selectedState.Value, out int selectedStateId))
                    {
                        model.StateID = selectedStateId;
                    }
                }

                model.StateSelectList = states.Select(x => new SelectListItem
                {
                    Value = x.Value,
                    Text = x.Text,
                    Selected = model.StateID.HasValue && x.Value == model.StateID.Value.ToString()
                }).ToList();
            }
        }

        private async Task HydrateLocationNamesFromIdsAsync(CustomerViewModel model)
        {
            if (model.CountryID.HasValue && model.CountryID.Value > 0)
            {
                var countries = await _dropDownBinderService.GetCountryDropdownAsync();
                var selectedCountry = countries.FirstOrDefault(x => x.Value == model.CountryID.Value.ToString());
                model.Country = selectedCountry?.Value;
            }

            if (model.CountryID.HasValue && model.CountryID.Value > 0 && model.StateID.HasValue && model.StateID.Value > 0)
            {
                var states = await _dropDownBinderService.GetStateDropdownByCountryAsync(model.CountryID.Value);
                var selectedState = states.FirstOrDefault(x => x.Value == model.StateID.Value.ToString());
                model.State = selectedState?.Value;
            }
            else
            {
                model.State = null;
            }
        }
    }
}
