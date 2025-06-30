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
    public interface ILeadStatusService
    {
        Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections);
        Task<LeadStatusEntity?> GetByIdAsync(int LeadStatusID);
        Task<int> AddAsync(LeadStatusEntity model);
        Task<int> UpdateAsync(LeadStatusEntity model);
        Task<int> DeleteAsync(int LeadStatusID);
        Task<int> UpdateStatusAsync(int LeadStatusID, bool status);
        Task<bool> FindByNameAsync(string LeadStatus);
    }
}
