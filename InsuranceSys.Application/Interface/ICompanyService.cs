using InsuranceSys.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application
{
    public interface ICompanyService
    {
        Task <DataSet> GetAllCompanies(ImmutableDictionary<string, object> paramCollections);
        Task<CompanyDto> GetCompanyById(int CompanyID);
        Task<int> AddCompany(CompanyDto model);
        Task<int> UpdateCompany(CompanyDto model);
        Task<int> DeleteCompany(int CompanyID);
        Task<int> UpdateStatus(int CompanyID, bool status);
    }
}
