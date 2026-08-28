namespace InsuranceSys.Domain.PolicyForms
{
    /// <summary>
    /// Immutable metadata for one policy form template. Insurance types store only the <see cref="Key"/>.
    /// </summary>
    public sealed class PolicyFormTemplateDefinition
    {
        public required string Key { get; init; }
        public required string AdminDisplayName { get; init; }
        public required string FormCollectionPrefix { get; init; }
        public required string BindingPolicyType { get; init; }
        public required string PartialViewName { get; init; }
        public required string DefaultIconClass { get; init; }

        /// <summary>Stable DOM slug. When null, slug is <c>standard-{insuranceTypeId}</c>.</summary>
        public string? StableSlug { get; init; }

        /// <summary>When true, at most one active insurance type may use this template.</summary>
        public bool IsSpecialized { get; init; }

        public bool AllowMultipleInsuranceTypes => !IsSpecialized;

        public IReadOnlyList<string> Aliases { get; init; } = [];

        public bool IsStandardBucket => !IsSpecialized;

        /// <summary>Unique card/section slug for one master row.</summary>
        public string ToUniqueSlug(int insuranceTypeId) =>
            string.IsNullOrWhiteSpace(StableSlug)
                ? $"standard-{insuranceTypeId}"
                : StableSlug;
    }
}
