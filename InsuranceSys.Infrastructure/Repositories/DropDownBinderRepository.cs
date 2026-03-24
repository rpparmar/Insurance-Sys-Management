using AutoMapper;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class DropDownBinderRepository: SharedEFdbContextRepositoryBase, IDropDownBinderService
    {        
        public DropDownBinderRepository(
            IEFdbContextProvider contextProvider
            ) : base(contextProvider)
        {            
        }
        public async Task<List<DropdownItemDto>> GetCompanyDropdownAsync()
        {
            //using var _efdbcontext = await CreateContextAsync();
            //return await _efdbcontext.EFCompanies
            //    .AsNoTracking()
            //    .Where(c => c.IsActive && !c.IsDeleted)
            //    .OrderBy(c => c.CompanyName)
            //    .Select(c => new DropdownItemDto
            //    {
            //        Value = c.CompanyID.ToString(),
            //        Text = c.CompanyName
            //    })
            //    .ToListAsync();

            return await ExecuteReadAsync(async context =>
            {
                // AsNoTracking for read operations (better performance)
                return await context.EFCompanies
                .AsNoTracking()
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.CompanyName)
                .Select(c => new DropdownItemDto
                {
                    Value = c.CompanyID.ToString(),
                    Text = c.CompanyName
                })
                .ToListAsync();
            });
        }
        public async Task<List<DropdownItemDto>> GetCompanyMappedWithInsuranceType(int policyTypeID)
        {
            //using var _efdbcontext = await CreateContextAsync();
            //var result = await (from b in _efdbcontext.EFMappingCompanyInsuranceType
            //                    join a in _efdbcontext.EFCompanies
            //                        on b.CompanyID equals a.CompanyID into companyGroup
            //                    from a in companyGroup.DefaultIfEmpty()
            //                    where b.InsuranceTypeId == policyTypeID
            //                          && (b.IsActive)
            //                          && (a != null && !a.IsDeleted)
            //                    select new DropdownItemDto
            //                    {
            //                        Value = a.CompanyID.ToString(),
            //                        Text = a.CompanyName
            //                    }).Distinct().ToListAsync();
            //return result;

            return await ExecuteReadAsync(async context =>
            {
                return await (from b in context.EFMappingCompanyInsuranceType
                                    join a in context.EFCompanies
                                        on b.CompanyID equals a.CompanyID into companyGroup
                                    from a in companyGroup.DefaultIfEmpty()
                                    where b.InsuranceTypeId == policyTypeID
                                          && (b.IsActive)
                                          && (a != null && !a.IsDeleted)
                                    select new DropdownItemDto
                                    {
                                        Value = a.CompanyID.ToString(),
                                        Text = a.CompanyName
                                    }).Distinct().ToListAsync();
            });
        }
        public async Task<List<DropdownItemDto>> GetInsuranceTypeDropdownAsync()
        {
            //using var _efdbcontext = await CreateContextAsync();
            //return await _efdbcontext.EFInsuranceTypes
            //    .AsNoTracking()
            //    .Where(c => c.IsActive && !c.IsDeleted)
            //    .OrderBy(c => c.InsuranceType)
            //    .Select(c => new DropdownItemDto
            //    {
            //        Value = c.InsuranceTypeId.ToString(),
            //        Text = c.InsuranceType
            //    })
            //    .ToListAsync();

            return await ExecuteReadAsync(async context =>
            {
                // AsNoTracking for read operations (better performance)
                return await context.EFInsuranceTypes
                    .AsNoTracking()
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .OrderBy(c => c.InsuranceType)
                    .Select(c => new DropdownItemDto
                    {
                        Value = c.InsuranceTypeId.ToString(),
                        Text = c.InsuranceType
                    })
                    .ToListAsync();
            });
        }
        public async Task<List<DropdownItemDto>> GetLeadStatusDropdownAsync()
        {
            //using var _efdbcontext = await CreateContextAsync();
            //return await _efdbcontext.EFLeadStatus
            //    .AsNoTracking()
            //    .Where(c => c.IsActive && !c.IsDeleted)
            //    .OrderBy(c => c.LeadStatus)
            //    .Select(c => new DropdownItemDto
            //    {
            //        Value = c.LeadStatusID.ToString(),
            //        Text = c.LeadStatus
            //    })
            //    .ToListAsync();
            return await ExecuteReadAsync(async context =>
            {
                // AsNoTracking for read operations (better performance)
                return await context.EFLeadStatus
                    .AsNoTracking()
                    .Where(c => c.IsActive && !c.IsDeleted)
                    .OrderBy(c => c.LeadStatus)
                    .Select(c => new DropdownItemDto
                    {
                        Value = c.LeadStatusID.ToString(),
                        Text = c.LeadStatus
                    })
                    .ToListAsync();
            });
        }

        public async Task<List<DropdownItemDto>> GetCountryDropdownAsync()
        {
            return await ExecuteReadAsync(async context =>
            {
                return await context.EFCountries
                    .AsNoTracking()
                    .Where(x => x.IsActive && !x.IsDeleted)
                    .OrderBy(x => x.CountryName)
                    .Select(x => new DropdownItemDto
                    {
                        Value = x.CountryID.ToString(),
                        Text = x.CountryName
                    })
                    .ToListAsync();
            });
        }

        public async Task<List<DropdownItemDto>> GetStateDropdownByCountryAsync(int countryID)
        {
            return await ExecuteReadAsync(async context =>
            {
                return await context.EFStates
                    .AsNoTracking()
                    .Where(x => x.CountryID == countryID && x.IsActive && !x.IsDeleted)
                    .OrderBy(x => x.StateName)
                    .Select(x => new DropdownItemDto
                    {
                        Value = x.StateID.ToString(),
                        Text = x.StateName
                    })
                    .ToListAsync();
            });
        }
    }
}
