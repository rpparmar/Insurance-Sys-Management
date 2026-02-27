using AutoMapper;
using InsuranceSys.Application;
using InsuranceSys.Application.DTO;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Data;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class CompanyRepository : SharedEFdbContextRepositoryBase, ICompanyService
    {
        private readonly IAdoNetDBContext _dbcontext;        
        public CompanyRepository(IEFdbContextProvider contextProvider, IAdoNetDBContext dbcontext) : base(contextProvider)
        {
            _dbcontext = dbcontext;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {            
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "CompanyMaster_GetAll");
        }
        
        public async Task<CompanyEntity?> GetByIdAsync(int companyId)
        {
            return await ExecuteReadAsync(async context =>
            {
                // AsNoTracking for read operations (better performance)
                return await context.EFCompanies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CompanyID == companyId && !c.IsDeleted);
            });
        }        
        public async Task<int> AddAsync(CompanyEntity company)
        {
            return await ExecuteWriteAsync(async context =>
            {
                company.CreatedOn = DateTime.UtcNow;
                company.UpdatedOn = DateTime.UtcNow;

                await context.EFCompanies.AddAsync(company);
                return await context.SaveChangesAsync();
            });
        }        
        public async Task<int> UpdateAsync(CompanyEntity company)
        {
            return await ExecuteWriteAsync(async context =>
            {
                company.UpdatedOn = DateTime.UtcNow;

                context.EFCompanies.Update(company);
                return await context.SaveChangesAsync();

            });
        }                
        public async Task<int> DeleteAsync(int companyId)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var company = await context.EFCompanies
                    .FirstOrDefaultAsync(c => c.CompanyID == companyId);

                if (company != null)
                {
                    company.IsDeleted = true;
                    company.UpdatedOn = DateTime.UtcNow;                                        
                }
                return await context.SaveChangesAsync();
            });
        }        
        public async Task<bool> FindByNameAsync(string companyName, int? excludeId = null)
        {
            return await ExecuteReadAsync(async context =>
            {
                var query = context.EFCompanies
                    .AsNoTracking()
                    .Where(c => c.CompanyName == companyName && !c.IsDeleted);

                if (excludeId.HasValue)
                {
                    query = query.Where(c => c.CompanyID != excludeId.Value);
                }

                return await query.AnyAsync();
            });
        }

        public async Task<int> UpdateStatusAsync(int CompanyID, bool status)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var company = await context.EFCompanies.FindAsync(CompanyID);

                if (company != null)
                {
                    company.UpdatedOn = DateTime.UtcNow;
                    company.IsActive = status;
                }

                return await context.SaveChangesAsync();
            });

        }
    }
}