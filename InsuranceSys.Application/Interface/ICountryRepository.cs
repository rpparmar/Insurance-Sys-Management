using InsuranceSys.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.Interface
{
    public interface ICountryRepository
    {
        Task<(IEnumerable<CountryDto> countries, int TotalCount)> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
        Task<CountryDto> GetByIdAsync(int CountryID);
        Task<int> GetByName(string CountryName);
        Task<int> AddAsync(CountryDto countryDto);
        Task<int> UpdateAsync(CountryDto countryDto);
        Task<int> DeleteAsync(int CountryID);
        Task<int> UpdateStatusAsync(int CountryID, bool status);
    }
}
