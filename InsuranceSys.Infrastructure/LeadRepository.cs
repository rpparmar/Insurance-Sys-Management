using AutoMapper;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Infrastructure
{
    public class LeadRepository : ILeadService
    {
        private readonly IAppDBContext _dbcontext;
        private readonly IMapper _mapper;                
        private readonly IEFdbContextFactory _efdbContextFactory;
        private readonly IConnectionStringProvider _connStringProvider;
        public LeadRepository(            
            IAppDBContext dbcontext
            , IMapper mapper            
            , IEFdbContextFactory efdbContextFactory     
            , IConnectionStringProvider connStringProvider
            )
        {            
            _dbcontext = dbcontext;
            _mapper = mapper;
            _efdbContextFactory = efdbContextFactory;
            _connStringProvider = connStringProvider;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            DataSet ds = await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "Lead_GetAll");
            return ds;
        }
        public async Task<LeadEntity?> GetByIdAsync(int LeadID)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            
            return await _efdbcontext.EFLeads
                            .FirstOrDefaultAsync(c => c.LeadID == LeadID);
        }
        public async Task<int> AddAsync(LeadEntity lead)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            lead.InquiryDate = DateTime.UtcNow;
            await _efdbcontext.EFLeads.AddAsync(lead);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(LeadEntity lead)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            _efdbcontext.EFLeads.Update(lead);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int LeadID)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            var _lead = await _efdbcontext.EFLeads.FindAsync(LeadID);
            if (_lead != null)
            {
                _lead.IsDeleted = true;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateStatusAsync(int LeadID, bool status)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            var _lead = await _efdbcontext.EFLeads.FindAsync(LeadID);
            if (_lead != null)
            {
                _lead.IsActive = status;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
    }
}
