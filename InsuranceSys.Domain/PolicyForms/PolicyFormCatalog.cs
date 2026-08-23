using InsuranceSys.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSys.Domain.PolicyForms
{
    /// <summary>
    /// Static catalogs for insurance-type master (form templates and icon classes).
    /// </summary>
    public static class PolicyFormCatalog
    {
        public const string MotorVehicleKey = nameof(InsuranceTypeCode.MotorVehicle);
        public const string HealthKey = nameof(InsuranceTypeCode.Health);
        public const string LifeKey = nameof(InsuranceTypeCode.Life);
        public const string PersonalAccidentKey = nameof(InsuranceTypeCode.PersonalAccident);
        public const string StandardKey = nameof(InsuranceTypeCode.Standard);
        public const string DefaultIconClass = "fas fa-file-alt";

        private static readonly IReadOnlyList<(string Value, string Text)> TemplateOptions =
        [
            (MotorVehicleKey, "Motor Insurance Template"),
            (HealthKey, "Health Insurance Template"),
            (LifeKey, "Life Insurance Template"),
            (PersonalAccidentKey, "Personal Accident Insurance Template"),
            (StandardKey, "Default (Basic details)")
        ];

        private static readonly IReadOnlyList<(string Value, string Text)> IconOptions =
        [
            ("fas fa-car-side", "Car / Motor"),
            ("fas fa-heartbeat", "Health"),
            ("fas fa-shield-alt", "Life / Shield"),
            ("fas fa-ambulance", "Personal Accident"),
            ("fas fa-plane", "Travel"),
            ("fas fa-home", "Home / Property"),
            ("fas fa-fire", "Fire"),
            ("fas fa-ship", "Marine"),
            ("fas fa-building", "Commercial"),
            ("fas fa-balance-scale", "Liability"),
            ("fas fa-file-contract", "Term / Documents"),
            (DefaultIconClass, "Standard / Default")
        ];

        /// <summary>Form-template dropdown items for the insurance type master.</summary>
        /// <param name="selectedValue">Currently persisted template key.</param>
        public static IReadOnlyList<(string Value, string Text, bool Selected)> GetTemplateOptions(string? selectedValue)
        {
            var selected = string.IsNullOrWhiteSpace(selectedValue) ? StandardKey : selectedValue.Trim();
            if (InsuranceTypeCodeExtensions.TryParseTemplate(selected, out var template))
                selected = template.ToStoredKey();

            return TemplateOptions
                .Select(o => (o.Value, o.Text, Selected: string.Equals(o.Value, selected, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        /// <summary>Icon dropdown items for the insurance type master.</summary>
        /// <param name="selectedValue">Currently persisted CSS class.</param>
        public static IReadOnlyList<(string Value, string Text, bool Selected)> GetIconOptions(string? selectedValue)
        {
            var selected = string.IsNullOrWhiteSpace(selectedValue) ? DefaultIconClass : selectedValue.Trim();
            var options = IconOptions
                .Select(o => (o.Value, o.Text, Selected: string.Equals(o.Value, selected, StringComparison.Ordinal)))
                .ToList();

            if (!options.Any(o => o.Selected))
                options.Add((selected, selected, true));

            return options;
        }

        /// <summary>Admin-facing label for a specialized template (used in uniqueness errors).</summary>
        public static string GetTemplateDisplayName(InsuranceTypeCode template)
        {
            var normalized = template.NormalizeToFormTemplate();
            var member = typeof(InsuranceTypeCode).GetField(normalized.ToString());
            if (member?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() is DisplayAttribute display
                && !string.IsNullOrWhiteSpace(display.Name))
            {
                return display.Name;
            }

            return normalized.ToString();
        }
    }
}
