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
    public class LeadStatusRepository: EfRepositoryBase, ILeadStatusService
    {
        private readonly IAppDBContext _dbcontext;
        private readonly IMapper _mapper;
        public LeadStatusRepository(
            IAppDBContext dbcontext
            , IMapper mapper
            , IEFdbContextFactory efdbContextFactory
            , IConnectionStringProvider connStringProvider) : base(efdbContextFactory, connStringProvider)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "LeadStatus_GetAll");
        }
        public async Task<LeadStatusEntity?> GetByIdAsync(int LeadStatusID)
        {
            using var _efdbcontext = await CreateContextAsync();
            return await _efdbcontext.EFLeadStatus
                            .FirstOrDefaultAsync(c => c.LeadStatusID == LeadStatusID);
        }
        public async Task<int> AddAsync(LeadStatusEntity leadstatus)
        {
            using var _efdbcontext = await CreateContextAsync();
            leadstatus.CreatedOn = DateTime.UtcNow;
            leadstatus.UpdatedOn = DateTime.UtcNow;
            await _efdbcontext.EFLeadStatus.AddAsync(leadstatus);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(LeadStatusEntity leadstatus)
        {
            using var _efdbcontext = await CreateContextAsync();
            leadstatus.UpdatedOn = DateTime.UtcNow;
            _efdbcontext.EFLeadStatus.Update(leadstatus);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int LeadStatusID)
        {
            using var _efdbcontext = await CreateContextAsync();
            var _leadstatus = await _efdbcontext.EFLeadStatus.FindAsync(LeadStatusID);
            if (_leadstatus != null)
            {
                _leadstatus.UpdatedOn = DateTime.UtcNow;
                _leadstatus.IsDeleted = true;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateStatusAsync(int LeadStatusID, bool status)
        {
            using var _efdbcontext = await CreateContextAsync();
            var _leadstatus = await _efdbcontext.EFLeadStatus.FindAsync(LeadStatusID);
            if (_leadstatus != null)
            {
                _leadstatus.UpdatedOn = DateTime.UtcNow;
                _leadstatus.IsActive = status;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<bool> FindByNameAsync(string LeadStatus)
        {
            using var _efdbcontext = await CreateContextAsync();
            return await _efdbcontext.EFLeadStatus.AnyAsync(c => c.LeadStatus == LeadStatus && !c.IsDeleted);
        }
    }
}
