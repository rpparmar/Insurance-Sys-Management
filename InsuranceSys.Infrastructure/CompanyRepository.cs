using InsuranceSys.Application;
using InsuranceSys.Domain.DTO;
using InsuranceSys.Infrastructure;
using Microsoft.VisualBasic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;

namespace InsuranceSys.Infrastructure
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IAppDBContext _dbcontext;
        public CompanyRepository(IAppDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            DataSet ds = await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "CompanyMaster_GetAll");
            return ds;
        }
        public async Task<CompanyDto> GetByIdAsync(int CompanyID)
        {

            var _params = ImmutableDictionary<string, object>.Empty
            .Add("@CompanyID", CompanyID);
            CompanyDto company = await _dbcontext.GetObjectAsync<CompanyDto>(_params, CommandType.StoredProcedure, "CompanyMaster_GetByID");
            return company;
        }
        public async Task<int> AddAsync(CompanyDto company)
        {
            var _params = ImmutableDictionary<string, object>.Empty
            .Add("@CompanyName", company.CompanyName)
            .Add("@IsActive", company.IsActive);

            return await _dbcontext.ExecuteNonQueryAsync(_params, CommandType.StoredProcedure, "CompanyMaster_Insert");
        }
        public async Task<int> UpdateAsync(CompanyDto company)
        {
            var _params = ImmutableDictionary<string, object>.Empty
            .Add("@CompanyID", company.CompanyID)
            .Add("@CompanyName", company.CompanyName)
            .Add("@IsActive", company.IsActive);
            return await _dbcontext.ExecuteNonQueryAsync(_params, CommandType.StoredProcedure, "CompanyMaster_Update");
        }
        public async Task<int> DeleteAsync(int CompanyID)
        {
            var _params = ImmutableDictionary<string, object>.Empty
            .Add("@CompanyID", CompanyID);
            return await _dbcontext.ExecuteNonQueryAsync(_params, CommandType.StoredProcedure, "CompanyMaster_Delete");
        }
        public async Task<int> UpdateStatusAsync(int CompanyID, bool status)
        {
            var _params = ImmutableDictionary<string, object>.Empty
            .Add("@CompanyID", CompanyID)
            .Add("@IsActive", status);
            return await _dbcontext.ExecuteNonQueryAsync(_params, CommandType.StoredProcedure, "CompanyMaster_UpdateRecordStatus");
        }
        public async Task<string?> FindByNameAsync(string CompanyName)
        {
            var _params = ImmutableDictionary<string, object>.Empty
            .Add("@CompanyName", CompanyName);
            return await _dbcontext.ExecuteScalarAsync(_params, CommandType.StoredProcedure, "CompanyMaster_CheckExist");
            
        }
    }
}