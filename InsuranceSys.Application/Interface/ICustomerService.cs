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
        //Task<int> AddPolicyVehicleDetails(PolicyVehicleDetailsEntity vehicleDetails);
        //Task<int> AddPolicyPaymentDetails(PolicyPaymentDetailsEntity policyPayment);
    }
}
