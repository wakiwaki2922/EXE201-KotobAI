using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Service.BusinessModels.ChatModels.Responses;

namespace CursusJapaneseLearningPlatform.Service.Commons.Mappers
{
    public class ChatMappingProfile : Profile
    {
        public ChatMappingProfile()
        {
            CreateMap<Chat, ChatResponseModel>();
            CreateMap<Message, MessageResponseModel>();
        }
    }
}