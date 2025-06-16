using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class CompanyViewModel
    {        
        public int CompanyID { get; set; }
        [Required(ErrorMessage = "Company name is required")]
        [Remote(action: "IsCompanyExist", controller: "Company")]        
        public string? CompanyName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsEditMode { get; set; }
    }
}
