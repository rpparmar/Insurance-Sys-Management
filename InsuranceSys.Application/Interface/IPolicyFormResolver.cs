using InsuranceSys.Application.PolicyForms;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Domain.Enums;

namespace InsuranceSys.Application.Interface
{
    /// <summary>
    /// Maps insurance-type master rows to form-template descriptors without using
    /// <c>InsuranceTypeId</c> as an enum value.
    /// </summary>
    public interface IPolicyFormResolver
    {
        /// <summary>Builds a descriptor from a loaded master entity.</summary>
        PolicyFormDescriptor FromEntity(InsuranceTypeEntity entity);

        /// <summary>Loads a master row by id and resolves its form template.</summary>
        Task<PolicyFormDescriptor?> ResolveAsync(int insuranceTypeId);

        /// <summary>Returns the first active type that uses <paramref name="templateKey"/>.</summary>
        Task<int?> GetActiveInsuranceTypeIdByTemplateKeyAsync(string templateKey);

        /// <summary>Returns the first active type that uses <paramref name="template"/>.</summary>
        Task<int?> GetActiveInsuranceTypeIdByTemplateAsync(InsuranceTypeCode template);

        /// <summary>
        /// Finds another active type that already owns a specialized form template.
        /// Standard templates never conflict.
        /// </summary>
        Task<PolicyFormDescriptor?> FindConflictingActiveSpecializedAsync(
            string templateKey,
            int? excludeInsuranceTypeId);
    }
}
