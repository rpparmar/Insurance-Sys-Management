using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsuranceSys.Domain;
namespace InsuranceSys.Application
{
    public class CompanyService: ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        public CompanyService(ICompanyRepository companyRepository)
        {
            this._companyRepository = companyRepository;
        }
        public async Task<DataSet> GetAllCompanies(Dictionary<string, object> paramCollections)
        {
            return await _companyRepository.GetAllCompanies(paramCollections);
        }
    }
}
