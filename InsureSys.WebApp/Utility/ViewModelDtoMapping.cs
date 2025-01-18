using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Domain.DTO;

namespace Insurancesys.web.Utility
{
    public sealed class ViewModelDtoMapping : Profile
    {
        public ViewModelDtoMapping()
        {
            #region ViewModel to DTO
            CreateMap<CompanyViewModel, CompanyDto>();
            CreateMap<CountryViewModel, CountryDto>();
            CreateMap<LeadViewModel, LeadDto>();
            #endregion

            #region DTO to ViewModel
            CreateMap<CompanyDto, CompanyViewModel>(); 
            CreateMap<CountryDto, CountryViewModel>(); 
            CreateMap<LeadDto, LeadViewModel>(); 
            #endregion
        }
    }
}
