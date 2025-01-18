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
    public interface ILeadRepository
    {
        //Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
        Task<LeadDto> GetByIdAsync(int LeadID);
        Task<int> AddAsync(LeadDto lead);
        Task<int> UpdateAsync(LeadDto lead);
        Task<int> DeleteAsync(int LeadID);
        Task<int> UpdateStatusAsync(int LeadID, bool status);
        //Task<string?> FindByNameAsync(string CompanyName);
    }
}
