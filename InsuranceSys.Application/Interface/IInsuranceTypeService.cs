using InsuranceSys.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.Interface
{
    public interface IInsuranceTypeService
    {
        Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
        Task<InsuranceTypeEntity?> GetByIdAsync(int CompanyID);
        Task<int> AddAsync(InsuranceTypeEntity model);
        Task<int> UpdateAsync(InsuranceTypeEntity model);
        Task<int> DeleteAsync(int InsuranceTypeId);
        Task<int> UpdateStatusAsync(int InsuranceTypeId, bool status);
        Task<bool> FindByNameAsync(string InsuranceType);
        Task<IReadOnlyList<InsuranceTypeEntity>> GetAllNonDeletedAsync();
        Task<HashSet<int>> GetActiveInsuranceTypeIdsAsync();
    }
}
