using AutoMapper;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Immutable;
using System.ComponentModel.Design;
using System.Data;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class LeadRepository : SharedEFdbContextRepositoryBase, ILeadService
    {
        private readonly IAdoNetDBContext _dbcontext;        
        public LeadRepository(
            IEFdbContextProvider contextProvider, IAdoNetDBContext dbcontext) : base(contextProvider)
        {
            _dbcontext = dbcontext;            
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "Lead_GetAll");
        }
        public async Task<LeadEntity?> GetByIdAsync(int LeadID)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //return await _efdbcontext.EFLeads
            //                .FirstOrDefaultAsync(c => c.LeadID == LeadID); 
            #endregion
            return await ExecuteReadAsync(async context =>
            {
                // AsNoTracking for read operations (better performance)
                return await context.EFLeads
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.LeadID == LeadID);
            });
        }
        public async Task<int> AddAsync(LeadEntity lead)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //lead.InquiryDate = DateTime.UtcNow;
            ////lead.LeadID = await _dbcontext.GetNextIdAsync("LeadManagement_Seq");
            //await _efdbcontext.EFLeads.AddAsync(lead);
            //return await _efdbcontext.SaveChangesAsync(); 
            #endregion

            return await ExecuteWriteAsync(async context =>
            {
                lead.InquiryDate = DateTime.UtcNow;
                //lead.LeadID = await _dbcontext.GetNextIdAsync("LeadManagement_Seq");

                await context.EFLeads.AddAsync(lead);
                return await context.SaveChangesAsync();
            });
        }
        public async Task<int> UpdateAsync(LeadEntity lead)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //_efdbcontext.EFLeads.Update(lead);
            //return await _efdbcontext.SaveChangesAsync(); 
            #endregion
            return await ExecuteWriteAsync(async context =>
            {
                context.EFLeads.Update(lead);

                return await context.SaveChangesAsync();
            });
        }
        public async Task<int> DeleteAsync(int LeadID)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //var _lead = await _efdbcontext.EFLeads.FindAsync(LeadID);
            //if (_lead != null)
            //{
            //    _lead.IsDeleted = true;
            //}
            //return await _efdbcontext.SaveChangesAsync(); 
            #endregion
            return await ExecuteWriteAsync(async context =>
            {
                var lead = await context.EFLeads.FindAsync(LeadID);

                if (lead != null)
                {
                    lead.IsDeleted = true;                    
                }

                return await context.SaveChangesAsync();
            });
        }
        public async Task<int> UpdateStatusAsync(int LeadID, bool status)
        {
            #region Old
            //using var _efdbcontext = await CreateContextAsync();
            //var _lead = await _efdbcontext.EFLeads.FindAsync(LeadID);
            //if (_lead != null)
            //{
            //    _lead.IsActive = status;
            //}
            //return await _efdbcontext.SaveChangesAsync(); 
            #endregion
            return await ExecuteWriteAsync(async context =>
            {
                var lead = await context.EFLeads.FindAsync(LeadID);

                if (lead != null)
                {
                    lead.IsActive = status;                    
                }
                return await context.SaveChangesAsync();
            });
        }
        
    }
}
