using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class PolicyVehicleDetailsViewModel
    {
        // Vehicle Details
        public int VehicleId { get; set; }
        public string? Vehicleno { get; set; }
        public string? Make { get; set; }
        public string? VehicleModel { get; set; }
        public string? Chassiseno { get; set; }
        public string? Segment { get; set; }

        [VehicleFuelCode]
        public string? Fuel { get; set; }
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        [Range(0, 10000000.00)]
        public decimal? VehicleIDV { get; set; }

        [VehiclePlanTypeCode]
        public string? PlanType { get; set; }
    }
}
