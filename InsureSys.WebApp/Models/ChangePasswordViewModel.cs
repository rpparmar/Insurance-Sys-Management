using InsuranceSys.Domain;
using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models
{
    /// <summary>
    /// Form model for an authenticated user to change their own password.
    /// </summary>
    public sealed class ChangePasswordViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Enter current password")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter new password")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must include uppercase, lowercase, number, and special character")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Re-enter new password")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Ensures the new password is not the same as the current password.
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(OldPassword)
                && !string.IsNullOrEmpty(NewPassword)
                && string.Equals(OldPassword, NewPassword, StringComparison.Ordinal))
            {
                yield return new ValidationResult(
                    Constants.ErrorMessages.MsgNewPasswordSameAsOld,
                    new[] { nameof(NewPassword) });
            }
        }
    }
}
