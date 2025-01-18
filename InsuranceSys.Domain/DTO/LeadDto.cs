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
        public DateTime InquiryDate { get; set; }
        #endregion
        public int CompanyID { get; set; }
        public int PolicyTypeID { get; set; }
        public int Status { get; set; }
        public string LeadSource { get; set; }
        public int AssignedTo { get; set; }
        public DateTime NextFollowUpDate { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
