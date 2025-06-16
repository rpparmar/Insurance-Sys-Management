using AutoMapper;
using Insurancesys.web.Models;
using InsuranceSys.Domain.Entities;

namespace Insurancesys.web.Utility
{
    public sealed class ViewModelDtoMapping : Profile
    {
        public ViewModelDtoMapping()
        {
            #region ViewModel to DTO                        
            CreateMap<UsersViewModel, UsersEntity>();
            CreateMap<LeadViewModel, LeadEntity>();
            CreateMap<CompanyViewModel, CompanyEntity>();
            #endregion

            #region DTO to ViewModel
            CreateMap<UsersEntity, UsersViewModel>(); 
            CreateMap<CompanyEntity, CompanyViewModel>();             
            CreateMap<LeadEntity, LeadViewModel>(); 
            #endregion
        }
    }
}
