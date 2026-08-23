using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using Insurancesys.web.Utility;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Application.PolicyForms;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.Enums;
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
        IPolicyFormResolver policyFormResolver) : Controller
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICustomerService _customerService = customerService;
        private readonly IDropDownBinderService _dropDownBinderService = dropDownBinderService;
        private readonly IInsuranceTypeService _insuranceTypeService = insuranceTypeService;
        private readonly IPolicyFormResolver _policyFormResolver = policyFormResolver;

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
                // Use the stored deterministic token if available; fall back to
                // generating one on the fly for existing customers that predate
                // the EncryptedId column.
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

            if (model.MotorPolicies != null && model.MotorPolicies.Count > 0)
                await SaveMotorPoliciesAsync(model.CustomerID, model.MotorPolicies);

            if (model.HealthPolicies != null && model.HealthPolicies.Count > 0)
                await SaveHealthPoliciesAsync(customerId, model.HealthPolicies);

            if (model.LifePolicies != null && model.LifePolicies.Count > 0)
                await SaveLifePoliciesAsync(customerId, model.LifePolicies);

            if (model.PersonalAccidentPolicies != null && model.PersonalAccidentPolicies.Count > 0)
                await SavePersonalAccidentPoliciesAsync(customerId, model.PersonalAccidentPolicies);

            if (model.StandardPolicies != null && model.StandardPolicies.Count > 0)
                await SaveStandardPoliciesAsync(customerId, model.StandardPolicies);

            //var savedCustomer = await _customerService.GetCustomerByIdAsync(model.CustomerID);
            //var cid = !string.IsNullOrEmpty(savedCustomer?.EncryptedCustomerId)
            //    ? savedCustomer!.EncryptedCustomerId!
            //    : Cryptography.EncryptUtf16UrlSafe(Convert.ToString(model.CustomerID));

            TempData["Message"] = "Policy details saved successfully.";
            TempData["RowsAffected"] = "1";
            //return RedirectToAction(nameof(ListOfCustomers), new { cid });
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

            return descriptor.Template switch
            {
                InsuranceTypeCode.MotorVehicle => PartialView(descriptor.PartialViewName, new MotorPolicyViewModel
                {
                    BasicDetails = basic,
                    VehicleDetails = new PolicyVehicleDetailsViewModel()
                }),
                InsuranceTypeCode.Health => PartialView(descriptor.PartialViewName, new HealthPolicyViewModel
                {
                    BasicDetails = basic
                }),
                InsuranceTypeCode.Life => PartialView(descriptor.PartialViewName, new LifePolicyViewModel
                {
                    BasicDetails = basic
                }),
                InsuranceTypeCode.PersonalAccident => PartialView(descriptor.PartialViewName, new PersonalAccidentPolicyViewModel
                {
                    BasicDetails = basic
                }),
                _ => PartialView(descriptor.PartialViewName, new StandardPolicyViewModel
                {
                    BasicDetails = basic
                })
            };
        }

        [HttpGet]
        public Task<IActionResult> GetMotorPolicyPartial(int index, int policyNumber = 1) =>
            GetLegacyTemplatePartialAsync(InsuranceTypeCode.MotorVehicle, index, policyNumber);

        [HttpGet]
        public Task<IActionResult> GetHealthPolicyPartial(int index, int policyNumber = 1) =>
            GetLegacyTemplatePartialAsync(InsuranceTypeCode.Health, index, policyNumber);

        [HttpGet]
        public Task<IActionResult> GetLifePolicyPartial(int index, int policyNumber = 1) =>
            GetLegacyTemplatePartialAsync(InsuranceTypeCode.Life, index, policyNumber);

        [HttpGet]
        public Task<IActionResult> GetPersonalAccidentPolicyPartial(int index, int policyNumber = 1) =>
            GetLegacyTemplatePartialAsync(InsuranceTypeCode.PersonalAccident, index, policyNumber);

        #endregion

        private async Task<CustomerEntity?> SaveOrUpdateCustomer(CustomerViewModel model)
        {
            await HydrateLocationNamesFromIdsAsync(model);
            var customer = _mapper.Map<CustomerEntity>(model);

            if (model.IsEditMode && model.CustomerID > 0)
            {
                customer.CustomerID = model.CustomerID;
                await UpdateCustomerIdForEncryptedValue(customer.CustomerID); //remove this line lateron
                return await _customerService.UpdateCustomerAsync(customer);
            }

            customer.CustomerID = await _customerService.AddCustomer(customer);
            await UpdateCustomerIdForEncryptedValue(customer.CustomerID);
            return customer;
        }

        private async Task UpdateCustomerIdForEncryptedValue(int customerId = 0)
        {
            // Generate a deterministic encrypted token and persist it once.
            // The same CustomerID always produces the same token, so it can
            // be stored in the DB and reused in query-string links.
            if (customerId > 0)
            {
                var encryptedId = Cryptography.EncryptUtf16UrlSafe(Convert.ToString(customerId));
                await _customerService.UpdateEncryptedIdAsync(customerId, encryptedId);
            }
        }
        private async Task SaveMotorPoliciesAsync(int customerId, List<MotorPolicyViewModel> motorPolicies)
        {
            foreach (var policyDto in motorPolicies)
            {
                var isExisting = policyDto.BasicDetails != null && policyDto.BasicDetails.PolicyId > 0;

                var motorPolicy = new MotorPolicyViewModel
                {
                    BasicDetails = new PolicyBasicDetailsViewModel
                    {
                        CustomerID = customerId,
                        PolicyId = policyDto.BasicDetails.PolicyId,
                        InsuranceTypeID = await ResolvePostedInsuranceTypeIdAsync(policyDto.BasicDetails?.InsuranceTypeID, InsuranceTypeCode.MotorVehicle),
                        PolicyNumber = policyDto.BasicDetails.PolicyNumber,
                        PolicyStartDate = policyDto.BasicDetails.PolicyStartDate,
                        PolicyDueDate = policyDto.BasicDetails.PolicyDueDate,
                        Company = policyDto.BasicDetails.Company,
                        GrosssPremium = policyDto.BasicDetails.GrosssPremium,
                        NetPremium = policyDto.BasicDetails.NetPremium,
                        ODPremium = policyDto.BasicDetails.ODPremium,
                        NCB = policyDto.BasicDetails.NCB,
                        Dealer = policyDto.BasicDetails.Dealer,
                        SM = policyDto.BasicDetails.SM,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        IsActive = true
                    },
                    VehicleDetails = policyDto.VehicleDetails,
                    PolicyPaymentDetails = policyDto.PolicyPaymentDetails
                };

                var motorPolicyEntity = _mapper.Map<PolicyDetailsEntity>(motorPolicy.BasicDetails);
                if (isExisting)
                {
                    await _customerService.UpdatePolicyDetailsAsync(motorPolicyEntity);
                }
                else
                {
                    motorPolicyEntity.PolicyId = await _customerService.AddPolicyDetails(motorPolicyEntity);
                    motorPolicy.BasicDetails.PolicyId = motorPolicyEntity.PolicyId;
                }

                if (motorPolicyEntity.PolicyId > 0)
                {
                    await SaveVehicleDetailsAsync(motorPolicyEntity.PolicyId, motorPolicy.VehicleDetails);
                    await SavePolicyPaymentDetailsAsync(motorPolicyEntity.PolicyId, motorPolicy.PolicyPaymentDetails);
                }
            }
        }

        private async Task SaveVehicleDetailsAsync(int policyId, PolicyVehicleDetailsViewModel? vehicleDetailsViewModel)
        {
            if (vehicleDetailsViewModel == null)
                return;

            var policyVehicleEntity = _mapper.Map<PolicyVehicleDetailsEntity>(vehicleDetailsViewModel);
            if (policyId > 0 && policyVehicleEntity != null)
            {
                policyVehicleEntity.PolicyId = policyId;
                var existing = await _customerService.GetVehicleDetailsByPollicyAsync(policyId);
                if (existing != null)
                    await _customerService.UpdateVehicleDetailsAsync(policyVehicleEntity);
                else
                    await _customerService.AddVehicleDetails(policyVehicleEntity);
            }
        }

        private async Task SavePolicyPaymentDetailsAsync(int policyId, PolicyPaymentDetailsViewModel? paymentDetailsViewModel)
        {
            if (paymentDetailsViewModel == null)
                return;

            var paymentEntity = _mapper.Map<PolicyPaymentDetailsEntity>(paymentDetailsViewModel);
            if (policyId > 0 && paymentEntity != null)
            {
                paymentEntity.PolicyId = policyId;
                var existing = await _customerService.GetPolicyPaymentsByPollicyAsync(policyId);
                if (existing != null)
                    await _customerService.UpdatePolicyPaymentDetailsAsync(paymentEntity);
                else
                    await _customerService.AddPolicyPaymentDetails(paymentEntity);
            }
        }

        private async Task SaveHealthPoliciesAsync(int customerId, List<HealthPolicyViewModel> healthPolicies)
        {
            foreach (var policyDto in healthPolicies)
            {
                var isExisting = policyDto.BasicDetails != null && policyDto.BasicDetails.PolicyId > 0;

                var healthPolicy = new HealthPolicyViewModel
                {
                    BasicDetails = new PolicyBasicDetailsViewModel
                    {
                        CustomerID = customerId,
                        PolicyId = policyDto.BasicDetails.PolicyId,
                        InsuranceTypeID = await ResolvePostedInsuranceTypeIdAsync(policyDto.BasicDetails?.InsuranceTypeID, InsuranceTypeCode.Health),
                        PolicyNumber = policyDto.BasicDetails.PolicyNumber,
                        PolicyStartDate = policyDto.BasicDetails.PolicyStartDate,
                        PolicyDueDate = policyDto.BasicDetails.PolicyDueDate,
                        Company = policyDto.BasicDetails.Company,
                        GrosssPremium = policyDto.BasicDetails.GrosssPremium,
                        NetPremium = policyDto.BasicDetails.NetPremium,
                        ODPremium = policyDto.BasicDetails.ODPremium,
                        NCB = policyDto.BasicDetails.NCB,
                        Dealer = policyDto.BasicDetails.Dealer,
                        SM = policyDto.BasicDetails.SM,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        IsActive = true
                    },
                    PolicyPaymentDetails = policyDto.PolicyPaymentDetails
                };

                var policyEntity = _mapper.Map<PolicyDetailsEntity>(healthPolicy.BasicDetails);
                if (isExisting)
                {
                    await _customerService.UpdatePolicyDetailsAsync(policyEntity);
                }
                else
                {
                    policyEntity.PolicyId = await _customerService.AddPolicyDetails(policyEntity);
                    healthPolicy.BasicDetails.PolicyId = policyEntity.PolicyId;
                }

                if (policyEntity.PolicyId > 0)
                    await SavePolicyPaymentDetailsAsync(policyEntity.PolicyId, healthPolicy.PolicyPaymentDetails);
            }
        }

        private async Task SaveLifePoliciesAsync(int customerId, List<LifePolicyViewModel> lifePolicies)
        {
            foreach (var policyDto in lifePolicies)
            {
                var isExisting = policyDto.BasicDetails != null && policyDto.BasicDetails.PolicyId > 0;

                var lifePolicy = new LifePolicyViewModel
                {
                    BasicDetails = new PolicyBasicDetailsViewModel
                    {
                        CustomerID = customerId,
                        PolicyId = policyDto.BasicDetails.PolicyId,
                        InsuranceTypeID = await ResolvePostedInsuranceTypeIdAsync(policyDto.BasicDetails?.InsuranceTypeID, InsuranceTypeCode.Life),
                        PolicyNumber = policyDto.BasicDetails.PolicyNumber,
                        PolicyStartDate = policyDto.BasicDetails.PolicyStartDate,
                        PolicyDueDate = policyDto.BasicDetails.PolicyDueDate,
                        Company = policyDto.BasicDetails.Company,
                        GrosssPremium = policyDto.BasicDetails.GrosssPremium,
                        NetPremium = policyDto.BasicDetails.NetPremium,
                        ODPremium = policyDto.BasicDetails.ODPremium,
                        NCB = policyDto.BasicDetails.NCB,
                        Dealer = policyDto.BasicDetails.Dealer,
                        SM = policyDto.BasicDetails.SM,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        IsActive = true
                    },
                    PolicyPaymentDetails = policyDto.PolicyPaymentDetails
                };

                var policyEntity = _mapper.Map<PolicyDetailsEntity>(lifePolicy.BasicDetails);
                if (isExisting)
                {
                    await _customerService.UpdatePolicyDetailsAsync(policyEntity);
                }
                else
                {
                    policyEntity.PolicyId = await _customerService.AddPolicyDetails(policyEntity);
                    lifePolicy.BasicDetails.PolicyId = policyEntity.PolicyId;
                }

                if (policyEntity.PolicyId > 0)
                    await SavePolicyPaymentDetailsAsync(policyEntity.PolicyId, lifePolicy.PolicyPaymentDetails);
            }
        }

        private async Task SavePersonalAccidentPoliciesAsync(int customerId, List<PersonalAccidentPolicyViewModel> personalAccidentPolicies)
        {
            foreach (var policyDto in personalAccidentPolicies)
            {
                var isExisting = policyDto.BasicDetails != null && policyDto.BasicDetails.PolicyId > 0;

                var paPolicy = new PersonalAccidentPolicyViewModel
                {
                    BasicDetails = new PolicyBasicDetailsViewModel
                    {
                        CustomerID = customerId,
                        PolicyId = policyDto.BasicDetails.PolicyId,
                        InsuranceTypeID = await ResolvePostedInsuranceTypeIdAsync(policyDto.BasicDetails?.InsuranceTypeID, InsuranceTypeCode.PersonalAccident),
                        PolicyNumber = policyDto.BasicDetails.PolicyNumber,
                        PolicyStartDate = policyDto.BasicDetails.PolicyStartDate,
                        PolicyDueDate = policyDto.BasicDetails.PolicyDueDate,
                        Company = policyDto.BasicDetails.Company,
                        GrosssPremium = policyDto.BasicDetails.GrosssPremium,
                        NetPremium = policyDto.BasicDetails.NetPremium,
                        ODPremium = policyDto.BasicDetails.ODPremium,
                        NCB = policyDto.BasicDetails.NCB,
                        Dealer = policyDto.BasicDetails.Dealer,
                        SM = policyDto.BasicDetails.SM,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        IsActive = true
                    },
                    PolicyPaymentDetails = policyDto.PolicyPaymentDetails
                };

                var policyEntity = _mapper.Map<PolicyDetailsEntity>(paPolicy.BasicDetails);
                if (isExisting)
                {
                    await _customerService.UpdatePolicyDetailsAsync(policyEntity);
                }
                else
                {
                    policyEntity.PolicyId = await _customerService.AddPolicyDetails(policyEntity);
                    paPolicy.BasicDetails.PolicyId = policyEntity.PolicyId;
                }

                if (policyEntity.PolicyId > 0)
                    await SavePolicyPaymentDetailsAsync(policyEntity.PolicyId, paPolicy.PolicyPaymentDetails);
            }
        }

        private async Task SaveStandardPoliciesAsync(int customerId, List<StandardPolicyViewModel> standardPolicies)
        {
            foreach (var policyDto in standardPolicies)
            {
                if (policyDto.BasicDetails == null)
                    continue;

                var typeId = policyDto.BasicDetails.InsuranceTypeID;
                var descriptor = await _policyFormResolver.ResolveAsync(typeId);
                if (descriptor is null || !descriptor.IsStandardBucket)
                    continue;

                var isExisting = policyDto.BasicDetails.PolicyId > 0;

                var copy = new StandardPolicyViewModel
                {
                    BasicDetails = new PolicyBasicDetailsViewModel
                    {
                        CustomerID = customerId,
                        PolicyId = policyDto.BasicDetails.PolicyId,
                        InsuranceTypeID = typeId,
                        PolicyNumber = policyDto.BasicDetails.PolicyNumber,
                        PolicyStartDate = policyDto.BasicDetails.PolicyStartDate,
                        PolicyDueDate = policyDto.BasicDetails.PolicyDueDate,
                        Company = policyDto.BasicDetails.Company,
                        GrosssPremium = policyDto.BasicDetails.GrosssPremium,
                        NetPremium = policyDto.BasicDetails.NetPremium,
                        ODPremium = policyDto.BasicDetails.ODPremium,
                        NCB = policyDto.BasicDetails.NCB,
                        Dealer = policyDto.BasicDetails.Dealer,
                        SM = policyDto.BasicDetails.SM,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        IsActive = true
                    },
                    PolicyPaymentDetails = policyDto.PolicyPaymentDetails
                };

                var policyEntity = _mapper.Map<PolicyDetailsEntity>(copy.BasicDetails);
                if (isExisting)
                {
                    await _customerService.UpdatePolicyDetailsAsync(policyEntity);
                }
                else
                {
                    policyEntity.PolicyId = await _customerService.AddPolicyDetails(policyEntity);
                    copy.BasicDetails.PolicyId = policyEntity.PolicyId;
                }

                if (policyEntity.PolicyId > 0)
                    await SavePolicyPaymentDetailsAsync(policyEntity.PolicyId, copy.PolicyPaymentDetails);
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

                switch (descriptor.Template)
                {
                    case InsuranceTypeCode.MotorVehicle:
                        {
                            var vehicle = await _customerService.GetVehicleDetailsByPollicyAsync(policy.PolicyId);
                            var vehicleVm = vehicle != null ? _mapper.Map<PolicyVehicleDetailsViewModel>(vehicle) : null;
                            model.MotorPolicies.Add(new MotorPolicyViewModel
                            {
                                BasicDetails = basicDetails,
                                VehicleDetails = vehicleVm,
                                PolicyPaymentDetails = paymentVm
                            });
                            break;
                        }
                    case InsuranceTypeCode.Health:
                        model.HealthPolicies.Add(new HealthPolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        });
                        break;
                    case InsuranceTypeCode.Life:
                        model.LifePolicies.Add(new LifePolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        });
                        break;
                    case InsuranceTypeCode.PersonalAccident:
                        model.PersonalAccidentPolicies.Add(new PersonalAccidentPolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        });
                        break;
                    default:
                        model.StandardPolicies.Add(new StandardPolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        });
                        break;
                }
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
            if (model.MotorPolicies != null)
            {
                foreach (var p in model.MotorPolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID;
                    if (typeId <= 0) continue;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
            if (model.HealthPolicies != null)
            {
                foreach (var p in model.HealthPolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID;
                    if (typeId <= 0) continue;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
            if (model.LifePolicies != null)
            {
                foreach (var p in model.LifePolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID;
                    if (typeId <= 0) continue;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
            if (model.PersonalAccidentPolicies != null)
            {
                foreach (var p in model.PersonalAccidentPolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID;
                    if (typeId <= 0) continue;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
            if (model.StandardPolicies != null)
            {
                foreach (var p in model.StandardPolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID;
                    if (typeId <= 0) continue;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
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

        /// <summary>
        /// Uses the posted master id when present; otherwise the single active type for that template.
        /// </summary>
        private async Task<int> ResolvePostedInsuranceTypeIdAsync(int? postedInsuranceTypeId, InsuranceTypeCode template)
        {
            if (postedInsuranceTypeId.GetValueOrDefault() > 0)
                return postedInsuranceTypeId!.Value;

            return await _policyFormResolver.GetActiveInsuranceTypeIdByTemplateAsync(template) ?? 0;
        }

        /// <summary>Legacy wrapper endpoints resolve by template, never by hardcoded identity.</summary>
        private async Task<IActionResult> GetLegacyTemplatePartialAsync(InsuranceTypeCode template, int index, int policyNumber)
        {
            var insuranceTypeId = await _policyFormResolver.GetActiveInsuranceTypeIdByTemplateAsync(template);
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

            void Check(PolicyBasicDetailsViewModel? b)
            {
                if (b == null || b.InsuranceTypeID <= 0)
                    return;
                if (b.PolicyId > 0)
                    return;
                if (!active.Contains(b.InsuranceTypeID))
                    ok = false;
            }

            if (model.MotorPolicies != null)
            {
                foreach (var p in model.MotorPolicies)
                    Check(p.BasicDetails);
            }
            if (model.HealthPolicies != null)
            {
                foreach (var p in model.HealthPolicies)
                    Check(p.BasicDetails);
            }
            if (model.LifePolicies != null)
            {
                foreach (var p in model.LifePolicies)
                    Check(p.BasicDetails);
            }
            if (model.PersonalAccidentPolicies != null)
            {
                foreach (var p in model.PersonalAccidentPolicies)
                    Check(p.BasicDetails);
            }
            if (model.StandardPolicies != null)
            {
                foreach (var p in model.StandardPolicies)
                    Check(p.BasicDetails);
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

            // Backfill IDs from old string storage when editing legacy rows.
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
