using InsuranceSys.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _companyRepository.GetAllAsync(paramCollections);
        }
        public async Task<CompanyDto> GetCompanyById(int CompanyID)
        {
            return await _companyRepository.GetByIdAsync(CompanyID);
        }
        public async Task<int> AddCompany(CompanyDto model)
        {
            return await _companyRepository.AddAsync(model);
        }
        public async Task<int> UpdateCompany(CompanyDto model)
        {
            return await _companyRepository.UpdateAsync(model);
        }
        public async Task<int> DeleteCompany(int CompanyID)
        {
            return await _companyRepository.DeleteAsync(CompanyID);
        }
    }
}
