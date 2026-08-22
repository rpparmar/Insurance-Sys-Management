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

        /// <summary>Full name for header display; set server-side from customer record.</summary>
        public string? CustomerDisplayName { get; set; }

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

        /// <summary>
        /// Additional types (travel, fire, etc.) using the standard common + payment partial.
        /// </summary>
        public List<StandardPolicyViewModel> StandardPolicies { get; set; } = new List<StandardPolicyViewModel>();

        /// <summary>
        /// Insurance types from master for the selector grid (includes inactive rows).
        /// </summary>
        public List<InsuranceTypePolicyGridItemViewModel> InsuranceTypesForGrid { get; set; } = new List<InsuranceTypePolicyGridItemViewModel>();

        /// <summary>
        /// Indicates if any insurance types are configured in master (active or inactive).
        /// </summary>
        public bool HasAnyInsuranceTypes => InsuranceTypesForGrid != null && InsuranceTypesForGrid.Count > 0;

        /// <summary>
        /// Indicates if at least one insurance type is active and available for adding new policies.
        /// </summary>
        public bool HasActiveInsuranceTypes => InsuranceTypesForGrid != null && InsuranceTypesForGrid.Any(x => x.IsActive);

        /// <summary>
        /// Indicates if there are any existing policies currently associated with the customer.
        /// </summary>
        public bool HasAnyExistingPolicies =>
            (MotorPolicies != null && MotorPolicies.Count > 0) ||
            (HealthPolicies != null && HealthPolicies.Count > 0) ||
            (LifePolicies != null && LifePolicies.Count > 0) ||
            (PersonalAccidentPolicies != null && PersonalAccidentPolicies.Count > 0) ||
            (StandardPolicies != null && StandardPolicies.Count > 0);

        #endregion
    }
}
