using AutoMapper;
using InsuranceSys.Application;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Infrastructure
{
    public class CompanyRepository : ICompanyService
    {
        private readonly IAppDBContext _dbcontext;
        private readonly IMapper _mapper;        
        private readonly IEFdbContextFactory _efdbContextFactory;
        private readonly IConnectionStringProvider _connStringProvider;
        public CompanyRepository(
            IAppDBContext dbcontext
            , IMapper mapper            
            , IEFdbContextFactory efdbContextFactory
            , IConnectionStringProvider connStringProvider)
        {            
            _dbcontext = dbcontext;
            _mapper = mapper;
            _efdbContextFactory = efdbContextFactory;
            _connStringProvider = connStringProvider;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            DataSet ds = await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "CompanyMaster_GetAll");
            return ds;
        }
        public async Task<CompanyEntity?> GetByIdAsync(int CompanyID)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            return await _efdbcontext.EFCompanies
                            .FirstOrDefaultAsync(c => c.CompanyID == CompanyID);            
        }
        public async Task<int> AddAsync(CompanyEntity company)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            company.CreatedOn = DateTime.UtcNow;
            company.UpdatedOn = DateTime.UtcNow;
            await _efdbcontext.EFCompanies.AddAsync(company);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(CompanyEntity company)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            company.UpdatedOn = DateTime.UtcNow;
            _efdbcontext.EFCompanies.Update(company);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int CompanyID)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            var _company = await _efdbcontext.EFCompanies.FindAsync(CompanyID);
            if (_company != null)
            {
                _company.UpdatedOn = DateTime.UtcNow;
                _company.IsDeleted = true;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateStatusAsync(int CompanyID, bool status)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            var _company = await _efdbcontext.EFCompanies.FindAsync(CompanyID);
            if (_company != null)
            {
                _company.UpdatedOn = DateTime.UtcNow;
                _company.IsActive = status;
            }
            return await _efdbcontext.SaveChangesAsync();            
        }
        public async Task<bool> FindByNameAsync(string CompanyName)
        {
            var connStr = await _connStringProvider.GetConnectionStringAsync();
            using var _efdbcontext = _efdbContextFactory.CreateDbContext(connStr);
            return await _efdbcontext.EFCompanies.AnyAsync(c => c.CompanyName == CompanyName);
        }
    }
}