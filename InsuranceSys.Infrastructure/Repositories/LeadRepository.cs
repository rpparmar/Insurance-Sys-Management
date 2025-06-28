using AutoMapper;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class LeadRepository : EfRepositoryBase, ILeadService
    {
        private readonly IAppDBContext _dbcontext;
        private readonly IMapper _mapper;
        public LeadRepository(
            IAppDBContext dbcontext
            , IMapper mapper
            , IEFdbContextFactory efdbContextFactory
            , IConnectionStringProvider connStringProvider
            ) : base(efdbContextFactory, connStringProvider)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "Lead_GetAll");
        }
        public async Task<LeadEntity?> GetByIdAsync(int LeadID)
        {
            using var _efdbcontext = await CreateContextAsync();
            return await _efdbcontext.EFLeads
                            .FirstOrDefaultAsync(c => c.LeadID == LeadID);
        }
        public async Task<int> AddAsync(LeadEntity lead)
        {
            using var _efdbcontext = await CreateContextAsync();
            lead.InquiryDate = DateTime.UtcNow;
            await _efdbcontext.EFLeads.AddAsync(lead);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(LeadEntity lead)
        {
            using var _efdbcontext = await CreateContextAsync();
            _efdbcontext.EFLeads.Update(lead);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int LeadID)
        {
            using var _efdbcontext = await CreateContextAsync();
            var _lead = await _efdbcontext.EFLeads.FindAsync(LeadID);
            if (_lead != null)
            {
                _lead.IsDeleted = true;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateStatusAsync(int LeadID, bool status)
        {
            using var _efdbcontext = await CreateContextAsync();
            var _lead = await _efdbcontext.EFLeads.FindAsync(LeadID);
            if (_lead != null)
            {
                _lead.IsActive = status;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        
    }
}
