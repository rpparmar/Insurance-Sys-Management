using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public class AgencyOnboardingViewModel
    {
        [MaxLength(50)]
        [RegularExpression(@"^$|^[A-Za-z0-9_]+$", ErrorMessage = "Only letters, numbers, and underscores allowed")]
        [Remote(action: "IsAgencyCodeAvailable", controller: "AgencyOnboarding")]        
        public string? AgencyCode { get; set; }

        [Required(ErrorMessage = "Enter agency/ageny name")]
        [Remote(action: "IsAgencyNameAvailable", controller: "AgencyOnboarding")]        
        public string AgencyName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]        
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }
                
        public string? ContactPhone { get; set; }

        [Required(ErrorMessage = "Enter username")]        
        [RegularExpression(@"^[a-zA-Z0-9_@]+$", ErrorMessage = "Use only letters, numbers, underscore (_), and at-sign (@). No spaces.")]
        [Remote(action: "IsAdminUsernameAvailable", controller: "AgencyOnboarding")]        
        public string AdminUsername { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]        
        public string AdminPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm the password")]
        [Compare(nameof(AdminPassword), ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
