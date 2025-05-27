using InsuranceSys.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.Interface
{
    public interface ILeadService
    {
		Task<DataSet> GetAllLeads(ImmutableDictionary<string, object> paramCollections);
		Task<LeadDto> GetLeadByIdAsync(int LeadID);
        Task<int> AddLead(LeadDto countryDto);
        Task<int> UpdateLead(LeadDto countryDto);
        Task<int> DeleteLead(int LeadID);
        Task<int> UpdateStatus(int LeadID, bool status);
    }
}
