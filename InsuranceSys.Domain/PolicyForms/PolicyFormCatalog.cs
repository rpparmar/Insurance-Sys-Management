using InsuranceSys.Domain.Enums;

namespace InsuranceSys.Domain.PolicyForms
{
    /// <summary>
    /// Registered form templates and icon options. New layouts are added here, then a matching handler is registered in the web app.
    /// </summary>
    public static class PolicyFormCatalog
    {
        public const string MotorVehicleKey = nameof(InsuranceTypeCode.MotorVehicle);
        public const string HealthKey = nameof(InsuranceTypeCode.Health);
        public const string LifeKey = nameof(InsuranceTypeCode.Life);
        public const string PersonalAccidentKey = nameof(InsuranceTypeCode.PersonalAccident);
        public const string StandardKey = nameof(InsuranceTypeCode.Standard);
        public const string DefaultIconClass = "fas fa-file-alt";

        public static readonly PolicyFormTemplateDefinition MotorVehicle = new()
        {
            Key = MotorVehicleKey,
            AdminDisplayName = "Motor Insurance Template",
            FormCollectionPrefix = "MotorPolicies",
            BindingPolicyType = "Motor",
            PartialViewName = "_MotorPolicyPartial",
            DefaultIconClass = "fas fa-car-side",
            StableSlug = "motor",
            IsSpecialized = true
        };

        public static readonly PolicyFormTemplateDefinition Health = new()
        {
            Key = HealthKey,
            AdminDisplayName = "Health Insurance Template",
            FormCollectionPrefix = "HealthPolicies",
            BindingPolicyType = "Health",
            PartialViewName = "_HealthPolicyPartial",
            DefaultIconClass = "fas fa-heartbeat",
            StableSlug = "health",
            IsSpecialized = true
        };

        public static readonly PolicyFormTemplateDefinition Life = new()
        {
            Key = LifeKey,
            AdminDisplayName = "Life Insurance Template",
            FormCollectionPrefix = "LifePolicies",
            BindingPolicyType = "Life",
            PartialViewName = "_LifePolicyPartial",
            DefaultIconClass = "fas fa-shield-alt",
            StableSlug = "life",
            IsSpecialized = true
        };

        public static readonly PolicyFormTemplateDefinition PersonalAccident = new()
        {
            Key = PersonalAccidentKey,
            AdminDisplayName = "Personal Accident Insurance Template",
            FormCollectionPrefix = "PersonalAccidentPolicies",
            BindingPolicyType = "PersonalAccident",
            PartialViewName = "_PersonalAccidentPolicyPartial",
            DefaultIconClass = "fas fa-ambulance",
            StableSlug = "personalaccident",
            IsSpecialized = true
        };

        public static readonly PolicyFormTemplateDefinition Standard = new()
        {
            Key = StandardKey,
            AdminDisplayName = "Default (Basic details)",
            FormCollectionPrefix = "StandardPolicies",
            BindingPolicyType = "Standard",
            PartialViewName = "_StandardPolicyPartial",
            DefaultIconClass = DefaultIconClass,
            IsSpecialized = false,
            Aliases =
            [
                nameof(InsuranceTypeCode.Travel),
                nameof(InsuranceTypeCode.HomeProperty),
                nameof(InsuranceTypeCode.Fire),
                nameof(InsuranceTypeCode.Marine),
                nameof(InsuranceTypeCode.Commercial),
                nameof(InsuranceTypeCode.Liability),
                nameof(InsuranceTypeCode.TermLife)
            ]
        };

        public static IReadOnlyList<PolicyFormTemplateDefinition> All { get; } =
        [
            MotorVehicle,
            Health,
            Life,
            PersonalAccident,
            Standard
        ];

        private static readonly Dictionary<string, PolicyFormTemplateDefinition> Lookup = BuildLookup();

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

        /// <summary>Looks up a template by canonical key or alias (Travel, Fire, …).</summary>
        public static bool TryGet(string? key, out PolicyFormTemplateDefinition definition)
        {
            if (!string.IsNullOrWhiteSpace(key)
                && Lookup.TryGetValue(key.Trim(), out var match))
            {
                definition = match;
                return true;
            }

            definition = Standard;
            return false;
        }

        /// <summary>
        /// Resolves the template for a master row. Uses the stored key when present;
        /// empty keys fall back to legacy identities 1–4.
        /// </summary>
        public static PolicyFormTemplateDefinition ResolveDefinition(string? storedKey, int insuranceTypeId)
        {
            if (!string.IsNullOrWhiteSpace(storedKey))
                return TryGet(storedKey, out var fromKey) ? fromKey : Standard;

            return insuranceTypeId switch
            {
                1 => MotorVehicle,
                2 => Health,
                3 => Life,
                4 => PersonalAccident,
                _ => Standard
            };
        }

        /// <summary>Form-template dropdown items for the insurance type master.</summary>
        public static IReadOnlyList<(string Value, string Text, bool Selected)> GetTemplateOptions(string? selectedValue)
        {
            var selected = ResolveDefinition(selectedValue, insuranceTypeId: 0).Key;
            return All
                .Select(d => (d.Key, d.AdminDisplayName, Selected: string.Equals(d.Key, selected, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        /// <summary>Icon dropdown items for the insurance type master.</summary>
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

        /// <summary>Admin-facing label for uniqueness errors.</summary>
        public static string GetTemplateDisplayName(string? templateKey)
        {
            return TryGet(templateKey, out var definition)
                ? definition.AdminDisplayName
                : Standard.AdminDisplayName;
        }

        /// <summary>Admin-facing label for an enum template value.</summary>
        public static string GetTemplateDisplayName(InsuranceTypeCode template)
        {
            return GetTemplateDisplayName(InsuranceTypeCodeExtensions.ToStoredKey(template));
        }

        private static Dictionary<string, PolicyFormTemplateDefinition> BuildLookup()
        {
            var map = new Dictionary<string, PolicyFormTemplateDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (var definition in All)
            {
                map[definition.Key] = definition;
                foreach (var alias in definition.Aliases)
                    map[alias] = definition;
            }

            return map;
        }
    }
}
