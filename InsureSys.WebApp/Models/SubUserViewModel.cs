using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    public sealed class SubUserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Enter username")]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9_@]+$", ErrorMessage = "Use only letters, numbers, underscore (_), and at-sign (@). No spaces.")]
        [Remote(action: "IsUserNameAvailable", controller: "SubUsers", AdditionalFields = nameof(UserId))]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Select role")]
        [Range(3, int.MaxValue, ErrorMessage = "Invalid role selection.")]
        public int Role { get; set; }

        [Required(ErrorMessage = "Enter first name")]
        [MaxLength(25)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(25)]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Enter last name")]
        [MaxLength(25)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(256)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter password")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Re-enter password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsEditMode { get; set; }
    }
}

