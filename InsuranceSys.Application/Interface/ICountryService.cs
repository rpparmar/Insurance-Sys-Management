using InsuranceSys.Domain.Entities;
using System.Collections.Immutable;
using System.Data;

namespace InsuranceSys.Application
{
    public interface ICountryService
    {
        Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
        Task<CountryEntity?> GetByIdAsync(int countryID);
        Task<int> AddAsync(CountryEntity model);
        Task<int> UpdateAsync(CountryEntity model);
        Task<int> DeleteAsync(int countryID);
        Task<int> UpdateStatusAsync(int countryID, bool status);
        Task<bool> FindByNameAsync(string countryName, int? excludeId = null);
        Task<bool> FindByCodeAsync(string countryCode, int? excludeId = null);
    }
}
