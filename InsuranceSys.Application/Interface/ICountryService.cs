using InsuranceSys.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.Interface
{
    public interface ICountryService
    {
        Task<(IEnumerable<CountryDto> countries, int TotalCount)> GetAllCountries(ImmutableDictionary<string, object> paramCollections);
        Task<CountryDto> GetCountryByIdAsync(int CountryID);
        Task<int> GetCountryByName(string CountryName);
        Task<int> AddCountry(CountryDto countryDto);
        Task<int> UpdateCountry(CountryDto countryDto);
        Task<int> DeleteCountry(int CountryID);
        Task<int> UpdateStatus(int CountryID, bool status);
    }
}
