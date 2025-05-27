using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.DTO
{
    public class LeadDto
    {
        public int LeadID { get; set; }
        #region Basic Info
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime? InquiryDate { get; set; }
        #endregion
        public DateTime? NextFollowUpDate { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string LeadSource { get; set; }
        #region Dropdown
        public string CompanyID { get; set; }
        public string PolicyTypeID { get; set; }
        public string LeadStatus { get; set; }
        public string AssignedTo { get; set; }
        #endregion
    }
}
