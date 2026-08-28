using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.PolicyForms;

namespace Insurancesys.web.PolicyForms.Handlers
{
    /// <summary>Health form: common fields and payment.</summary>
    public sealed class HealthPolicyFormHandler(
        PolicyFormPersistence persistence,
        IPolicyFormResolver policyFormResolver)
        : PolicyFormHandlerBase<HealthPolicyViewModel>(persistence, policyFormResolver)
    {
        public override string TemplateKey => PolicyFormCatalog.HealthKey;

        /// <inheritdoc />
        protected override List<HealthPolicyViewModel> GetList(PolicyDetailsViewModel model) =>
            model.HealthPolicies;

        /// <inheritdoc />
        protected override HealthPolicyViewModel CreateInstance(
            PolicyBasicDetailsViewModel basic,
            PolicyPaymentDetailsViewModel? payment)
        {
            return new HealthPolicyViewModel
            {
                BasicDetails = basic,
                PolicyPaymentDetails = payment
            };
        }
    }
}
