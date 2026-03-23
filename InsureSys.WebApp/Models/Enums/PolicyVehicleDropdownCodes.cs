using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models.Enums
{
    /// <summary>Stored vehicle fuel codes (DB / form values).</summary>
    public enum PolicyVehicleFuelCode
    {
        [Display(Name = "Petrol")]
        PT,

        [Display(Name = "Diesel")]
        DS,

        [Display(Name = "Petrol/CNG")]
        PT_CNG
    }

    /// <summary>Stored motor plan type codes (DB / form values).</summary>
    public enum PolicyVehiclePlanTypeCode
    {
        [Display(Name = "FULL")]
        FULL,

        [Display(Name = "T.P")]
        TP,

        [Display(Name = "Own Damage")]
        OF,

        [Display(Name = "Comprehensive")]
        COMP
    }
}
