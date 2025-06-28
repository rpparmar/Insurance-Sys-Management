using AutoMapper;
using InsuranceSys.Application.DTO;
using InsuranceSys.Application.Interface;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class DropDownBinderRepository: EfRepositoryBase, IDropDownBinderService
    {
        private readonly IAppDBContext _dbcontext;
        private readonly IMapper _mapper;
        public DropDownBinderRepository(
            IAppDBContext dbcontext
            , IMapper mapper
            , IEFdbContextFactory efdbContextFactory
            , IConnectionStringProvider connStringProvider
            ) : base(efdbContextFactory, connStringProvider)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
        }
        public async Task<List<DropdownItemDto>> GetCompanyDropdownAsync()
        {
            using var _efdbcontext = await CreateContextAsync();

            return await _efdbcontext.EFCompanies
                .AsNoTracking()
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.CompanyName)
                .Select(c => new DropdownItemDto
                {
                    Value = c.CompanyID.ToString(),
                    Text = c.CompanyName
                })
                .ToListAsync();
        }
        public async Task<List<DropdownItemDto>> GetCompanyMappedWithInsuranceType(int policyTypeID)
        {
            using var _efdbcontext = await CreateContextAsync();

            var result = await (from b in _efdbcontext.EFMappingCompanyInsuranceType
                                join a in _efdbcontext.EFCompanies
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
            return result;
        }
        public async Task<List<DropdownItemDto>> GetInsuranceTypeDropdownAsync()
        {
            using var _efdbcontext = await CreateContextAsync();

            return await _efdbcontext.EFInsuranceTypes
                .AsNoTracking()
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.InsuranceType)
                .Select(c => new DropdownItemDto
                {
                    Value = c.InsuranceTypeId.ToString(),
                    Text = c.InsuranceType
                })
                .ToListAsync();
        }
    }
}
