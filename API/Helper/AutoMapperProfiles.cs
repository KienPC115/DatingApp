using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helper;

// add package AutoMapper.Extensions.Microsoft.DependencyInjection to use it
// AutoMapper always help us to map from Entity -> Dto or Dto -> Entity with the same Properties's name
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // AutoMapper bwt Entity vs Dto
        CreateMap<AppUser,MemberDto>()
            // That's how we configure an individual mapping for an individual property that automapper doesnt understand what to do with
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url)) // dest -> it represent for MemberDto. src -> it represent for AppUser
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            // .ForMember(dest => dest.KnowAs, opt => opt.MapFrom(src => src.UserName));
        CreateMap<Photo,PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
        CreateMap<RegisterDto, AppUser>();
        CreateMap<Message,MessageDto>()
            .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
            .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
        // Dealing with UTC date formats
        // because DateTime has many kind of time -> now we want to get UTC.
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }


}