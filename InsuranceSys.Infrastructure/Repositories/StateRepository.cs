using InsuranceSys.Application;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class StateRepository : SharedEFdbContextRepositoryBase, IStateService
    {
        private readonly IAdoNetDBContext _dbcontext;
        public StateRepository(IEFdbContextProvider contextProvider, IAdoNetDBContext dbcontext) : base(contextProvider)
        {
            _dbcontext = dbcontext;
        }

        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "StateMaster_GetAll");
        }

        public async Task<StateEntity?> GetByIdAsync(int stateID)
        {
            return await ExecuteReadAsync(async context =>
            {
                return await context.EFStates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.StateID == stateID && !x.IsDeleted);
            });
        }

        public async Task<int> AddAsync(StateEntity model)
        {
            return await ExecuteWriteAsync(async context =>
            {
                model.CreatedOn = DateTime.UtcNow;
                model.UpdatedOn = DateTime.UtcNow;
                await context.EFStates.AddAsync(model);
                return await context.SaveChangesAsync();
            });
        }

        public async Task<int> UpdateAsync(StateEntity model)
        {
            return await ExecuteWriteAsync(async context =>
            {
                model.UpdatedOn = DateTime.UtcNow;
                context.EFStates.Update(model);
                return await context.SaveChangesAsync();
            });
        }

        public async Task<int> DeleteAsync(int stateID)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var entity = await context.EFStates.FirstOrDefaultAsync(x => x.StateID == stateID);
                if (entity != null)
                {
                    entity.IsDeleted = true;
                    entity.UpdatedOn = DateTime.UtcNow;
                }
                return await context.SaveChangesAsync();
            });
        }

        public async Task<int> UpdateStatusAsync(int stateID, bool status)
        {
            return await ExecuteWriteAsync(async context =>
            {
                var entity = await context.EFStates.FindAsync(stateID);
                if (entity != null)
                {
                    entity.IsActive = status;
                    entity.UpdatedOn = DateTime.UtcNow;
                }
                return await context.SaveChangesAsync();
            });
        }

        public async Task<bool> FindByNameAsync(int countryID, string stateName, int? excludeId = null)
        {
            return await ExecuteReadAsync(async context =>
            {
                var query = context.EFStates
                    .AsNoTracking()
                    .Where(x => x.CountryID == countryID && x.StateName == stateName && !x.IsDeleted);
                if (excludeId.HasValue)
                {
                    query = query.Where(x => x.StateID != excludeId.Value);
                }
                return await query.AnyAsync();
            });
        }

        public async Task<bool> FindByCodeAsync(int countryID, string stateCode, int? excludeId = null)
        {
            return await ExecuteReadAsync(async context =>
            {
                var query = context.EFStates
                    .AsNoTracking()
                    .Where(x => x.CountryID == countryID && x.StateCode == stateCode && !x.IsDeleted);
                if (excludeId.HasValue)
                {
                    query = query.Where(x => x.StateID != excludeId.Value);
                }
                return await query.AnyAsync();
            });
        }

        public async Task<List<StateEntity>> GetByCountryAsync(int countryID)
        {
            return await ExecuteReadAsync(async context =>
            {
                return await context.EFStates
                    .AsNoTracking()
                    .Where(x => x.CountryID == countryID && x.IsActive && !x.IsDeleted)
                    .OrderBy(x => x.StateName)
                    .ToListAsync();
            });
        }
    }
}
