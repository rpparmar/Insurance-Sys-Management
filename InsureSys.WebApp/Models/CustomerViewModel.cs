using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Models
{
    public class CustomerViewModel
    {
        public int CustomerID { get; set; }
        public int? LeadID { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool SmsReminderEnabled { get; set; } = true;
        public bool EmailReminderEnabled { get; set; } = true;
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string SubmitType { get; set; } = "Customer"; // "Customer" or "Full"

        #region Customer Details
        [Required(ErrorMessage = "Enter first name")]
        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^[6-9][0-9]{9}$", ErrorMessage = "Please enter a valid 10-digit mobile number")]
        [Display(Name = "Mobile Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be exactly 10 digits")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AnniversaryDate { get; set; }

        public string? Gender { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public int? CountryID { get; set; }
        public int? StateID { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public List<SelectListItem> CountrySelectList { get; set; } = new();
        public List<SelectListItem> StateSelectList { get; set; } = new();
        #endregion

        #region Policies
        /// <summary>
        /// Motor Policies
        /// </summary>
        public List<MotorPolicyViewModel> MotorPolicies2 { get; set; } = new List<MotorPolicyViewModel>();
        /// <summary>
        /// Health Insurance Policies
        /// </summary>
        public List<HealthPolicyViewModel> HealthPolicies2 { get; set; } = new List<HealthPolicyViewModel>();

        /// <summary>
        /// Life Insurance Policies
        /// </summary>
        public List<LifePolicyViewModel> LifePolicies2 { get; set; } = new List<LifePolicyViewModel>();

        /// <summary>
        /// Personal Accident Insurance Policies
        /// </summary>
        public List<PersonalAccidentPolicyViewModel> PersonalAccidentPolicies2 { get; set; } = new List<PersonalAccidentPolicyViewModel>();

        #endregion

        //// Helper method to get policy field names for validation removal
        //public static string[] GetPolicyFieldNames()
        //{
        //    return new[]
        //    {
        //        "PolicyNumber", "PolicyStartDate", "PolicyDueDate", "Company",
        //        "GrosssPremium", "NetPremium", "ODPremium", "NCB",
        //        "Dealer", "SM", "Vehicleno", "Make", "VehicleModel",
        //        "Chassiseno", "Segment", "Fuel", "VehicleIDV", "PlanType",
        //        "PaymentMode", "Transactionreferance", "BankName"
        //    };
        //}
    }

}
