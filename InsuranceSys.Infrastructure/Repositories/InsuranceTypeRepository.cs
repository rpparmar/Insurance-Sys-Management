using AutoMapper;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.Entities;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.Database.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Repositories
{
    public class InsuranceTypeRepository : EfRepositoryBase, IInsuranceTypeService
    {
        private readonly IAppDBContext _dbcontext;
        private readonly IMapper _mapper;
        public InsuranceTypeRepository(
            IAppDBContext dbcontext
            , IMapper mapper
            , IEFdbContextFactory efdbContextFactory
            , IConnectionStringProvider connStringProvider) : base(efdbContextFactory, connStringProvider)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
        }
        public async Task<DataSet> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            return await _dbcontext.GetDataSetAsync(paramCollections, CommandType.StoredProcedure, "InsuranceTypeMaster_GetAll");
        }
        public async Task<InsuranceTypeEntity?> GetByIdAsync(int InsuranceTypeId)
        {
            using var _efdbcontext = await CreateContextAsync();

            // Fetch Insurance Type
            var insuranceType = await _efdbcontext.EFInsuranceTypes
                            .FirstOrDefaultAsync(c => c.InsuranceTypeId == InsuranceTypeId);
            if (insuranceType == null)
                return null;
            // Fetch associated company IDs
            var companyIds = await _efdbcontext.EFMappingCompanyInsuranceType
                .Where(m => m.InsuranceTypeId == InsuranceTypeId && m.IsActive == true)
                .Select(m => m.CompanyID.ToString())
                .ToListAsync();
            // Join them into comma-separated string
            insuranceType.AssociationWithCompanyIDs = string.Join(",", companyIds);
            return insuranceType;
        }
        public async Task<int> AddAsync(InsuranceTypeEntity insuranceType)
        {
            using var _efdbcontext = await CreateContextAsync();
            insuranceType.CreatedOn = DateTime.UtcNow;
            insuranceType.UpdatedOn = DateTime.UtcNow;
            await _efdbcontext.EFInsuranceTypes.AddAsync(insuranceType);
            var result = await _efdbcontext.SaveChangesAsync();
            if (result > 0 && !string.IsNullOrWhiteSpace(insuranceType.AssociationWithCompanyIDs))
            {
                // Use the auto-generated primary key (InsuranceTypeId)
                int newInsuranceTypeId = insuranceType.InsuranceTypeId;

                // Split and map company IDs
                var companyMappings = insuranceType.AssociationWithCompanyIDs
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(id => int.TryParse(id, out var companyId) ? new Mapping_Company_InsuranceType
                    {
                        CompanyID = companyId,
                        InsuranceTypeId = newInsuranceTypeId,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow,
                        IsActive = true
                    } : null)
                    .Where(mapping => mapping != null)
                    .ToList();

                // Add all mappings in one go
                if (companyMappings.Any())
                {
                    await _efdbcontext.EFMappingCompanyInsuranceType.AddRangeAsync(companyMappings);
                    await _efdbcontext.SaveChangesAsync();
                }
            }
            return result;
        }
        public async Task<int> UpdateAsync(InsuranceTypeEntity insuranceType)
        {
            using var _efdbcontext = await CreateContextAsync();
            // Fetch current mappings from the DB
            var existingMappings = await _efdbcontext.EFMappingCompanyInsuranceType
                .Where(m => m.InsuranceTypeId == insuranceType.InsuranceTypeId)
                .ToListAsync();

            // Parse the new set of company IDs from AssociationWithCompanyIDs
            var newCompanyIds = insuranceType.AssociationWithCompanyIDs?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(id => int.TryParse(id, out var cid) ? cid : (int?)null)
                .Where(cid => cid.HasValue)
                .Select(cid => cid.Value)
                .ToHashSet() ?? new HashSet<int>();

            var now = DateTime.UtcNow;

            // Handle existing mappings
            foreach (var mapping in existingMappings)
            {
                if (newCompanyIds.Contains(mapping.CompanyID))
                {
                    if (!mapping.IsActive)
                    {
                        // Reactivate if it was inactive
                        mapping.IsActive = true;
                        mapping.UpdatedOn = now;
                    }

                    // Remove from the newCompanyIds to avoid re-adding it below
                    newCompanyIds.Remove(mapping.CompanyID);
                }
                else
                {
                    // Mark as inactive if it exists in DB but not in the new list
                    if (mapping.IsActive)
                    {
                        mapping.IsActive = false;
                        mapping.UpdatedOn = now;
                    }
                }
            }
            // Add fresh new mappings
            var newMappings = newCompanyIds.Select(cid => new Mapping_Company_InsuranceType
            {
                CompanyID = cid,
                InsuranceTypeId = insuranceType.InsuranceTypeId,
                IsActive = true,
                CreatedOn = now,
                UpdatedOn = now
            }).ToList();
            if (newMappings.Any())
                await _efdbcontext.EFMappingCompanyInsuranceType.AddRangeAsync(newMappings);

            insuranceType.UpdatedOn = now;
            _efdbcontext.EFInsuranceTypes.Update(insuranceType);
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int InsuranceTypeId)
        {
            using var _efdbcontext = await CreateContextAsync();
            var _insuranceType = await _efdbcontext.EFInsuranceTypes.FindAsync(InsuranceTypeId);
            if (_insuranceType != null)
            {
                _insuranceType.UpdatedOn = DateTime.UtcNow;
                _insuranceType.IsDeleted = true;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<int> UpdateStatusAsync(int InsuranceTypeId, bool status)
        {
            using var _efdbcontext = await CreateContextAsync();
            var _insuranceType = await _efdbcontext.EFInsuranceTypes.FindAsync(InsuranceTypeId);
            if (_insuranceType != null)
            {
                _insuranceType.UpdatedOn = DateTime.UtcNow;
                _insuranceType.IsActive = status;
            }
            return await _efdbcontext.SaveChangesAsync();
        }
        public async Task<bool> FindByNameAsync(string InsuranceType)
        {
            using var _efdbcontext = await CreateContextAsync();
            return await _efdbcontext.EFInsuranceTypes.AnyAsync(c => c.InsuranceType == InsuranceType);
        }
    }
}
