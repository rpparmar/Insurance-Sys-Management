using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Domain.DTO;
using System.Diagnostics.Metrics;

namespace Insurancesys.web.Utility
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ViewModel to DTO
            CreateMap<CompanyViewModel, CompanyDto>();
            CreateMap<CompanyDto, CompanyViewModel>();
        }
    }
}
