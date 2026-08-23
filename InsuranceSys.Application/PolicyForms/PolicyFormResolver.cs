using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.Enums;

namespace InsuranceSys.Application.PolicyForms
{
    /// <summary>
    /// Resolves policy form metadata from <c>InsuranceTypeMaster</c> using the stored
    /// template key (and a legacy id fallback only when that key is empty).
    /// </summary>
    public sealed class PolicyFormResolver(IInsuranceTypeService insuranceTypeService) : IPolicyFormResolver
    {
        private readonly IInsuranceTypeService _insuranceTypeService = insuranceTypeService;

        /// <inheritdoc />
        public PolicyFormDescriptor FromEntity(InsuranceTypeEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var template = InsuranceTypeCodeExtensions.ResolveTemplate(entity.InsuranceTypeCode, entity.InsuranceTypeId);
            var iconClass = string.IsNullOrWhiteSpace(entity.IconClass)
                ? template.GetDefaultIconClass()
                : entity.IconClass.Trim();

            return new PolicyFormDescriptor
            {
                InsuranceTypeId = entity.InsuranceTypeId,
                DisplayName = entity.InsuranceType,
                Template = template,
                FormTemplateKey = template.ToStoredKey(),
                Slug = template.ToUniqueSlug(entity.InsuranceTypeId),
                IconClass = iconClass,
                FormCollectionPrefix = template.GetFormCollectionPrefix(),
                BindingPolicyType = template.GetBindingPolicyType(),
                PartialViewName = template.GetPartialViewName(),
                IsStandardBucket = template.UsesStandardPoliciesList(),
                IsActive = entity.IsActive
            };
        }

        /// <inheritdoc />
        public async Task<PolicyFormDescriptor?> ResolveAsync(int insuranceTypeId)
        {
            if (insuranceTypeId <= 0)
                return null;

            var entity = await _insuranceTypeService.GetByIdAsync(insuranceTypeId);
            if (entity is null || entity.IsDeleted)
                return null;

            return FromEntity(entity);
        }

        /// <inheritdoc />
        public async Task<int?> GetActiveInsuranceTypeIdByTemplateAsync(InsuranceTypeCode template)
        {
            var match = await FindFirstMatchingAsync(template, activeOnly: true, excludeInsuranceTypeId: null);
            return match?.InsuranceTypeId;
        }

        /// <inheritdoc />
        public async Task<PolicyFormDescriptor?> FindConflictingActiveSpecializedAsync(
            InsuranceTypeCode template,
            int? excludeInsuranceTypeId)
        {
            var normalized = template.NormalizeToFormTemplate();
            if (!normalized.UsesSpecializedForm())
                return null;

            var match = await FindFirstMatchingAsync(normalized, activeOnly: true, excludeInsuranceTypeId);
            return match;
        }

        private async Task<PolicyFormDescriptor?> FindFirstMatchingAsync(
            InsuranceTypeCode template,
            bool activeOnly,
            int? excludeInsuranceTypeId)
        {
            var storedKey = template.NormalizeToFormTemplate().ToStoredKey();
            var rows = await _insuranceTypeService.GetAllNonDeletedAsync();

            foreach (var entity in rows)
            {
                if (excludeInsuranceTypeId.HasValue && entity.InsuranceTypeId == excludeInsuranceTypeId.Value)
                    continue;

                if (activeOnly && !entity.IsActive)
                    continue;

                var descriptor = FromEntity(entity);
                if (string.Equals(descriptor.FormTemplateKey, storedKey, StringComparison.OrdinalIgnoreCase))
                    return descriptor;
            }

            return null;
        }
    }
}
