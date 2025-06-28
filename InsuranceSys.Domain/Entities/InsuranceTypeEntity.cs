using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.Entities
{
    public class InsuranceTypeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceTypeId { get; set; }
        public required string InsuranceType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }=false;
        public bool IsActive { get; set; }=true;
        [NotMapped]
        public string? AssociationWithCompanyIDs { get; set; }
    }
}
