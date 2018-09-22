using AutoMapper;
using AutoMapper.Configuration;
using iSeal.API.DTO;
using iSeal.Dal.Contexts;
using iSeal.Dal.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iSeal.API.Configuration
{
    public class MapperConfiguration : Profile
    {

        public MapperConfiguration()
        {
            CreateMap<UserRegister, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Organization, opt => opt.Ignore());
            CreateMap<User, OrganizationUser>();
            CreateMap<SealRegister, Seal>();
            CreateMap<OrganizationRegister, Organization>();
            CreateMap<Seal, SealViewModel>();
            CreateMap<Seal, SealAccessResponse>();
        }
    }
}
