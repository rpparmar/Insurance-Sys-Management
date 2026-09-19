using System.ComponentModel.DataAnnotations;

namespace Insurancesys.web.Models;

/// <summary>
/// SMTP and sender fields displayed on the Settings page.
/// Persistence is deferred to a later phase.
/// </summary>
public sealed class EmailSettingsViewModel
{
    [Display(Name = "SMTP Server")]
    [StringLength(255)]
    public string SmtpHost { get; set; } = string.Empty;

    [Display(Name = "SMTP Server Port")]
    [Range(1, 65535, ErrorMessage = "Enter a port between 1 and 65535")]
    public int? SmtpPort { get; set; }

    [Display(Name = "User Name")]
    [StringLength(256)]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "App Password")]
    [DataType(DataType.Password)]
    [StringLength(40)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Connection Timeout (in seconds)")]
    [Range(1, 300, ErrorMessage = "Enter a timeout between 1 and 300 seconds")]
    public int? ConnectionTimeoutSeconds { get; set; }

    [Display(Name = "Domain")]
    [StringLength(255)]
    public string Domain { get; set; } = string.Empty;

    [Display(Name = "Use SSL")]
    public bool UseSsl { get; set; }

    [Display(Name = "Send As Address")]
    [EmailAddress(ErrorMessage = "Enter a valid send-as email address")]
    [StringLength(256)]
    public string SendAsAddress { get; set; } = string.Empty;

    [Display(Name = "Reporting Email Address")]
    [StringLength(500)]
    public string ReportingEmailAddress { get; set; } = string.Empty;
}
