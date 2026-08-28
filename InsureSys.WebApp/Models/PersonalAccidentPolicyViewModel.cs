using System.ComponentModel.DataAnnotations;
using static InsuranceSys.Domain.Constants;

namespace Insurancesys.web.Models
{
    /// <summary>
    /// ViewModel for individual policy
    /// </summary>
    public class PersonalAccidentPolicyViewModel : ICommonPolicyForm
    {
        #region Basic Details
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
        #endregion

        public PolicyBasicDetailsViewModel BasicDetails { get; set; }

        //// Payment Details
        public PolicyPaymentDetailsViewModel? PolicyPaymentDetails { get; set; }
        
    }
}
