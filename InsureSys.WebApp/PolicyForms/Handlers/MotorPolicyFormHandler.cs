using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.PolicyForms;

namespace Insurancesys.web.PolicyForms.Handlers
{
    /// <summary>Motor / vehicle form: common fields, vehicle block, and payment.</summary>
    public sealed class MotorPolicyFormHandler(
        PolicyFormPersistence persistence,
        IPolicyFormResolver policyFormResolver)
        : PolicyFormHandlerBase<MotorPolicyViewModel>(persistence, policyFormResolver)
    {
        private readonly PolicyFormPersistence _persistence = persistence;

        public override string TemplateKey => PolicyFormCatalog.MotorVehicleKey;

        /// <inheritdoc />
        protected override List<MotorPolicyViewModel> GetList(PolicyDetailsViewModel model) =>
            model.MotorPolicies;

        /// <inheritdoc />
        protected override MotorPolicyViewModel CreateInstance(
            PolicyBasicDetailsViewModel basic,
            PolicyPaymentDetailsViewModel? payment)
        {
            return new MotorPolicyViewModel
            {
                BasicDetails = basic,
                VehicleDetails = new PolicyVehicleDetailsViewModel(),
                PolicyPaymentDetails = payment
            };
        }

        /// <inheritdoc />
        public override object CreateBlankViewModel(PolicyBasicDetailsViewModel basic) =>
            CreateInstance(basic, payment: null);

        /// <inheritdoc />
        public override async Task AppendLoadedPolicyAsync(PolicyDetailsViewModel model, PolicyFormLoadContext context)
        {
            var vehicle = await _persistence.LoadVehicleAsync(context.PolicyId);
            model.MotorPolicies.Add(new MotorPolicyViewModel
            {
                BasicDetails = context.BasicDetails,
                VehicleDetails = vehicle,
                PolicyPaymentDetails = context.Payment
            });
        }

        /// <inheritdoc />
        protected override Task AfterSaveAsync(int policyId, MotorPolicyViewModel item) =>
            _persistence.SaveVehicleAsync(policyId, item.VehicleDetails);
    }
}
