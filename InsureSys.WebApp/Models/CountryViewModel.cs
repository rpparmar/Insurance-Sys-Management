using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class CountryViewModel
    {
        public int CountryID { get; set; }
        [Required(ErrorMessage = "Country name is required")]
        [Remote(action: "IsCountryExist", controller: "Country")]
        [StringLength(100)]
        public string CountryName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsEditMode { get; set; }
    }
}
