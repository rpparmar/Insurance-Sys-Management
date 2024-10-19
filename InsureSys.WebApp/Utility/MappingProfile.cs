using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Domain.DTO;

namespace Insurancesys.web.Utility
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region ViewModel to DTO
            CreateMap<CompanyViewModel, CompanyDto>();
            #endregion

            #region DTO to ViewModel
            CreateMap<CompanyDto, CompanyViewModel>(); 
            #endregion
        }
    }
}
