using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.PolicyForms;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Insurancesys.web.PolicyForms.Handlers
{
    /// <summary>
    /// Shared load / save / render for templates that bind a typed list of <typeparamref name="TViewModel"/>.
    /// </summary>
    public abstract class PolicyFormHandlerBase<TViewModel> : IPolicyFormHandler
        where TViewModel : class, ICommonPolicyForm, new()
    {
        private readonly PolicyFormPersistence _persistence;
        private readonly IPolicyFormResolver _policyFormResolver;

        protected PolicyFormHandlerBase(
            PolicyFormPersistence persistence,
            IPolicyFormResolver policyFormResolver)
        {
            _persistence = persistence;
            _policyFormResolver = policyFormResolver;
        }

        public abstract string TemplateKey { get; }

        public PolicyFormTemplateDefinition Definition =>
            PolicyFormCatalog.TryGet(TemplateKey, out var definition)
                ? definition
                : PolicyFormCatalog.Standard;

        /// <summary>Posted/loaded collection on <see cref="PolicyDetailsViewModel"/>.</summary>
        protected abstract List<TViewModel> GetList(PolicyDetailsViewModel model);

        /// <summary>Builds a list item from loaded or blank basics.</summary>
        protected abstract TViewModel CreateInstance(
            PolicyBasicDetailsViewModel basic,
            PolicyPaymentDetailsViewModel? payment);

        /// <inheritdoc />
        public virtual object CreateBlankViewModel(PolicyBasicDetailsViewModel basic) =>
            CreateInstance(basic, payment: null);

        /// <inheritdoc />
        public virtual Task AppendLoadedPolicyAsync(PolicyDetailsViewModel model, PolicyFormLoadContext context)
        {
            GetList(model).Add(CreateInstance(context.BasicDetails, context.Payment));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual async Task SavePostedAsync(PolicyDetailsViewModel model, int customerId)
        {
            var items = GetList(model);
            if (items.Count == 0)
                return;

            foreach (var item in items)
            {
                if (item.BasicDetails == null)
                    continue;

                if (!await ShouldSaveAsync(item))
                    continue;

                var typeId = await ResolveInsuranceTypeIdAsync(item.BasicDetails.InsuranceTypeID);
                if (typeId <= 0)
                    continue;

                var basic = PolicyFormPersistence.CopyForSave(item.BasicDetails, customerId, typeId);
                var policyId = await _persistence.SaveBasicDetailsAsync(basic);
                if (policyId <= 0)
                    continue;

                item.BasicDetails.PolicyId = policyId;
                await _persistence.SavePaymentAsync(policyId, item.PolicyPaymentDetails);
                await AfterSaveAsync(policyId, item);
            }
        }

        /// <inheritdoc />
        public IEnumerable<PolicyBasicDetailsViewModel> EnumeratePostedBasics(PolicyDetailsViewModel model)
        {
            return GetList(model)
                .Where(item => item.BasicDetails != null)
                .Select(item => item.BasicDetails);
        }

        /// <inheritdoc />
        public async Task RepopulateDropdownsAsync(
            PolicyDetailsViewModel model,
            Func<int, string?, Task<List<SelectListItem>>> loadCompanies)
        {
            foreach (var item in GetList(model))
            {
                if (item.BasicDetails == null || item.BasicDetails.InsuranceTypeID <= 0)
                    continue;

                item.BasicDetails.CompanySelectList = await loadCompanies(
                    item.BasicDetails.InsuranceTypeID,
                    item.BasicDetails.Company);
            }
        }

        /// <inheritdoc />
        public IHtmlContent RenderExisting(
            IHtmlHelper html,
            PolicyDetailsViewModel model,
            InsuranceTypePolicyGridItemViewModel gridItem)
        {
            var builder = new HtmlContentBuilder();
            var items = GetList(model);
            var ordinal = 1;

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                if (item.BasicDetails?.InsuranceTypeID != gridItem.InsuranceTypeId)
                    continue;

                html.ViewContext.ViewData["Index"] = index;
                html.ViewContext.ViewData["PolicyNumber"] = ordinal++;
                html.ViewContext.ViewData["PolicyType"] = gridItem.BindingPolicyType;
                html.ViewContext.ViewData["PolicySlug"] = gridItem.Slug;
                html.ViewContext.ViewData["PolicyDisplayTitle"] = gridItem.DisplayName;
                builder.AppendHtml(html.Partial(Definition.PartialViewName, item));
            }

            return builder;
        }

        /// <summary>Specialized templates fall back to the single active master row when the posted id is missing.</summary>
        protected virtual async Task<int> ResolveInsuranceTypeIdAsync(int postedInsuranceTypeId)
        {
            if (postedInsuranceTypeId > 0)
                return postedInsuranceTypeId;

            if (!Definition.IsSpecialized)
                return 0;

            return await _policyFormResolver.GetActiveInsuranceTypeIdByTemplateKeyAsync(TemplateKey) ?? 0;
        }

        /// <summary>Standard handler skips rows whose type is not a Standard template.</summary>
        protected virtual Task<bool> ShouldSaveAsync(TViewModel item) => Task.FromResult(true);

        /// <summary>Hook for extra child rows (vehicle details).</summary>
        protected virtual Task AfterSaveAsync(int policyId, TViewModel item) => Task.CompletedTask;
    }
}
