using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class InsuranceTypeViewModel
    {
        public InsuranceTypeViewModel()
        {
            lstOfCompanies = new();
        }
        public int InsuranceTypeId { get; set; }

        [Required(ErrorMessage = "Insurance type is required")]
        [Remote(action: "IsInsurancetypeExist", controller: "InsuranceType")]
        public string InsuranceType { get; set; } = string.Empty;

        /// <summary>Form-template key persisted on the master row.</summary>
        [Required(ErrorMessage = "Form template is required")]
        [MaxLength(64)]
        public string InsuranceTypeCode { get; set; } = "Standard";

        /// <summary>CSS icon class (FontAwesome / Flaticon).</summary>
        [MaxLength(128)]
        public string IconClass { get; set; } = string.Empty;

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public bool IsEditMode { get; set; }
        public string? AssociationWithCompanyIDs { get; set; }
        public List<SelectListItem> lstOfCompanies { get; set; }

        [BindNever]
        public List<SelectListItem> FormTemplateSelectList { get; set; } = [];

        [BindNever]
        public List<SelectListItem> IconSelectList { get; set; } = [];
    }
}
