using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class LeadViewModel
    {
        public LeadViewModel()
        {
            lstCompanies = new();
            lstInsuranceType = new();
            lstUsers = new();
            lstLeadStatus = new();
        }
        public int LeadID { get; set; }
        #region Basic Info
        [Required(ErrorMessage = "Enter first name")]
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Enter phone number")]
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public DateTime? InquiryDate { get; set; }
        #endregion
        public DateTime? NextFollowUpDate { get; set; }
        public string? Notes { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string? LeadSource { get; set; }        
        #region Dropdown
        public string? CompanyID { get; set; }
        public List<SelectListItem> lstCompanies { get; set; }
        [Required(ErrorMessage = "Select type of insurance")]
        public string PolicyTypeID { get; set; } = string.Empty;
        public List<SelectListItem> lstInsuranceType { get; set; }
        public string? LeadStatus { get; set; }
        public List<SelectListItem> lstLeadStatus{ get; set; }
        [Required(ErrorMessage = "Select any one assignee")]
        public string AssignedTo { get; set; } = string.Empty;
        public List<SelectListItem> lstUsers { get; set; }
        #endregion
    }
}
