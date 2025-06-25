using Microsoft.AspNetCore.Mvc;
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
        public string? InsuranceType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsActive { get; set; }
        public bool IsEditMode { get; set; }
        public string? AssociationWithCompanyIDs { get; set; }
        public List<SelectListItem> lstOfCompanies { get; set; }

    }
}
