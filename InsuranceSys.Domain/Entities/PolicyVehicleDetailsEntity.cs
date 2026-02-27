using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Domain.Entities
{
    public class PolicyVehicleDetailsEntity
    {
        // Vehicle Details
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VehicleId { get; set; }
        public int PolicyId { get; set; }
        public string? Vehicleno { get; set; }
        public string? Make { get; set; }
        public string? VehicleModel { get; set; }
        public string? Chassiseno { get; set; }
        public string? Segment { get; set; }
        public string? Fuel { get; set; }
        public string? PlanType { get; set; } = string.Empty;
        public string? VehicleIDV { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
