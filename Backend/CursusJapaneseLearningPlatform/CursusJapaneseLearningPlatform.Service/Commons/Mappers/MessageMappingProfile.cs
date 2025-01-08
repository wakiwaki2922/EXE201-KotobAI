using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Commons.Mappers
{
    public class MessageMappingProfile : Profile
    {
        public MessageMappingProfile()
        {
            CreateMap<MessageRequestModel, Message>()
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.MapFrom(src => DateTimeOffset.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => false));

            CreateMap<Message, MessageResponseModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ChatId, opt => opt.MapFrom(src => src.ChatId))
                .ForMember(dest => dest.MessageContent, opt => opt.MapFrom(src => src.MessageContent))
                .ForMember(dest => dest.SenderType, opt => opt.MapFrom(src => src.SenderType))
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime));
        }
    }
}
