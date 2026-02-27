using AutoMapper;
using InsuranceSys.Application;
using InsuranceSys.Application.DTO;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class CompanyRepository_v1 : EfRepositoryBase,ICompanyService_v1
    {
        private readonly IAdoNetDBContext _dbcontext;
        private readonly IMapper _mapper;
        public CompanyRepository_v1(
            IAdoNetDBContext dbcontext
            , IMapper mapper
            , IEFdbContextFactory efdbContextFactory
            , IConnectionStringProvider connStringProvider) : base(efdbContextFactory, connStringProvider)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {            
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "CompanyMaster_GetAll");
        }
        public async Task<CompanyEntity?> GetByIdAsync(int CompanyID)
        {
            using var _efdbcontext = await CreateContextAsync();
            return await _efdbcontext.EFCompanies
                            .FirstOrDefaultAsync(c => c.CompanyID == CompanyID);
        }
        public async Task<int> AddAsync(CompanyEntity company)
        {
            using var _efdbcontext = await CreateContextAsync();
            company.CreatedOn = DateTime.UtcNow;
            company.UpdatedOn = DateTime.UtcNow;
            await _efdbcontext.EFCompanies.AddAsync(company);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(CompanyEntity company)
        {
            using var _efdbcontext = await CreateContextAsync();
            company.UpdatedOn = DateTime.UtcNow;
            _efdbcontext.EFCompanies.Update(company);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int CompanyID)
        {
            using var _efdbcontext = await CreateContextAsync();
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
            using var _efdbcontext = await CreateContextAsync();
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
            using var _efdbcontext = await CreateContextAsync();
            return await _efdbcontext.EFCompanies.AnyAsync(c => c.CompanyName == CompanyName && !c.IsDeleted);
        }        
    }
}