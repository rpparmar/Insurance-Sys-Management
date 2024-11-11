using AutoMapper;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain.DTO;
using InsuranceSys.Infrastructure.Database;
using InsuranceSys.Infrastructure.EFEntities;
using InsuranceSys.Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure
{
    public class CountryRepository : ICountryRepository
    {
        private readonly EfdbContext _context;
        private readonly IMapper _mapper;
        public CountryRepository(EfdbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<(IEnumerable<CountryDto> countries, int TotalCount)> GetAllAsync(ImmutableDictionary<string, object> paramCollections)
        {
            // Safely retrieve parameters from the dictionary
            var searchTerm = GetValues.GetValueOrDefault<string>(paramCollections, "searchval");
            var status = GetValues.GetValueOrDefault<bool>(paramCollections, "status");
            var page = GetValues.GetValueOrDefault<int>(paramCollections, "page", 1); // Default to page 1
            var pageSize = GetValues.GetValueOrDefault<int>(paramCollections, "pagesize", 10);   // Default to 10 items per page


            // Start with the base query for CountryMaster
            var query = _context.Countries.AsQueryable();

            query = query.Where(c => c.IsActive == status && !c.IsDeleted);
            // Apply search filter on CountryName and CountryCode if searchTerm is provided
            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(c => c.CountryName.Contains(searchTerm));

            // Get the total count for pagination purposes (before applying Skip/Take)
            var totalCount = await query.CountAsync();

            // Apply sorting, pagination (Skip and Take)
            var countries = await query
                                    .OrderByDescending(c => c.CreatedOn) // Sort by CountryName or any other column
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return (_mapper.Map<IEnumerable<CountryDto>>(countries), totalCount);
        }
        public async Task<CountryDto> GetByIdAsync(int CountryID)
        {
            var country = await _context.Countries
                            .FirstOrDefaultAsync(c => c.CountryID == CountryID);
            return _mapper.Map<CountryDto>(country);
        }
        public async Task<int> GetByName(string CountryName)
        {
            return await _context.Countries.Where(w => w.CountryName == CountryName).CountAsync();
        }
        public async Task<int> AddAsync(CountryDto countryDto)
        {
            var country = _mapper.Map<CountryMaster>(countryDto);
            country.CreatedOn = DateTime.UtcNow;
            country.UpdatedOn = DateTime.UtcNow;
            await _context.Countries.AddAsync(country);
            return await _context.SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(CountryDto countryDto)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.CountryID == countryDto.CountryID);
            if (country != null)
            {
                countryDto.UpdatedOn = DateTime.UtcNow;
                _mapper.Map(countryDto, country);
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(int CountryID)
        {
            var country = await _context.Countries.FindAsync(CountryID);
            if (country != null)
            {
                country.IsDeleted = true;
                country.UpdatedOn = DateTime.UtcNow;
                //_context.Countries.Remove(country);
            }
            return await _context.SaveChangesAsync();
        }
        public async Task<int> UpdateStatusAsync(int CountryID, bool status)
        {
            var country = await _context.Countries.FindAsync(CountryID);
            if (country != null)
            {
                country.IsActive = status;
                country.UpdatedOn = DateTime.UtcNow;
            }
            return await _context.SaveChangesAsync();
        }
    }
}
