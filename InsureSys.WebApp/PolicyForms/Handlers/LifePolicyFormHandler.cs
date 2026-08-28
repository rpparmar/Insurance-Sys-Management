using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.PolicyForms;

namespace Insurancesys.web.PolicyForms.Handlers
{
    /// <summary>Life form: common fields and payment.</summary>
    public sealed class LifePolicyFormHandler(
        PolicyFormPersistence persistence,
        IPolicyFormResolver policyFormResolver)
        : PolicyFormHandlerBase<LifePolicyViewModel>(persistence, policyFormResolver)
    {
        public override string TemplateKey => PolicyFormCatalog.LifeKey;

        /// <inheritdoc />
        protected override List<LifePolicyViewModel> GetList(PolicyDetailsViewModel model) =>
            model.LifePolicies;

        /// <inheritdoc />
        protected override LifePolicyViewModel CreateInstance(
            PolicyBasicDetailsViewModel basic,
            PolicyPaymentDetailsViewModel? payment)
        {
            return new LifePolicyViewModel
            {
                BasicDetails = basic,
                PolicyPaymentDetails = payment
            };
        }
    }
}
