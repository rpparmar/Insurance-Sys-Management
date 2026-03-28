using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.Models
{
    public class PolicyBasicDetailsViewModel
    {
        public int PolicyId { get; set; }
        public int CustomerID { get; set; }
        public int InsuranceTypeID { get; set; }

        [Required(ErrorMessage ="Enter policy number")]
        public string PolicyNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Select policy start date ")]
        public DateTime PolicyStartDate { get; set; }

        [Required(ErrorMessage = "Select policy due date ")]
        public DateTime PolicyDueDate { get; set; }

        [Required(ErrorMessage = "Select insurer")]
        public string Company { get; set; } = string.Empty;

        /// <summary>Populated server-side for Insurer dropdown; not posted.</summary>
        [BindNever]
        public List<SelectListItem>? CompanySelectList { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0.01, 10000000.00)]
        public decimal? GrosssPremium { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 10000000.00)]
        public decimal? NetPremium { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 10000000.00)]
        public decimal? ODPremium { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 10000000.00)]
        public decimal? NCB { get; set; }        
        public string? Dealer { get; set; }
        public string? SM { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
