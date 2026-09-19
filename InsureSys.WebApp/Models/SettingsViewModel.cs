using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models;

/// <summary>
/// Root view model for the Settings page, grouping email configuration and test-send fields.
/// </summary>
public sealed class SettingsViewModel
{
    /// <summary>
    /// SMTP configuration bound to the Email Settings section.
    /// </summary>
    public EmailSettingsViewModel EmailSettings { get; set; } = new();

    /// <summary>
    /// Recipient used by the Test Email Configuration action (not persisted in Phase 1).
    /// </summary>
    [Display(Name = "To Email Address")]
    [EmailAddress(ErrorMessage = "Enter a valid recipient email address")]
    [StringLength(256)]
    public string TestRecipientEmail { get; set; } = string.Empty;
}
