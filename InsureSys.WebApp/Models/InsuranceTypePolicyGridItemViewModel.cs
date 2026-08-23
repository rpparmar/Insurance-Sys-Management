namespace Insurancesys.web.Models
{
    /// <summary>One row in the customer policies type selector (from master, including inactive).</summary>
    public class InsuranceTypePolicyGridItemViewModel
    {
        public int InsuranceTypeId { get; set; }

        /// <summary>Canonical form-template key (MotorVehicle, Health, Life, PersonalAccident, Standard).</summary>
        public string FormTemplateKey { get; set; } = string.Empty;

        /// <summary>Alias of <see cref="FormTemplateKey"/> for existing JSON consumers.</summary>
        public string EnumName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string? IconEmoji { get; set; }

        /// <summary>Value assigned to ViewBag.PolicyType for shared partial field prefixes.</summary>
        public string BindingPolicyType { get; set; } = string.Empty;

        /// <summary>Model-binding collection name: MotorPolicies, HealthPolicies, …, StandardPolicies.</summary>
        public string FormCollectionPrefix { get; set; } = string.Empty;
        public bool IsStandardBucket { get; set; }
    }
}
