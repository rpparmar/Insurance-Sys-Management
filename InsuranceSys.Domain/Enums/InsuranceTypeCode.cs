using System.ComponentModel.DataAnnotations;

namespace InsuranceSys.Domain.Enums
{
    public enum InsuranceTypeCode : int
    {
        [Display(Name = "Motor / Vehicle Insurance")]
        MotorVehicle = 1,

        [Display(Name = "Health Insurance")]
        Health = 2,

        [Display(Name = "Life Insurance")]
        Life = 3,

        [Display(Name = "Personal Accident Insurance")]
        PersonalAccident = 4,

        [Display(Name = "Travel Insurance")]
        Travel = 5,

        [Display(Name = "Home / Property Insurance")]
        HomeProperty = 6,

        [Display(Name = "Fire Insurance")]
        Fire = 7,

        [Display(Name = "Marine Insurance")]
        Marine = 8,

        [Display(Name = "Commercial / Corporate Insurance")]
        Commercial = 9,

        [Display(Name = "Liability Insurance")]
        Liability = 10,

        [Display(Name = "Term Insurance")]
        TermLife = 11
    }
}
