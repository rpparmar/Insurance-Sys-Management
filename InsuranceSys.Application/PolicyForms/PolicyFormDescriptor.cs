using InsuranceSys.Domain.Enums;

namespace InsuranceSys.Application.PolicyForms
{
    /// <summary>
    /// Resolved rendering metadata for one <c>InsuranceTypeMaster</c> row.
    /// </summary>
    public sealed class PolicyFormDescriptor
    {
        public int InsuranceTypeId { get; init; }
        public string DisplayName { get; init; } = string.Empty;
        public InsuranceTypeCode Template { get; init; }
        public string FormTemplateKey { get; init; } = string.Empty;
        public string Slug { get; init; } = string.Empty;
        public string IconClass { get; init; } = string.Empty;
        public string FormCollectionPrefix { get; init; } = string.Empty;
        public string BindingPolicyType { get; init; } = string.Empty;
        public string PartialViewName { get; init; } = string.Empty;
        public bool IsStandardBucket { get; init; }
        public bool IsActive { get; init; }
    }
}
