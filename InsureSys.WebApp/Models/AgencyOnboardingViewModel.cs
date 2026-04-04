using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class AgencyOnboardingViewModel
    {
        [Required(ErrorMessage = "Agency code is required")]
        [MaxLength(50)]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Only letters, numbers, and underscores allowed")]
        [Remote(action: "IsAgencyCodeAvailable", controller: "AgencyOnboarding")]
        [Display(Name = "Agency Code")]
        public string AgencyCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Agency/Agent name is required")]
        [MaxLength(200)]
        [Display(Name = "Agency/Agent Name")]
        public string AgencyName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(256)]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        [MaxLength(20)]
        [Display(Name = "Contact Phone")]
        public string? ContactPhone { get; set; }

        [Required(ErrorMessage = "Database name is required")]
        [MaxLength(128)]
        [RegularExpression(@"^[A-Za-z0-9_]+$", ErrorMessage = "Only letters, numbers, and underscores allowed")]
        [Remote(action: "IsDatabaseNameAvailable", controller: "AgencyOnboarding")]
        [Display(Name = "Database Name")]
        public string DesiredDatabaseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admin username is required")]
        [MaxLength(100)]
        [Display(Name = "Admin Username")]
        public string AdminUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admin password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Admin Password")]
        public string AdminPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm the password")]
        [Compare(nameof(AdminPassword), ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}
