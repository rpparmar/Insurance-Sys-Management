using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.Entities
{
	public class UsersEntity
	{
		[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public short? Role { get; set; }
		public string? FirstName { get; set; }
		public string? MiddleName { get; set; }
		public string? LastName { get; set; }
		public string? Phone { get; set; }
		public string? Password { get; set; }
		public bool? IsActive { get; set; }
		public string? Address1 { get; set; }
		public string? Address2 { get; set; }
		public string? Address3 { get; set; }
		public int? CountryID { get; set; }
		public int? StateID { get; set; }
		public string? PostalCode { get; set; }
		public DateTime? CreatedOn { get; set; }
		public DateTime? UpdatedOn { get; set; }
		public int? ParentUserID { get; set; }
		public bool? IsParentUser { get; set; }
		public string? ProfilePicPath { get; set; }
        public int? AgencyID { get; set; }
    }
}
