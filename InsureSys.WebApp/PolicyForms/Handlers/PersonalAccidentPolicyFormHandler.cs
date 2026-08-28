using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.PolicyForms;

namespace Insurancesys.web.PolicyForms.Handlers
{
    /// <summary>Personal accident form: common fields and payment.</summary>
    public sealed class PersonalAccidentPolicyFormHandler(
        PolicyFormPersistence persistence,
        IPolicyFormResolver policyFormResolver)
        : PolicyFormHandlerBase<PersonalAccidentPolicyViewModel>(persistence, policyFormResolver)
    {
        public override string TemplateKey => PolicyFormCatalog.PersonalAccidentKey;

        /// <inheritdoc />
        protected override List<PersonalAccidentPolicyViewModel> GetList(PolicyDetailsViewModel model) =>
            model.PersonalAccidentPolicies;

        /// <inheritdoc />
        protected override PersonalAccidentPolicyViewModel CreateInstance(
            PolicyBasicDetailsViewModel basic,
            PolicyPaymentDetailsViewModel? payment)
        {
            return new PersonalAccidentPolicyViewModel
            {
                BasicDetails = basic,
                PolicyPaymentDetails = payment
            };
        }
    }
}
