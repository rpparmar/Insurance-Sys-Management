using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class CountryViewModel
    {
        public int CountryID { get; set; }

        [Required(ErrorMessage = "Country name is required")]
        [Remote(action: "IsCountryNameExist", controller: "Country")]
        public string CountryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country code is required")]
        [Remote(action: "IsCountryCodeExist", controller: "Country")]
        public string CountryCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsEditMode { get; set; }
    }
}
