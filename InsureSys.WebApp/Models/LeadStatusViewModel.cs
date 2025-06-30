using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class LeadStatusViewModel
    {
        public int LeadStatusID { get; set; }
        [Required(ErrorMessage = "Enter lead status")]
        [Remote(action: "IsLeadStatusExist", controller: "LeadStatus")]
        public string LeadStatus { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsEditMode { get; set; }
    }
}
