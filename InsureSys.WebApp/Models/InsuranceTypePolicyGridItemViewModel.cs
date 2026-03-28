namespace Insurancesys.web.Models
{
    /// <summary>One row in the customer policies type selector (from master, including inactive).</summary>
    public class InsuranceTypePolicyGridItemViewModel
    {
        public int InsuranceTypeId { get; set; }
        public string EnumName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string? IconEmoji { get; set; }
        /// <summary>Model-binding collection name: MotorPolicies, HealthPolicies, …, StandardPolicies.</summary>
        public string FormCollectionPrefix { get; set; } = string.Empty;
        public bool IsStandardBucket { get; set; }
    }
}
