using InsuranceSys.Application.DTO;
using InsuranceSys.Domain.Entities;
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
        #region New
        Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
        Task<CompanyEntity?> GetByIdAsync(int CompanyID);
        Task<int> AddAsync(CompanyEntity model);
        Task<int> UpdateAsync(CompanyEntity model);
        Task<int> DeleteAsync(int CompanyID);
        Task<int> UpdateStatusAsync(int CompanyID, bool status);
        Task<bool> FindByNameAsync(string CompanyName, int? excludeId = null);
        #endregion
    }
}
