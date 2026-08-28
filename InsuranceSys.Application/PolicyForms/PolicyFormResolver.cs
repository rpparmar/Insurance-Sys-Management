using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.Enums;
using InsuranceSys.Domain.PolicyForms;

namespace InsuranceSys.Application.PolicyForms
{
    /// <summary>
    /// Resolves policy form metadata from <c>InsuranceTypeMaster</c> via <see cref="PolicyFormCatalog"/>.
    /// </summary>
    public sealed class PolicyFormResolver(IInsuranceTypeService insuranceTypeService) : IPolicyFormResolver
    {
        private readonly IInsuranceTypeService _insuranceTypeService = insuranceTypeService;

        /// <inheritdoc />
        public PolicyFormDescriptor FromEntity(InsuranceTypeEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var definition = PolicyFormCatalog.ResolveDefinition(entity.InsuranceTypeCode, entity.InsuranceTypeId);
            var iconClass = string.IsNullOrWhiteSpace(entity.IconClass)
                ? definition.DefaultIconClass
                : entity.IconClass.Trim();

            _ = Enum.TryParse(definition.Key, ignoreCase: true, out InsuranceTypeCode template);

            return new PolicyFormDescriptor
            {
                InsuranceTypeId = entity.InsuranceTypeId,
                DisplayName = entity.InsuranceType,
                Template = template,
                FormTemplateKey = definition.Key,
                Slug = definition.ToUniqueSlug(entity.InsuranceTypeId),
                IconClass = iconClass,
                FormCollectionPrefix = definition.FormCollectionPrefix,
                BindingPolicyType = definition.BindingPolicyType,
                PartialViewName = definition.PartialViewName,
                IsStandardBucket = definition.IsStandardBucket,
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
        public Task<int?> GetActiveInsuranceTypeIdByTemplateAsync(InsuranceTypeCode template) =>
            GetActiveInsuranceTypeIdByTemplateKeyAsync(template.ToStoredKey());

        /// <inheritdoc />
        public async Task<int?> GetActiveInsuranceTypeIdByTemplateKeyAsync(string templateKey)
        {
            var match = await FindFirstMatchingAsync(templateKey, activeOnly: true, excludeInsuranceTypeId: null);
            return match?.InsuranceTypeId;
        }

        /// <inheritdoc />
        public async Task<PolicyFormDescriptor?> FindConflictingActiveSpecializedAsync(
            string templateKey,
            int? excludeInsuranceTypeId)
        {
            if (!PolicyFormCatalog.TryGet(templateKey, out var definition) || !definition.IsSpecialized)
                return null;

            return await FindFirstMatchingAsync(definition.Key, activeOnly: true, excludeInsuranceTypeId);
        }

        private async Task<PolicyFormDescriptor?> FindFirstMatchingAsync(
            string templateKey,
            bool activeOnly,
            int? excludeInsuranceTypeId)
        {
            if (!PolicyFormCatalog.TryGet(templateKey, out var expected))
                return null;

            var rows = await _insuranceTypeService.GetAllNonDeletedAsync();
            foreach (var entity in rows)
            {
                if (excludeInsuranceTypeId.HasValue && entity.InsuranceTypeId == excludeInsuranceTypeId.Value)
                    continue;

                if (activeOnly && !entity.IsActive)
                    continue;

                var descriptor = FromEntity(entity);
                if (string.Equals(descriptor.FormTemplateKey, expected.Key, StringComparison.OrdinalIgnoreCase))
                    return descriptor;
            }

            return null;
        }
    }
}
