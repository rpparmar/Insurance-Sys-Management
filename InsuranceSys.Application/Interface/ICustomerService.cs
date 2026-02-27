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
        Task<int> AddPolicyDetails(PolicyDetailsEntity policyDetails);
        Task<int> AddVehicleDetails(PolicyVehicleDetailsEntity vehicleDetails);
        Task<int> AddPolicyPaymentDetails(PolicyPaymentDetailsEntity policyPayment);
        Task<List<PolicyDetailsEntity>> GetPolicyDetailsByCustomerAsync(int customerId);
        Task<PolicyVehicleDetailsEntity?> GetVehicleDetailsByPollicyAsync(int policyId);
        Task<PolicyPaymentDetailsEntity?> GetPolicyPaymentsByPollicyAsync(int policyId);
    }
}
