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
    public class LeadService: ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        public LeadService(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }
        //public async Task<(IEnumerable<CountryDto> countries, int TotalCount)> GetAllCountries(ImmutableDictionary<string, object> paramCollections)
        //{
        //    return await _countryRepository.GetAllAsync(paramCollections);
        //}
        public async Task<LeadDto> GetLeadByIdAsync(int leadID)
        {
            return await _leadRepository.GetByIdAsync(leadID);
        }
        public async Task<int> AddLead(LeadDto leadDto)
        {
            return await _leadRepository.AddAsync(leadDto);
        }
        public async Task<int> UpdateLead(LeadDto leadDto)
        {
            return await _leadRepository.UpdateAsync(leadDto);
        }
        public async Task<int> DeleteLead(int leadID)
        {
            return await _leadRepository.DeleteAsync(leadID);
        }
        public async Task<int> UpdateStatus(int leadID, bool status)
        {
            return await _leadRepository.UpdateStatusAsync(leadID, status);
        }
    }
}
