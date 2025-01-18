using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class LeadViewModel
    {
        public LeadViewModel()
        {
            lstCompanies = new List<SelectListItem>();
            lstInsuranceType = new List<SelectListItem>();
            lstUsers = new List<SelectListItem>();
        }
        public int LeadID { get; set; }
        #region Basic Info
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime InquiryDate { get; set; }
        #endregion
        public DateTime NextFollowUpDate { get; set; }
        public string Notes { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string LeadSource { get; set; }

        #region Dropdown
        public string CompanyID { get; set; }
        public List<SelectListItem> lstCompanies { get; set; }
        //[Required(ErrorMessage = "select type of insurance")]
        public string PolicyTypeID { get; set; }
        public List<SelectListItem> lstInsuranceType { get; set; }
        public string LeadStatus { get; set; }
        public List<SelectListItem> lstLeadStatus{ get; set; }
        //[Required(ErrorMessage = "select any one assignee")]
        public string AssignedTo { get; set; }
        public List<SelectListItem> lstUsers { get; set; }
        #endregion
    }
}
