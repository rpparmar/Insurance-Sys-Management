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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.Metrics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
    public class CustomerController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICustomerService _customerService;
        public CustomerController(IMapper mapper, ICustomerService customerService)
        {
            _mapper = mapper;
            _customerService = customerService;
        }
        [Route("Customers")]
        public IActionResult ListOfCustomers()
        {
            return View("../Customer/ListOfCustomers");
        }
        [Route("Customer/Policies")]
        public async Task<IActionResult> ManagePolicies(int customerId = 0)
        {
            var model = new PolicyDetailsViewModel();

            if (customerId > 0)
            {
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

        //[HttpGet]
        //public IActionResult GetMotorPolicyPartial(int index, int policyNumber = 1)
        //{
        //    #region Old Working
        //    // Create an empty model for the partial view
        //    var model = new CustomerViewModel();

        //    // Pass index and policy number to the partial view
        //    ViewBag.Index = index;
        //    ViewBag.PolicyNumber = policyNumber;

        //    // Return partial view            
        //    return PartialView("_MotorPolicyDetailsPartial_working", model);
        //    #endregion
        //}

        [HttpGet("Customer/Add")]
        [HttpGet("Customer/Edit/{id}")]
        public async Task<ActionResult> AddEditCustomer(string id = "")
        {
            CustomerViewModel model = new();

            //if (id.HasValue && id.Value > 0)
            //{
            //    var customer = _context.Customers
            //        .Include(c => c.MotorPolicies)
            //        .FirstOrDefault(c => c.CustomerID == id.Value);

            //    if (customer != null)
            //    {
            //        model = MapCustomerToViewModel(customer);
            //    }
            //}

            model.IsEditMode = false;
            model.IsActive = true;
            return View("../Customer/AddEditCustomer", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCustomer(CustomerViewModel model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Message"] = "Please correct the errors and try again.";
                TempData["RowsAffected"] = "0";
                return View("AddEditCustomer", model);
            }

            //// Save or update customer
            var customer = await SaveOrUpdateCustomer(model);
            if (customer == null || customer.CustomerID <= 0)
            {
                TempData["Message"] = "Failed to save customer. Please try again.";
                TempData["RowsAffected"] = "0";
                return View("AddEditCustomer", model);
            }

            // Success message
            TempData["Message"] = "Customer saved successfully.";
            TempData["RowsAffected"] = "1";

            if (string.IsNullOrWhiteSpace(model.SubmitType))
                return RedirectToAction("ListOfCustomers");

            if (model.SubmitType.Equals("customeronly", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("ListOfCustomers");
            }
            if (model.SubmitType.Equals("customerwithpolicies", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("ManagePolicies", new { customerId = customer.CustomerID });
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
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Message"] = "Please correct the errors and try again.";
                TempData["RowsAffected"] = "0";
                return View("CustomerPolicies", model);
            }

            if (model.MotorPolicies != null && model.MotorPolicies.Count > 0)
            {
                await SaveMotorPoliciesAsync(model.CustomerID, model.MotorPolicies);
            }
            if (model.HealthPolicies != null && model.HealthPolicies.Count > 0)
            {
                await SaveHealthPoliciesAsync(customerId, model.HealthPolicies);
            }
            if (model.LifePolicies != null && model.LifePolicies.Count > 0)
            {
                await SaveLifePoliciesAsync(customerId, model.LifePolicies);
            }
            if (model.PersonalAccidentPolicies != null && model.PersonalAccidentPolicies.Count > 0)
            {
                await SavePersonalAccidentPoliciesAsync(customerId, model.PersonalAccidentPolicies);
            }
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

        /// <summary>
        /// Returns a partial view for a single motor policy form
        /// This is called via AJAX when adding a new policy
        /// </summary>
        /// <param name="index"></param>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMotorPolicyPartial(int index, int policyNumber = 1)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicyType = "Motor";

            var model = new MotorPolicyViewModel
            {
                BasicDetails = new PolicyBasicDetailsViewModel()
            };
            return PartialView("_MotorPolicyPartial", model);
        }

        /// <summary>
        /// Returns a partial view for a single health policy form
        /// This is called via AJAX when adding a new policy
        /// </summary>
        /// <param name="index"></param>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetHealthPolicyPartial(int index, int policyNumber = 1)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicyType = "Health";

            var model = new HealthPolicyViewModel
            {
                BasicDetails = new PolicyBasicDetailsViewModel()
            };
            return PartialView("_HealthPolicyPartial", model);
        }

        /// <summary>
        /// Returns a partial view for a single life policy form
        /// This is called via AJAX when adding a new policy
        /// </summary>
        /// <param name="index"></param>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetLifePolicyPartial(int index, int policyNumber = 1)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicyType = "Life";

            var model = new LifePolicyViewModel
            {
                BasicDetails = new PolicyBasicDetailsViewModel()
            };
            return PartialView("_LifePolicyPartial", model);
        }

        /// <summary>
        /// Returns a partial view for a single accident policy form
        /// This is called via AJAX when adding a new policy
        /// </summary>
        /// <param name="index"></param>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetPersonalAccidentPolicyPartial(int index, int policyNumber = 1)
        {
            ViewBag.Index = index;
            ViewBag.PolicyNumber = policyNumber;
            ViewBag.PolicyType = "PersonalAccident";

            var model = new PersonalAccidentPolicyViewModel
            {
                BasicDetails = new PolicyBasicDetailsViewModel()
            };
            return PartialView("_PersonalAccidentPolicyPartial", model);
        }

        #endregion

        private async Task<CustomerEntity> SaveOrUpdateCustomer(CustomerViewModel model)
        {

            var customer = _mapper.Map<CustomerEntity>(model);

            if (model.IsEditMode && model.CustomerID > 0)
            {
                //UpdateCustomerProperties(customer, model);
                //customer.UpdatedOn = DateTime.UtcNow;
            }
            else
            {
                customer.CustomerID = await _customerService.AddCustomer(customer);
            }
            return customer;
        }

        //private void UpdateCustomerProperties(CustomerEntity customer, CustomerViewModel model)
        //{
        //    customer.FirstName = model.FirstName;
        //    customer.LastName = model.LastName;
        //    customer.Phone = model.Phone;
        //    customer.Email = model.Email;
        //    customer.DOB = model.DOB;
        //    customer.AnniversaryDate = model.AnniversaryDate;
        //    customer.Gender = model.Gender;
        //    customer.AddressLine1 = model.AddressLine1;
        //    customer.AddressLine2 = model.AddressLine2;
        //    customer.Country = model.Country;
        //    customer.State = model.State;
        //    customer.City = model.City;
        //    customer.ZipCode = model.ZipCode;
        //}

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

        private async Task SaveVehicleDetailsAsync(int PolicyId, PolicyVehicleDetailsViewModel? vehicleDetailsViewModel)
        {
            if (vehicleDetailsViewModel != null)
            {
                var policyVehicleEntity = _mapper.Map<PolicyVehicleDetailsEntity>(vehicleDetailsViewModel);
                if (PolicyId > 0 && policyVehicleEntity != null)
                {
                    policyVehicleEntity.PolicyId = PolicyId;
                    var existing = await _customerService.GetVehicleDetailsByPollicyAsync(PolicyId);
                    if (existing != null)
                    {
                        await _customerService.UpdateVehicleDetailsAsync(policyVehicleEntity);
                    }
                    else
                    {
                        await _customerService.AddVehicleDetails(policyVehicleEntity);
                    }
                }
            }

        }

        private async Task SavePolicyPaymentDetailsAsync(int policyId, PolicyPaymentDetailsViewModel? paymentDetailsViewModel)
        {
            if (paymentDetailsViewModel != null)
            {
                var paymentEntity = _mapper.Map<PolicyPaymentDetailsEntity>(paymentDetailsViewModel);
                if (policyId > 0 && paymentEntity != null)
                {
                    paymentEntity.PolicyId = policyId;
                    var existing = await _customerService.GetPolicyPaymentsByPollicyAsync(policyId);
                    if (existing != null)
                    {
                        await _customerService.UpdatePolicyPaymentDetailsAsync(paymentEntity);
                    }
                    else
                    {
                        await _customerService.AddPolicyPaymentDetails(paymentEntity);
                    }
                }
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
                {
                    await SavePolicyPaymentDetailsAsync(policyEntity.PolicyId, healthPolicy.PolicyPaymentDetails);
                }
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
                {
                    await SavePolicyPaymentDetailsAsync(policyEntity.PolicyId, lifePolicy.PolicyPaymentDetails);
                }
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
                {
                    await SavePolicyPaymentDetailsAsync(policyEntity.PolicyId, paPolicy.PolicyPaymentDetails);
                }
            }
        }

        private async Task<PolicyDetailsViewModel> BuildPolicyDetailsViewModelAsync(int customerId)
        {
            var model = new PolicyDetailsViewModel
            {
                CustomerID = customerId
            };

            var policies = await _customerService.GetPolicyDetailsByCustomerAsync(customerId);
            if (policies == null || policies.Count == 0)
            {
                return model;
            }



            foreach (var policy in policies)
            {
                var basicDetails = _mapper.Map<PolicyBasicDetailsViewModel>(policy);
                var payment = await _customerService.GetPolicyPaymentsByPollicyAsync(policy.PolicyId);
                var paymentVm = payment != null
                    ? _mapper.Map<PolicyPaymentDetailsViewModel>(payment)
                    : null;

                switch (policy.InsuranceTypeID)
                {
                    case 1: // Motor Insurance
                        var vehicle = await _customerService.GetVehicleDetailsByPollicyAsync(policy.PolicyId);
                        var vehicleVm = vehicle != null
                    ? _mapper.Map<PolicyVehicleDetailsViewModel>(vehicle)
                    : null;
                        var motorVm = new MotorPolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            VehicleDetails = vehicleVm,
                            PolicyPaymentDetails = paymentVm
                        };
                        model.MotorPolicies.Add(motorVm);
                        break;
                    case 2: // Health Insurance
                        var healthVm = new HealthPolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        };
                        model.HealthPolicies.Add(healthVm);
                        break;
                    case 3: // Term / Life Insurance
                        var lifeVm = new LifePolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        };
                        model.LifePolicies.Add(lifeVm);
                        break;
                    case 4: // Personal Accident
                        var paVm = new PersonalAccidentPolicyViewModel
                        {
                            BasicDetails = basicDetails,
                            PolicyPaymentDetails = paymentVm
                        };
                        model.PersonalAccidentPolicies.Add(paVm);
                        break;
                }
            }

            return model;
        }

    }
}
