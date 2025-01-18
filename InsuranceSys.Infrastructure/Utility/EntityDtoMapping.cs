using AutoMapper;
using InsuranceSys.Domain.DTO;
using InsuranceSys.Infrastructure.EFEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSys.Infrastructure.Utility
{
    public sealed class EntityDtoMapping:Profile
    {
        public EntityDtoMapping()
        {
            CreateMap<CountryMaster, CountryDto>().ReverseMap();
            CreateMap<StateMaster, StateDto>().ReverseMap();
            CreateMap<LeadEFEntity, LeadDto>().ReverseMap();
        }        
    }
}
