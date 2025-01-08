using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses;

namespace CursusJapaneseLearningPlatform.Service.Commons.Mappers;
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserRequestModel>()
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.UserName))
                .ReverseMap()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.EmailAddress));
        CreateMap<User, UserResponseModel>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))    
            .ReverseMap();
    }
}
