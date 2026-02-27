using System.ComponentModel.DataAnnotations;
using static InsuranceSys.Domain.Constants;

namespace Insurancesys.web.Models
{
    /// <summary>
    /// ViewModel for policies details of customer
    /// </summary>
    public class PolicyDetailsViewModel
    {
        //#region Old working
        //public int PolicyId { get; set; }
        //public int CustomerID { get; set; }

        //[Required(ErrorMessage = "Enter policy number")]
        //public string PolicyNumber { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Select policy start date")]
        //public DateTime? PolicyStartDate { get; set; }

        //[Required(ErrorMessage = "Select policy due date")]
        //public DateTime? PolicyDueDate { get; set; }

        //[Required(ErrorMessage = "Select insurer")]
        //public string Company { get; set; } = string.Empty;

        //public string? PlanType { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Enter gross premium")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid amount entered")]
        //[Range(0.01, 10000000.00, ErrorMessage = "Premium must be greater than 0")]
        //public decimal? GrosssPremium { get; set; }

        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid amount entered")]
        //[Range(0, 10000000.00)]
        //public decimal? NetPremium { get; set; }

        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid amount entered")]
        //[Range(0, 10000000.00)]
        //public decimal? ODPremium { get; set; }

        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Invalid amount entered")]
        //[Range(0, 10000000.00)]
        //public decimal? NCB { get; set; }

        //public string? Dealer { get; set; }
        //public string? SM { get; set; }

        //public bool IsActive { get; set; }
        //public bool IsDeleted { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public DateTime? UpdatedOn { get; set; }

        ////// Vehicle Details
        //public PolicyVehicleDetailsViewModel? VehicleDetails { get; set; }

        ////// Payment Details
        //public PolicyPaymentDetailsViewModel? PolicyPaymentDetails { get; set; }

        //#endregion
        public int CustomerID { get; set; }
        #region Policies
        /// <summary>
        /// Motor Policies
        /// </summary>
        public List<MotorPolicyViewModel> MotorPolicies { get; set; } = new List<MotorPolicyViewModel>();
        /// <summary>
        /// Health Insurance Policies
        /// </summary>
        public List<HealthPolicyViewModel> HealthPolicies { get; set; } = new List<HealthPolicyViewModel>();

        /// <summary>
        /// Life Insurance Policies
        /// </summary>
        public List<LifePolicyViewModel> LifePolicies { get; set; } = new List<LifePolicyViewModel>();

        /// <summary>
        /// Personal Accident Insurance Policies
        /// </summary>
        public List<PersonalAccidentPolicyViewModel> PersonalAccidentPolicies { get; set; } = new List<PersonalAccidentPolicyViewModel>();

        #endregion
    }
}
