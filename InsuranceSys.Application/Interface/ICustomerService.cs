using InsuranceSys.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.Interface
{
    public interface ICustomerService
    {
        Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);

        Task<int> AddCustomer(CustomerEntity customer);
        Task<CustomerEntity?> GetCustomerByIdAsync(int customerId);
        Task<CustomerEntity?> UpdateCustomerAsync(CustomerEntity customer);
        Task UpdateEncryptedIdAsync(int customerId, string encryptedCustomerId);
        /// <summary>Soft-deletes the customer (sets <c>IsDeleted</c>). Returns rows affected (0 or 1).</summary>
        Task<int> DeleteAsync(int customerId);
        Task<bool> SoftDeletePolicyAsync(int policyId, int customerId);
        Task<int> AddPolicyDetails(PolicyDetailsEntity policyDetails);
        Task<int> AddVehicleDetails(PolicyVehicleDetailsEntity vehicleDetails);
        Task<int> AddPolicyPaymentDetails(PolicyPaymentDetailsEntity policyPayment);
        Task<List<PolicyDetailsEntity>> GetPolicyDetailsByCustomerAsync(int customerId);
        Task UpdatePolicyDetailsAsync(PolicyDetailsEntity policyDetails);
        Task UpdateVehicleDetailsAsync(PolicyVehicleDetailsEntity vehicleDetails);
        Task UpdatePolicyPaymentDetailsAsync(PolicyPaymentDetailsEntity policyPayment);
        Task<PolicyVehicleDetailsEntity?> GetVehicleDetailsByPollicyAsync(int policyId);
        Task<PolicyPaymentDetailsEntity?> GetPolicyPaymentsByPollicyAsync(int policyId);
    }
}
