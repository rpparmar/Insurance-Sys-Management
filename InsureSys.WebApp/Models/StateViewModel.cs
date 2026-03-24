using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class StateViewModel
    {
        public int StateID { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public int CountryID { get; set; }

        [Required(ErrorMessage = "State name is required")]
        [Remote(action: "IsStateNameExist", controller: "State", AdditionalFields = nameof(CountryID))]
        public string StateName { get; set; } = string.Empty;

        [Required(ErrorMessage = "State code is required")]
        [Remote(action: "IsStateCodeExist", controller: "State", AdditionalFields = nameof(CountryID))]
        public string StateCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsEditMode { get; set; }
        public List<SelectListItem> CountrySelectList { get; set; } = new();
    }
}
