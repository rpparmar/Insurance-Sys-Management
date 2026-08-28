using Insurancesys.web.Models;
using InsuranceSys.Domain.PolicyForms;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Insurancesys.web.PolicyForms
{
    /// <summary>
    /// One registered policy form template: blank partial, load, save, and existing-row render.
    /// </summary>
    public interface IPolicyFormHandler
    {
        string TemplateKey { get; }

        PolicyFormTemplateDefinition Definition { get; }

        /// <summary>Creates the view model for a newly added policy card.</summary>
        object CreateBlankViewModel(PolicyBasicDetailsViewModel basic);

        /// <summary>Adds an existing policy to the correct collection on <paramref name="model"/>.</summary>
        Task AppendLoadedPolicyAsync(PolicyDetailsViewModel model, PolicyFormLoadContext context);

        /// <summary>Persists posted rows for this template.</summary>
        Task SavePostedAsync(PolicyDetailsViewModel model, int customerId);

        /// <summary>Posted basic-detail rows (for master validation).</summary>
        IEnumerable<PolicyBasicDetailsViewModel> EnumeratePostedBasics(PolicyDetailsViewModel model);

        /// <summary>Rebuilds insurer dropdowns after a failed save.</summary>
        Task RepopulateDropdownsAsync(
            PolicyDetailsViewModel model,
            Func<int, string?, Task<List<SelectListItem>>> loadCompanies);

        /// <summary>Renders existing instances for one insurance-type card.</summary>
        IHtmlContent RenderExisting(
            IHtmlHelper html,
            PolicyDetailsViewModel model,
            InsuranceTypePolicyGridItemViewModel gridItem);
    }
}
