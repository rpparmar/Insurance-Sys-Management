using InsuranceSys.Application;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class CountryRepository : SharedEFdbContextRepositoryBase, ICountryService
    {
        private readonly IAdoNetDBContext _dbcontext;
        public CountryRepository(IEFdbContextProvider contextProvider, IAdoNetDBContext dbcontext) : base(contextProvider)
        {
            _dbcontext = dbcontext;
        }

        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "CountryMaster_GetAll");
        }

        public async Task<CountryEntity?> GetByIdAsync(int countryID)
        {
            return await ExecuteReadAsync(async context =>
            {
                return await context.EFCountries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.CountryID == countryID && !x.IsDeleted);
            });
        }

        public async Task<int> AddAsync(CountryEntity model)
        {
            return await ExecuteWriteAsync(async context =>
            {
                model.CreatedOn = DateTime.UtcNow;
                model.UpdatedOn = DateTime.UtcNow;
                await context.EFCountries.AddAsync(model);
                return await context.SaveChangesAsync();
            });
        }

        public async Task<int> UpdateAsync(CountryEntity model)
        {
            return await ExecuteWriteAsync(async context =>
            {
                model.UpdatedOn = DateTime.UtcNow;
                context.EFCountries.Update(model);
                return await context.SaveChangesAsync();
            });
        }

        public async Task<int> DeleteAsync(int countryID)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var entity = await context.EFCountries.FirstOrDefaultAsync(x => x.CountryID == countryID);
                if (entity != null)
                {
                    entity.IsDeleted = true;
                    entity.UpdatedOn = DateTime.UtcNow;
                }
                return await context.SaveChangesAsync();
            });
        }

        public async Task<int> UpdateStatusAsync(int countryID, bool status)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var entity = await context.EFCountries.FindAsync(countryID);
                if (entity != null)
                {
                    entity.IsActive = status;
                    entity.UpdatedOn = DateTime.UtcNow;
                }
                return await context.SaveChangesAsync();
            });
        }

        public async Task<bool> FindByNameAsync(string countryName, int? excludeId = null)
        {
            return await ExecuteReadAsync(async context =>
            {
                var query = context.EFCountries
                    .AsNoTracking()
                    .Where(x => x.CountryName == countryName && !x.IsDeleted);
                if (excludeId.HasValue)
                {
                    query = query.Where(x => x.CountryID != excludeId.Value);
                }
                return await query.AnyAsync();
            });
        }

        public async Task<bool> FindByCodeAsync(string countryCode, int? excludeId = null)
        {
            return await ExecuteReadAsync(async context =>
            {
                var query = context.EFCountries
                    .AsNoTracking()
                    .Where(x => x.CountryCode == countryCode && !x.IsDeleted);
                if (excludeId.HasValue)
                {
                    query = query.Where(x => x.CountryID != excludeId.Value);
                }
                return await query.AnyAsync();
            });
        }
    }
}
