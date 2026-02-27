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
        public IActionResult ManagePolicies(int customerId = 0)
        {
            return View("../Customer/CustomerPolicies");
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
                Console.WriteLine($"Saving {model.HealthPolicies.Count} health policies...");
                foreach (var paymentDetails in model.HealthPolicies)
                {
                    if (paymentDetails.PolicyPaymentDetails != null)
                    {
                        Console.WriteLine($"health policies at {model.HealthPolicies.IndexOf(paymentDetails)} contains payment details");
                    }
                }
            }
            if (model.LifePolicies != null && model.LifePolicies.Count > 0)
            {
                Console.WriteLine($"Saving {model.LifePolicies.Count} life policies...");
                foreach (var paymentDetails in model.LifePolicies)
                {
                    if (paymentDetails.PolicyPaymentDetails != null)
                    {
                        Console.WriteLine($"life policies at {model.LifePolicies.IndexOf(paymentDetails)} contains payment details");
                    }
                }
            }
            if (model.PersonalAccidentPolicies != null && model.PersonalAccidentPolicies.Count > 0)
            {
                Console.WriteLine($"Saving {model.PersonalAccidentPolicies.Count} personal accident policies...");
                foreach (var paymentDetails in model.PersonalAccidentPolicies)
                {
                    if (paymentDetails.PolicyPaymentDetails != null)
                    {
                        Console.WriteLine($"personal accident policies at {model.PersonalAccidentPolicies.IndexOf(paymentDetails)} contains payment details");
                    }
                }
            }
            return RedirectToAction("ListOfCustomers");
        }

        #region AJAX calls

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

            var model = new MotorPolicyViewModel();
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

            var model = new HealthPolicyViewModel();
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

            var model = new LifePolicyViewModel();
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

            var model = new PersonalAccidentPolicyViewModel();
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
                //customer.CustomerID = await _customerService.AddCustomer(customer);                                
                customer.CustomerID = 1;                                
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
                var motorPolicy = new MotorPolicyViewModel
                {
                    BasicDetails = new PolicyBasicDetailsViewModel
                    {
                        CustomerID = customerId,
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

                    }
                };

                var motorPolicyEntity = _mapper.Map<PolicyDetailsEntity>(motorPolicy.BasicDetails);
                motorPolicyEntity.PolicyId = await _customerService.AddPolicyDetails(motorPolicyEntity);
                if (motorPolicyEntity.PolicyId > 0)
                {                    
                    await SaveVehicleDetailsAsync(motorPolicyEntity.PolicyId, motorPolicy.VehicleDetails);
                }
                //var policyPaymentEntity = _mapper.Map<PolicyPaymentDetailsEntity>(policy.PolicyPaymentDetails);
                //if (motorPolicyEntity.PolicyId > 0 && policyPaymentEntity != null)
                //{

                //}

            }
        }

        private async Task SaveVehicleDetailsAsync(int PolicyId, PolicyVehicleDetailsViewModel? vehicleDetailsViewModel)
        {
            if (vehicleDetailsViewModel != null)
            {
                var policyVehicleEntity = _mapper.Map<PolicyVehicleDetailsEntity>(vehicleDetailsViewModel);
                if (PolicyId > 0 && policyVehicleEntity != null)
                {

                }
            }
            
        }

    }
}
