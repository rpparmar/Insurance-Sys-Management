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
    public interface ICompanyRepository
    {
        Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
        Task<CompanyDto> GetByIdAsync(int CompanyID);
        Task<int> AddAsync(CompanyDto company);
        Task<int> UpdateAsync(CompanyDto company);
        Task<int> DeleteAsync(int CompanyID);
        Task<int> UpdateStatusAsync(int CompanyID, bool status);
    }
}
