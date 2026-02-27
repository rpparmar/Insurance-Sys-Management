using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InsuranceSys.Domain.Constants;

namespace InsuranceSys.Domain.Entities
{
    public class PolicyDetailsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("PolicyID")]
        public int PolicyId { get; set; }
        public int CustomerID { get; set; }
        public int InsuranceTypeID { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;        
        public DateTime? PolicyStartDate { get; set; }        
        public DateTime? PolicyDueDate { get; set; }        
        public string Company { get; set; } = string.Empty;               
        public decimal? GrosssPremium { get; set; }        
        public decimal? NetPremium { get; set; }        
        public decimal? ODPremium { get; set; }        
        public decimal? NCB { get; set; }
        public string? Dealer { get; set; }
        public string? SM { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

    }
}
