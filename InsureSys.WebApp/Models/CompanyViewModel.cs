using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class CompanyViewModel
    {
        public int CompanyID { get; set; }
        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
