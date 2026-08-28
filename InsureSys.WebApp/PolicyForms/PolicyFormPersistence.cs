using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;

namespace Insurancesys.web.PolicyForms
{
    /// <summary>Shared persist helpers for policy basic, payment, and vehicle rows.</summary>
    public sealed class PolicyFormPersistence(ICustomerService customerService, IMapper mapper)
    {
        private readonly ICustomerService _customerService = customerService;
        private readonly IMapper _mapper = mapper;

        /// <summary>Copies posted basic details for insert/update without mutating the posted model.</summary>
        public static PolicyBasicDetailsViewModel CopyForSave(
            PolicyBasicDetailsViewModel source,
            int customerId,
            int insuranceTypeId)
        {
            return new PolicyBasicDetailsViewModel
            {
                CustomerID = customerId,
                PolicyId = source.PolicyId,
                InsuranceTypeID = insuranceTypeId,
                PolicyNumber = source.PolicyNumber,
                PolicyStartDate = source.PolicyStartDate,
                PolicyDueDate = source.PolicyDueDate,
                Company = source.Company,
                GrosssPremium = source.GrosssPremium,
                NetPremium = source.NetPremium,
                ODPremium = source.ODPremium,
                NCB = source.NCB,
                Dealer = source.Dealer,
                SM = source.SM,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                IsActive = true
            };
        }

        /// <summary>Inserts or updates <c>PolicyDetails</c> and returns the persisted policy id.</summary>
        public async Task<int> SaveBasicDetailsAsync(PolicyBasicDetailsViewModel basic)
        {
            var entity = _mapper.Map<PolicyDetailsEntity>(basic);
            if (basic.PolicyId > 0)
            {
                await _customerService.UpdatePolicyDetailsAsync(entity);
                return entity.PolicyId;
            }

            return await _customerService.AddPolicyDetails(entity);
        }

        /// <summary>Inserts or updates payment details when a view model is posted.</summary>
        public async Task SavePaymentAsync(int policyId, PolicyPaymentDetailsViewModel? payment)
        {
            if (payment == null || policyId <= 0)
                return;

            var entity = _mapper.Map<PolicyPaymentDetailsEntity>(payment);
            if (entity == null)
                return;

            entity.PolicyId = policyId;
            var existing = await _customerService.GetPolicyPaymentsByPollicyAsync(policyId);
            if (existing != null)
                await _customerService.UpdatePolicyPaymentDetailsAsync(entity);
            else
                await _customerService.AddPolicyPaymentDetails(entity);
        }

        /// <summary>Inserts or updates vehicle details when a view model is posted.</summary>
        public async Task SaveVehicleAsync(int policyId, PolicyVehicleDetailsViewModel? vehicle)
        {
            if (vehicle == null || policyId <= 0)
                return;

            var entity = _mapper.Map<PolicyVehicleDetailsEntity>(vehicle);
            if (entity == null)
                return;

            entity.PolicyId = policyId;
            var existing = await _customerService.GetVehicleDetailsByPollicyAsync(policyId);
            if (existing != null)
                await _customerService.UpdateVehicleDetailsAsync(entity);
            else
                await _customerService.AddVehicleDetails(entity);
        }

        /// <summary>Maps stored vehicle details for a policy, if any.</summary>
        public async Task<PolicyVehicleDetailsViewModel?> LoadVehicleAsync(int policyId)
        {
            var vehicle = await _customerService.GetVehicleDetailsByPollicyAsync(policyId);
            return vehicle != null ? _mapper.Map<PolicyVehicleDetailsViewModel>(vehicle) : null;
        }
    }
}
