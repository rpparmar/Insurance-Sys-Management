using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.Entities
{
    public class LeadEntity
    {
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeadID { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Email { get; set; }
		public string? PhoneNumber { get; set; }
		public int? CompanyID { get; set; }
		public int? PolicyTypeID { get; set; }
		public DateTime? InquiryDate { get; set; }
		public short? LeadStatus { get; set; }
		public string? LeadSource { get; set; }
		public int? AssignedTo { get; set; }
		public DateTime? NextFollowUpDate { get; set; }
		public string? Notes { get; set; }
		public string? Address { get; set; }
		public bool? IsActive { get; set; }
		public bool? IsDeleted { get; set; }	
    }
}
