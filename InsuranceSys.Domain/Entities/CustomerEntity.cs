using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.Entities
{
    public class CustomerEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }
        public int? LeadID { get; set; }

        #region Customer Details        
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? AnniversaryDate { get; set; }
        public string? Gender { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        #endregion
       
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }        
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}
