using InsuranceSys.Application.DTO;
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
    public interface ILeadService
    {
		Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
		Task<LeadEntity?> GetByIdAsync(int LeadID);
		Task<int> AddAsync(LeadEntity lead);
		Task<int> UpdateAsync(LeadEntity lead);
		Task<int> DeleteAsync(int LeadID);
		Task<int> UpdateStatusAsync(int LeadID, bool status);		
    }
}
