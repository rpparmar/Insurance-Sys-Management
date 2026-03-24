using InsuranceSys.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Application.Interface
{
    public interface IDropDownBinderService
    {
        Task<List<DropdownItemDto>> GetCompanyMappedWithInsuranceType(int policyTypeID);
        Task<List<DropdownItemDto>> GetInsuranceTypeDropdownAsync();
        Task<List<DropdownItemDto>> GetCompanyDropdownAsync();
        Task<List<DropdownItemDto>> GetLeadStatusDropdownAsync();
        Task<List<DropdownItemDto>> GetCountryDropdownAsync();
        Task<List<DropdownItemDto>> GetStateDropdownByCountryAsync(int countryID);
    }
}
