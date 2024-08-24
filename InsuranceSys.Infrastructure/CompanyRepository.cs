using InsuranceSys.Application;
using InsuranceSys.Domain;
using InsuranceSys.Infrastructure;
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
        public async Task<DataSet> GetAllCompanies(Dictionary<string, object> paramCollections)
        {
            DataSet ds = await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "Master_Company_GetAll_New");
            return ds;
        }
    }
}