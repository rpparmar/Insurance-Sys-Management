using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class AgencyOnboardingViewModel
    {
        public int AgencyId { get; set; }

        public bool IsEditMode { get; set; }

        [Display(Name = "Database")]
        public string? DatabaseNameDisplay { get; set; }

        [MaxLength(50)]
        [RegularExpression(@"^$|^[A-Za-z0-9_]+$", ErrorMessage = "Only letters, numbers, and underscores allowed")]
        [Remote(action: "IsAgencyCodeAvailable", controller: "AgencyOnboarding")]
        public string? AgencyCode { get; set; }

        [Required(ErrorMessage = "Enter agency/agent name")]
        [MaxLength(200)]
        [Remote(action: "IsAgencyNameAvailable", controller: "AgencyOnboarding")]
        [Display(Name = "Agency/Agent Name")]
        public string AgencyName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        [MaxLength(20)]
        [Display(Name = "Contact Phone")]
        public string? ContactPhone { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [RegularExpression(@"^[a-zA-Z0-9_@]+$", ErrorMessage = "Use only letters, numbers, underscore (_), and at-sign (@). No spaces.")]
        [Remote(action: "IsAdminUsernameAvailable", controller: "AgencyOnboarding")]
        [Required(ErrorMessage = "Enter username")]
        [Display(Name = "Admin Username")]
        public string? AdminUsername { get; set; }

        public bool IsAdminUsernameEditEnabled { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Admin Password")]
        public string? AdminPassword { get; set; }

        [Required(ErrorMessage = "Re-enter password")]
        [Compare(nameof(AdminPassword), ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
