using API.Controllers;
using API.DTO;
using API.Entitites;
using AutoMapper;

namespace API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, UserToReturnDTO>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()))
                .ForMember(dest => dest.photoUrl, opt => opt.MapFrom(src => src.Photos.SingleOrDefault(b => b.IsMain).Url));


            CreateMap<Photo, PhotoDto>();
            CreateMap<UserUpdateDTO, AppUser>();
            CreateMap<RegisterDTO, AppUser>();
        }
    }
}
