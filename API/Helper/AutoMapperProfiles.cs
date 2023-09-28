using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helper;

// add package AutoMapper.Extensions.Microsoft.DependencyInjection to use it
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // AutoMapper bwt Entity vs Dto
        CreateMap<AppUser,MemberDto>()
            // That's how we configure an individual mapping for an individual property that automapper doesnt understand what to do with
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url)) // dest -> it represent for MemberDto. src -> it represent for AppUser
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo,PhotoDto>();
    }


}