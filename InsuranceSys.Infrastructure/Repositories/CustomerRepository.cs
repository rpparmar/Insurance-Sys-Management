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

                int generatedVehicleId = vehicleDetails.VehicleId;

                // Detach only this entity — don't affect other tracked entities
                context.Entry(vehicleDetails).State = EntityState.Detached;

                return generatedVehicleId;
            });
        }

        public async Task<int> AddPolicyPaymentDetails(PolicyPaymentDetailsEntity policyPayment)
        {
            return await ExecuteWriteAsync(async context =>
            {
                policyPayment.CreatedOn = DateTime.UtcNow;
                policyPayment.UpdatedOn = DateTime.UtcNow;
                await context.EFPolicyPayment.AddAsync(policyPayment);
                await context.SaveChangesAsync();

                int generatedPolicyId = policyPayment.PolicyId;

                // Detach only this entity — don't affect other tracked entities
                context.Entry(policyPayment).State = EntityState.Detached;

                return generatedPolicyId;
            });
        }

        public async Task<List<PolicyDetailsEntity>> GetPolicyDetailsByCustomerAsync(int customerId)
        {
            return await ExecuteReadAsync(async context =>
            {
                return await context.EFPolicyDetails.AsNoTracking()
                    .Where(p => p.CustomerID == customerId && !p.IsDeleted)
                    .ToListAsync();
            });
        }

        public async Task<PolicyVehicleDetailsEntity?> GetVehicleDetailsByPollicyAsync(int policyId)
        {
            return await ExecuteReadAsync(async context =>
            {                
                return await context.EFVehicleDetails.AsNoTracking()
                    .Where(v => v.PolicyId == policyId)
                    .FirstOrDefaultAsync();
            });
        }

        public async Task<PolicyPaymentDetailsEntity?> GetPolicyPaymentsByPollicyAsync(int policyId)
        {
            return await ExecuteReadAsync(async context =>
            {

                return await context.EFPolicyPayment.AsNoTracking()
                    .Where(pp => pp.PolicyId == policyId)
                    .FirstOrDefaultAsync();
            });
        }

        public async Task<CustomerEntity?> GetCustomerByIdAsync(int customerId)
        {
            return await ExecuteReadAsync(async context =>
            {
                return await context.EFCustomers.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CustomerID == customerId && !c.IsDeleted);
            });
        }

        public async Task<CustomerEntity?> UpdateCustomerAsync(CustomerEntity incoming)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var existing = await context.EFCustomers
                    .FirstOrDefaultAsync(c => c.CustomerID == incoming.CustomerID && !c.IsDeleted);
                if (existing == null)
                    return null;

                existing.LeadID = incoming.LeadID;
                existing.FirstName = incoming.FirstName;
                existing.LastName = incoming.LastName;
                existing.DOB = incoming.DOB;
                existing.AnniversaryDate = incoming.AnniversaryDate;
                existing.Gender = incoming.Gender;
                existing.Phone = incoming.Phone;
                existing.Email = incoming.Email;
                existing.AddressLine1 = incoming.AddressLine1;
                existing.AddressLine2 = incoming.AddressLine2;
                existing.Country = incoming.Country;
                existing.State = incoming.State;
                existing.City = incoming.City;
                existing.ZipCode = incoming.ZipCode;
                existing.IsActive = incoming.IsActive;
                existing.UpdatedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();
                context.Entry(existing).State = EntityState.Detached;
                return existing;
            });
        }

        public async Task UpdateEncryptedIdAsync(int customerId, string encryptedCustomerId)
        {
            await ExecuteAsync(async context =>
            {
                var customer = await context.EFCustomers
                    .FirstOrDefaultAsync(c => c.CustomerID == customerId);
                if (customer == null)
                    return;

                customer.EncryptedCustomerId = encryptedCustomerId;
                customer.UpdatedOn = DateTime.UtcNow;
                await context.SaveChangesAsync();
            });
        }

        public async Task<int> DeleteAsync(int customerId)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var customer = await context.EFCustomers
                    .FirstOrDefaultAsync(c => c.CustomerID == customerId && !c.IsDeleted);
                if (customer == null)
                    return 0;

                customer.IsDeleted = true;
                customer.UpdatedOn = DateTime.UtcNow;
                return await context.SaveChangesAsync();
            });
        }

        public async Task<bool> SoftDeletePolicyAsync(int policyId, int customerId)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var policy = await context.EFPolicyDetails
                    .FirstOrDefaultAsync(p => p.PolicyId == policyId && p.CustomerID == customerId && !p.IsDeleted);
                if (policy == null)
                    return false;

                policy.IsDeleted = true;
                policy.UpdatedOn = DateTime.UtcNow;
                await context.SaveChangesAsync();
                return true;
            });
        }

        public async Task UpdatePolicyDetailsAsync(PolicyDetailsEntity policyDetails)
        {
            await ExecuteAsync(async context =>
            {
                var existing = await context.EFPolicyDetails
                    .FirstOrDefaultAsync(p => p.PolicyId == policyDetails.PolicyId && p.CustomerID == policyDetails.CustomerID);
                if (existing == null)
                    return;

                existing.InsuranceTypeID = policyDetails.InsuranceTypeID;
                existing.PolicyNumber = policyDetails.PolicyNumber;
                existing.PolicyStartDate = policyDetails.PolicyStartDate;
                existing.PolicyDueDate = policyDetails.PolicyDueDate;
                existing.Company = policyDetails.Company;
                existing.GrosssPremium = policyDetails.GrosssPremium;
                existing.NetPremium = policyDetails.NetPremium;
                existing.ODPremium = policyDetails.ODPremium;
                existing.NCB = policyDetails.NCB;
                existing.Dealer = policyDetails.Dealer;
                existing.SM = policyDetails.SM;
                existing.IsActive = policyDetails.IsActive;
                existing.UpdatedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();
            });
        }

        public async Task UpdateVehicleDetailsAsync(PolicyVehicleDetailsEntity vehicleDetails)
        {
            await ExecuteAsync(async context =>
            {
                var existing = await context.EFVehicleDetails
                    .FirstOrDefaultAsync(v => v.VehicleId == vehicleDetails.VehicleId && v.PolicyId == vehicleDetails.PolicyId);
                if (existing == null)
                    return;

                existing.Vehicleno = vehicleDetails.Vehicleno;
                existing.Make = vehicleDetails.Make;
                existing.VehicleModel = vehicleDetails.VehicleModel;
                existing.Chassiseno = vehicleDetails.Chassiseno;
                existing.Segment = vehicleDetails.Segment;
                existing.Fuel = vehicleDetails.Fuel;
                existing.PlanType = vehicleDetails.PlanType;
                existing.VehicleIDV = vehicleDetails.VehicleIDV;
                existing.UpdatedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();
            });
        }

        public async Task UpdatePolicyPaymentDetailsAsync(PolicyPaymentDetailsEntity policyPayment)
        {
            await ExecuteAsync(async context =>
            {
                var existing = await context.EFPolicyPayment
                    .FirstOrDefaultAsync(p => p.PolicyId == policyPayment.PolicyId);
                if (existing == null)
                    return;

                existing.PaymentMode = policyPayment.PaymentMode;
                existing.Transactionreferance = policyPayment.Transactionreferance;
                existing.BankName = policyPayment.BankName;
                existing.UpdatedOn = DateTime.UtcNow;

                await context.SaveChangesAsync();
            });
        }
    }
}
