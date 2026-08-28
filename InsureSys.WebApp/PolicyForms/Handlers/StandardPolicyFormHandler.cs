using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.PolicyForms;

namespace Insurancesys.web.PolicyForms.Handlers
{
    /// <summary>Standard form: common fields and payment. Multiple insurance types may share this template.</summary>
    public sealed class StandardPolicyFormHandler(
        PolicyFormPersistence persistence,
        IPolicyFormResolver policyFormResolver)
        : PolicyFormHandlerBase<StandardPolicyViewModel>(persistence, policyFormResolver)
    {
        private readonly IPolicyFormResolver _policyFormResolver = policyFormResolver;

        public override string TemplateKey => PolicyFormCatalog.StandardKey;

        /// <inheritdoc />
        protected override List<StandardPolicyViewModel> GetList(PolicyDetailsViewModel model) =>
            model.StandardPolicies;

        /// <inheritdoc />
        protected override StandardPolicyViewModel CreateInstance(
            PolicyBasicDetailsViewModel basic,
            PolicyPaymentDetailsViewModel? payment)
        {
            return new StandardPolicyViewModel
            {
                BasicDetails = basic,
                PolicyPaymentDetails = payment
            };
        }

        /// <inheritdoc />
        protected override async Task<bool> ShouldSaveAsync(StandardPolicyViewModel item)
        {
            var typeId = item.BasicDetails?.InsuranceTypeID ?? 0;
            if (typeId <= 0)
                return false;

            var descriptor = await _policyFormResolver.ResolveAsync(typeId);
            return descriptor is { IsStandardBucket: true };
        }
    }
}
