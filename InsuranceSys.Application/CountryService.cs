using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application
{
    public class CountryService: ICountryService
    {
        private readonly ICountryRepository _countryRepository;
        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }
        public async Task<(IEnumerable<CountryDto> countries, int TotalCount)> GetAllCountries(ImmutableDictionary<string, object> paramCollections)
        {
            return await _countryRepository.GetAllAsync(paramCollections);
        }
        public async Task<CountryDto> GetCountryByIdAsync(int CountryID)
        {
            return await _countryRepository.GetByIdAsync(CountryID);
        }
        public async Task<int> AddCountry(CountryDto countryDto)
        {
            return await _countryRepository.AddAsync(countryDto);
        }
        public async Task<int> UpdateCountry(CountryDto countryDto)
        {
            return await _countryRepository.UpdateAsync(countryDto);
        }
        public async Task<int> DeleteCountry(int CountryID)
        {
            return await _countryRepository.DeleteAsync(CountryID);
        }
        public async Task<int> UpdateStatus(int CompanyID, bool status)
        {
            return await _countryRepository.UpdateStatusAsync(CompanyID, status);
        }
        public async Task<int> GetCountryByName(string CountryName)
        {
            return await _countryRepository.GetByName(CountryName);
        }
    }
}
