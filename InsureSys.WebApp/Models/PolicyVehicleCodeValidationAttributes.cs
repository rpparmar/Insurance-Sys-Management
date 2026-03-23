using System.ComponentModel.DataAnnotations;
using Insurancesys.web.Helper;

namespace Insurancesys.web.Models
{
    public sealed class VehicleFuelCodeAttribute : ValidationAttribute
    {
        public VehicleFuelCodeAttribute() : base("Invalid fuel type.") { }

        public override bool IsValid(object? value)
        {
            return value is not string s || VehiclePolicyDropdowns.IsValidFuelCode(s);
        }
    }

    public sealed class VehiclePlanTypeCodeAttribute : ValidationAttribute
    {
        public VehiclePlanTypeCodeAttribute() : base("Invalid plan type.") { }

        public override bool IsValid(object? value)
        {
            return value is not string s || VehiclePolicyDropdowns.IsValidPlanTypeCode(s);
        }
    }
}
