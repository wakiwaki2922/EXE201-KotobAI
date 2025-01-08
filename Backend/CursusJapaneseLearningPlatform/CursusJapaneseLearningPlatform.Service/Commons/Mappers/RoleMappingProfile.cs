using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CursusJapaneseLearningPlatform.Service.BusinessModels.RoleModels.Responses;
using CursusJapaneseLearningPlatform.Repository.Entities;

namespace CursusJapaneseLearningPlatform.Service.Commons.Mappers;
public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Role, RoleResponseModel>().ReverseMap();
    }
}
