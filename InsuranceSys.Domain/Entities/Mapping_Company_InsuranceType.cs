using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.Entities
{
    public class Mapping_Company_InsuranceType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MappingId { get; set; }
        public int InsuranceTypeId { get; set; }
        public int CompanyID { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }        
        public bool IsActive { get; set; }

    }
}
