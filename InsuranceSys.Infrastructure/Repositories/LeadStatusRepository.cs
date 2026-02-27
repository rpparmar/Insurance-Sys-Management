using AutoMapper;
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
    public class LeadStatusRepository: SharedEFdbContextRepositoryBase, ILeadStatusService
    {
        private readonly IAdoNetDBContext _dbcontext;        
        public LeadStatusRepository(
            IEFdbContextProvider contextProvider, IAdoNetDBContext dbcontext) : base(contextProvider)
        {
            _dbcontext = dbcontext;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "LeadStatus_GetAll");
        }
        public async Task<LeadStatusEntity?> GetByIdAsync(int LeadStatusID)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //return await _efdbcontext.EFLeadStatus
            //                .FirstOrDefaultAsync(c => c.LeadStatusID == LeadStatusID); 
            #endregion

            return await ExecuteReadAsync(async context =>
            {
                // AsNoTracking for read operations (better performance)
                return await context.EFLeadStatus
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.LeadStatusID == LeadStatusID);
            });
        }
        public async Task<int> AddAsync(LeadStatusEntity leadstatus)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //leadstatus.CreatedOn = DateTime.UtcNow;
            //leadstatus.UpdatedOn = DateTime.UtcNow;
            //await _efdbcontext.EFLeadStatus.AddAsync(leadstatus);
            //return await _efdbcontext.SaveChangesAsync(); 
            #endregion

            return await ExecuteWriteAsync(async context =>
            {
                leadstatus.CreatedOn = DateTime.UtcNow;
                leadstatus.UpdatedOn = DateTime.UtcNow;

                await context.EFLeadStatus.AddAsync(leadstatus);
                return await context.SaveChangesAsync();
            });
        }
        public async Task<int> UpdateAsync(LeadStatusEntity leadstatus)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //leadstatus.UpdatedOn = DateTime.UtcNow;
            //_efdbcontext.EFLeadStatus.Update(leadstatus);
            //return await _efdbcontext.SaveChangesAsync(); 
            #endregion

            return await ExecuteWriteAsync(async context =>
            {
                leadstatus.UpdatedOn = DateTime.UtcNow;
                context.EFLeadStatus.Update(leadstatus);
                return await context.SaveChangesAsync();
            });
        }
        public async Task<int> DeleteAsync(int LeadStatusID)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //var _leadstatus = await _efdbcontext.EFLeadStatus.FindAsync(LeadStatusID);
            //if (_leadstatus != null)
            //{
            //    _leadstatus.UpdatedOn = DateTime.UtcNow;
            //    _leadstatus.IsDeleted = true;
            //}
            //return await _efdbcontext.SaveChangesAsync(); 
            #endregion

            return await ExecuteWriteAsync(async context =>
            {
                var leadstatus = await context.EFLeadStatus.FindAsync(LeadStatusID);

                if (leadstatus != null)
                {
                    leadstatus.UpdatedOn = DateTime.UtcNow;
                    leadstatus.IsDeleted = true;
                }

                return await context.SaveChangesAsync();
            });
        }
        public async Task<int> UpdateStatusAsync(int LeadStatusID, bool status)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //var _leadstatus = await _efdbcontext.EFLeadStatus.FindAsync(LeadStatusID);
            //if (_leadstatus != null)
            //{
            //    _leadstatus.UpdatedOn = DateTime.UtcNow;
            //    _leadstatus.IsActive = status;
            //}
            //return await _efdbcontext.SaveChangesAsync(); 
            #endregion

            return await ExecuteWriteAsync(async context =>
            {
                var leadstatus = await context.EFLeadStatus.FindAsync(LeadStatusID);

                if (leadstatus != null)
                {
                    leadstatus.UpdatedOn = DateTime.UtcNow;
                    leadstatus.IsActive = status;
                }            
                return await context.SaveChangesAsync();
            });
        }
        public async Task<bool> FindByNameAsync(string LeadStatus)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //return await _efdbcontext.EFLeadStatus.AnyAsync(c => c.LeadStatus == LeadStatus && !c.IsDeleted); 
            #endregion

            return await ExecuteReadAsync(async context =>
            {
                return await context.EFLeadStatus
                    .AsNoTracking()
                    .AnyAsync(c => c.LeadStatus == LeadStatus && !c.IsDeleted);
            });
        }
    }
}
