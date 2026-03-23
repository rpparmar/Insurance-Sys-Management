using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using Insurancesys.web.Utility;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CustomerController(IMapper mapper, ICustomerService customerService, IDropDownBinderService dropDownBinderService) : Controller
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICustomerService _customerService = customerService;
        private readonly IDropDownBinderService _dropDownBinderService = dropDownBinderService;

        [Route("Customers")]
        public IActionResult ListOfCustomers()
        {
            return View("../Customer/ListOfCustomers");
        }

        [Route("Customer/Policies")]
        public async Task<IActionResult> ManagePolicies(string? cid = null)
        {
            var model = new PolicyDetailsViewModel();

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
                return View("AddEditCustomer", model);
            }

            var customer = await SaveOrUpdateCustomer(model);
            if (customer is null || customer.CustomerID <= 0)
            {
                TempData["Message"] = "Failed to save customer. Please try again.";
                TempData["RowsAffected"] = "0";
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

            if (!ModelState.IsValid)
            {
                TempData["Message"] = "Please correct the errors and try again.";
                TempData["RowsAffected"] = "0";
                await RepopulatePolicyDropdownsAsync(model);
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

            return RedirectToAction("ListOfCustomers");
        }

        #region AJAX calls

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

        [HttpGet]
        public async Task<IActionResult> GetMotorPolicyPartial(int index, int policyNumber = 1)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicyType = "Motor";

            var basic = new PolicyBasicDetailsViewModel { InsuranceTypeID = 1 };
            basic.CompanySelectList = await GetCompanySelectListAsync(basic.InsuranceTypeID, basic.Company);
            var model = new MotorPolicyViewModel
            {
                BasicDetails = basic,
                VehicleDetails = new PolicyVehicleDetailsViewModel()
            };
            return PartialView("_MotorPolicyPartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetHealthPolicyPartial(int index, int policyNumber = 1)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicyType = "Health";

            var basic = new PolicyBasicDetailsViewModel { InsuranceTypeID = 2 };
            basic.CompanySelectList = await GetCompanySelectListAsync(basic.InsuranceTypeID, basic.Company);
            var model = new HealthPolicyViewModel { BasicDetails = basic };
            return PartialView("_HealthPolicyPartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetLifePolicyPartial(int index, int policyNumber = 1)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicyType = "Life";

            var basic = new PolicyBasicDetailsViewModel { InsuranceTypeID = 3 };
            basic.CompanySelectList = await GetCompanySelectListAsync(basic.InsuranceTypeID, basic.Company);
            var model = new LifePolicyViewModel { BasicDetails = basic };
            return PartialView("_LifePolicyPartial", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetPersonalAccidentPolicyPartial(int index, int policyNumber = 1)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicyType = "PersonalAccident";

            var basic = new PolicyBasicDetailsViewModel { InsuranceTypeID = 4 };
            basic.CompanySelectList = await GetCompanySelectListAsync(basic.InsuranceTypeID, basic.Company);
            var model = new PersonalAccidentPolicyViewModel { BasicDetails = basic };
            return PartialView("_PersonalAccidentPolicyPartial", model);
        }

        #endregion

        private async Task<CustomerEntity?> SaveOrUpdateCustomer(CustomerViewModel model)
        {
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
                        InsuranceTypeID = 1,
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
                        InsuranceTypeID = 2,
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
                        InsuranceTypeID = 3,
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
                        InsuranceTypeID = 4,
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

        private async Task<PolicyDetailsViewModel> BuildPolicyDetailsViewModelAsync(int customerId)
        {
            var model = new PolicyDetailsViewModel { CustomerID = customerId };

            var policies = await _customerService.GetPolicyDetailsByCustomerAsync(customerId);
            if (policies == null || policies.Count == 0)
                return model;

            foreach (var policy in policies)
            {
                var basicDetails = _mapper.Map<PolicyBasicDetailsViewModel>(policy);
                var payment = await _customerService.GetPolicyPaymentsByPollicyAsync(policy.PolicyId);
                var paymentVm = payment != null ? _mapper.Map<PolicyPaymentDetailsViewModel>(payment) : null;

                basicDetails.CompanySelectList = await GetCompanySelectListAsync(basicDetails.InsuranceTypeID, basicDetails.Company);

                switch (policy.InsuranceTypeID)
                {
                    case 1: // Motor Insurance
                        var vehicle = await _customerService.GetVehicleDetailsByPollicyAsync(policy.PolicyId);
                        var vehicleVm = vehicle != null ? _mapper.Map<PolicyVehicleDetailsViewModel>(vehicle) : null;
                        model.MotorPolicies.Add(new MotorPolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            VehicleDetails = vehicleVm,
                            PolicyPaymentDetails = paymentVm
                        });
                        break;
                    case 2: // Health Insurance
                        model.HealthPolicies.Add(new HealthPolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        });
                        break;
                    case 3: // Life Insurance
                        model.LifePolicies.Add(new LifePolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        });
                        break;
                    case 4: // Personal Accident
                        model.PersonalAccidentPolicies.Add(new PersonalAccidentPolicyViewModel
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

        private async Task RepopulatePolicyDropdownsAsync(PolicyDetailsViewModel model)
        {
            if (model.MotorPolicies != null)
            {
                foreach (var p in model.MotorPolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID > 0 ? p.BasicDetails.InsuranceTypeID : 1;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
            if (model.HealthPolicies != null)
            {
                foreach (var p in model.HealthPolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID > 0 ? p.BasicDetails.InsuranceTypeID : 2;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
            if (model.LifePolicies != null)
            {
                foreach (var p in model.LifePolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID > 0 ? p.BasicDetails.InsuranceTypeID : 3;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
            if (model.PersonalAccidentPolicies != null)
            {
                foreach (var p in model.PersonalAccidentPolicies)
                {
                    if (p.BasicDetails == null) continue;
                    var typeId = p.BasicDetails.InsuranceTypeID > 0 ? p.BasicDetails.InsuranceTypeID : 4;
                    p.BasicDetails.CompanySelectList = await GetCompanySelectListAsync(typeId, p.BasicDetails.Company);
                }
            }
        }
    }
}
