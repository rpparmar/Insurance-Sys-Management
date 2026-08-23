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
        /// <param name="entity">Non-deleted or historical insurance type row.</param>
        /// <returns>Resolved form, slug, icon, and binding metadata.</returns>
        PolicyFormDescriptor FromEntity(InsuranceTypeEntity entity);

        /// <summary>Loads a master row by id and resolves its form template.</summary>
        /// <param name="insuranceTypeId">Master primary key.</param>
        /// <returns>Descriptor, or <c>null</c> when the type does not exist.</returns>
        Task<PolicyFormDescriptor?> ResolveAsync(int insuranceTypeId);

        /// <summary>
        /// Returns the first active, non-deleted type that uses <paramref name="template"/>.
        /// </summary>
        /// <param name="template">Specialized or standard form template.</param>
        /// <returns>Master id, or <c>null</c> when none is configured.</returns>
        Task<int?> GetActiveInsuranceTypeIdByTemplateAsync(InsuranceTypeCode template);

        /// <summary>
        /// Finds another active type that already owns a specialized form template.
        /// Standard templates never conflict.
        /// </summary>
        /// <param name="template">Candidate form template.</param>
        /// <param name="excludeInsuranceTypeId">Current row id when editing.</param>
        /// <returns>Conflicting descriptor, or <c>null</c>.</returns>
        Task<PolicyFormDescriptor?> FindConflictingActiveSpecializedAsync(
            InsuranceTypeCode template,
            int? excludeInsuranceTypeId);
    }
}
