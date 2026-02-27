using AutoMapper;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class CustomerRepository : SharedEFdbContextRepositoryBase, ICustomerService
    {
        private readonly IAdoNetDBContext _dbcontext;
        private readonly IMapper _mapper;
        public CustomerRepository(
            IEFdbContextProvider contextProvider, IAdoNetDBContext dbcontext
            , IMapper mapper) : base(contextProvider)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "Customer_GetAll");
        }

        public async Task<int> AddCustomer(CustomerEntity customer)
        {
            return await ExecuteWriteAsync(async context =>
            {
                customer.CreatedOn = DateTime.UtcNow;
                customer.UpdatedOn = DateTime.UtcNow;

                await context.EFCustomers.AddAsync(customer);
                await context.SaveChangesAsync();

                int generatedCustomerId = customer.CustomerID;

                // Detach only this entity — don't affect other tracked entities
                context.Entry(customer).State = EntityState.Detached;

                return generatedCustomerId;
            });
        }

        public async Task<int> AddPolicyDetails(PolicyDetailsEntity policyDetails)
        {
            return await ExecuteWriteAsync(async context =>
            {
                policyDetails.CreatedOn = DateTime.UtcNow;
                policyDetails.UpdatedOn = DateTime.UtcNow;                 
                await context.EFPolicyDetails.AddAsync(policyDetails);
                await context.SaveChangesAsync();

                int generatedPolicyId = policyDetails.PolicyId;

                // Detach only this entity — don't affect other tracked entities
                context.Entry(policyDetails).State = EntityState.Detached;

                return generatedPolicyId;
            });
        }

        public async Task<int> AddVehicleDetails(PolicyVehicleDetailsEntity vehicleDetails)
        {
            return await ExecuteWriteAsync(async context =>
            {
                vehicleDetails.CreatedOn = DateTime.UtcNow;
                vehicleDetails.UpdatedOn = DateTime.UtcNow;
                await context.EFVehicleDetails.AddAsync(vehicleDetails);
                await context.SaveChangesAsync();

                int generatedPolicyId = vehicleDetails.PolicyId;

                // Detach only this entity — don't affect other tracked entities
                context.Entry(vehicleDetails).State = EntityState.Detached;

                return generatedPolicyId;
            });
        }
    }
}
