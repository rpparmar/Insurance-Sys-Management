using InsuranceSys.Domain.Entities;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Application
{
    public interface IStateService
    {
        Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
        Task<StateEntity?> GetByIdAsync(int stateID);
        Task<int> AddAsync(StateEntity model);
        Task<int> UpdateAsync(StateEntity model);
        Task<int> DeleteAsync(int stateID);
        Task<int> UpdateStatusAsync(int stateID, bool status);
        Task<bool> FindByNameAsync(int countryID, string stateName, int? excludeId = null);
        Task<bool> FindByCodeAsync(int countryID, string stateCode, int? excludeId = null);
        Task<List<StateEntity>> GetByCountryAsync(int countryID);
    }
}
