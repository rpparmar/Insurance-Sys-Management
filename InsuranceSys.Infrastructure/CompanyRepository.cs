using InsuranceSys.Application;
using InsuranceSys.Domain.DTO;
using InsuranceSys.Infrastructure;
using Microsoft.VisualBasic;
using System.ComponentModel;
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
        public async Task<DataSet> GetAllAsync(Dictionary<string, object> paramCollections)
        {
            DataSet ds = await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "CompanyMaster_GetAll");
            return ds;
        }
        public async Task<CompanyDto> GetByIdAsync(int CompanyID)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@CompanyID", CompanyID);
            CompanyDto company = await _dbcontext.GetObjectAsync<CompanyDto>(_params, CommandType.StoredProcedure, "CompanyMaster_GetByID");
            return company;
        }
        public async Task<int> AddAsync(CompanyDto company)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@CompanyName", company.CompanyName);
            return await _dbcontext.ExecuteNonQueryAsync(_params, CommandType.StoredProcedure, "CompanyMaster_Insert");
        }
        public async Task<int> UpdateAsync(CompanyDto company)
        {
            Dictionary<string, object> _params = new Dictionary<string, object>();
            _params.Add("@CompanyID", company.CompanyID);
            _params.Add("@CompanyName", company.CompanyName);
            _params.Add("@IsActive", company.IsActive);
            return await _dbcontext.ExecuteNonQueryAsync(_params, CommandType.StoredProcedure, "CompanyMaster_Update");
        }
        public async Task<int> DeleteAsync(int CompanyID)
        {
            return 0;
        }
    }
}