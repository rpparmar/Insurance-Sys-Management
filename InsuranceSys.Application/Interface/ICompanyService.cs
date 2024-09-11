using InsuranceSys.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application
{
    public interface ICompanyService
    {
        Task <DataSet> GetAllCompanies(Dictionary<string, object> paramCollections);
        Task<CompanyDto> GetCompanyById(int CompanyID);
        Task<int> AddCompany(CompanyDto model);
        Task<int> UpdateCompany(CompanyDto model);
        Task<int> DeleteCompany(int CompanyID);
    }
}
