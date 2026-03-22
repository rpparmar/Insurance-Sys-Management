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
            CreateMap<InsuranceTypeViewModel, InsuranceTypeEntity>();
            CreateMap<LeadStatusViewModel, LeadStatusEntity>();
            CreateMap<CustomerViewModel, CustomerEntity>()
                .ForMember(dest => dest.EncryptedCustomerId, opt => opt.Ignore());
            CreateMap<PolicyBasicDetailsViewModel, PolicyDetailsEntity>();
            CreateMap<PolicyVehicleDetailsViewModel, PolicyVehicleDetailsEntity>();
            CreateMap<PolicyPaymentDetailsViewModel, PolicyPaymentDetailsEntity>();
            #endregion

            #region DTO to ViewModel
            CreateMap<UsersEntity, UsersViewModel>();
            CreateMap<CompanyEntity, CompanyViewModel>();
            CreateMap<LeadEntity, LeadViewModel>();
            CreateMap<InsuranceTypeEntity, InsuranceTypeViewModel>();
            CreateMap<LeadStatusEntity, LeadStatusViewModel>();
            CreateMap<CustomerEntity, CustomerViewModel>();
            CreateMap<PolicyDetailsEntity, PolicyBasicDetailsViewModel>();
            CreateMap<PolicyVehicleDetailsEntity, PolicyVehicleDetailsViewModel>();
            CreateMap<PolicyPaymentDetailsEntity, PolicyPaymentDetailsViewModel>();
            #endregion
        }
    }
}
