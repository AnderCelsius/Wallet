using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Dtos.Authentication;
using Wallet.Models;

namespace Wallet.Utils.Mapper
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<RegisterUserDto, AppUser>()
                .ForMember(dest => dest.Name, y => y.MapFrom(src => src.FirstName + " " + src.LastName)).ReverseMap();
            CreateMap<AppUser, LoginRequestDto>().ReverseMap();
            CreateMap<AppUser, ResetPasswordDto>().ReverseMap();
            CreateMap<AppUser, UpdatePasswordDto>().ReverseMap();
        }
        
    }
}
