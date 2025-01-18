using InsuranceSys.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.Interface
{
    public interface ILeadService
    {
        //Task<(IEnumerable<CountryDto> countries, int TotalCount)> GetAllCountries(ImmutableDictionary<string, object> paramCollections);
        Task<LeadDto> GetLeadByIdAsync(int LeadID);
        //Task<int> GetCountryByName(string CountryName);
        Task<int> AddLead(LeadDto countryDto);
        Task<int> UpdateLead(LeadDto countryDto);
        Task<int> DeleteLead(int LeadID);
        Task<int> UpdateStatus(int LeadID, bool status);
    }
}
