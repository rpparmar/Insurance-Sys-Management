using AutoMapper;
using Azure.Core;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.DTO;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.EFEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure
{
    public class LeadRepository: ILeadRepository
    {
        private readonly EfdbContext _efdbcontext;
        private readonly IAppDBContext _dbcontext;
        private readonly IMapper _mapper;
        //private readonly DbContextFactory _dbContextFactory;
        //private readonly IConfiguration _config;

        public LeadRepository(EfdbContext efdbcontext, IAppDBContext dbcontext, IMapper mapper
            //, DbContextFactory dbContextFactory
            //, IConfiguration config
            )
        {
            _efdbcontext = efdbcontext;
            _dbcontext = dbcontext;
            _mapper = mapper;
            //_dbContextFactory = dbContextFactory;
            //_config = config;
        }
		public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
		{
			DataSet ds = await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "Lead_GetAll");
			return ds;
		}
		public async Task<LeadDto> GetByIdAsync(int LeadID)
        {
            var _leadEfEntity = await _efdbcontext.EFLeads
                            .FirstOrDefaultAsync(c => c.LeadID == LeadID);
            return _mapper.Map<LeadDto>(_leadEfEntity);
        }
        public async Task<int> AddAsync(LeadDto lead)
        {
            var _leadEfEntity = _mapper.Map<LeadEFEntity>(lead);
            _leadEfEntity.InquiryDate = DateTime.UtcNow;
            await _efdbcontext.EFLeads.AddAsync(_leadEfEntity);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(LeadDto lead)
        {
            var _lead = await _efdbcontext.EFLeads.FirstOrDefaultAsync(c => c.LeadID == lead.LeadID);
            if (_lead != null)
            {
                _mapper.Map(lead, _lead);
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int LeadID)
        {
            var _lead = await _efdbcontext.EFLeads.FindAsync(LeadID);
            if (_lead != null)
            {
                _lead.IsDeleted = true;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateStatusAsync(int LeadID, bool status)
        {
            var _lead = await _efdbcontext.Countries.FindAsync(LeadID);
            if (_lead != null)
            {
                _lead.IsActive = status;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
    }
}
