using CursusJapaneseLearningPlatform.Service.BusinessModels.ChatModels.Responses;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;

namespace CursusJapaneseLearningPlatform.Service.Interfaces
{
    public interface IChatService
    {
        Task<BaseResponseModel<List<ChatResponseModel>>> GetAllChatsAsync();
        Task<BaseResponseModel<ChatResponseModel>> GetChatByIdAsync(Guid chatId);
        Task<BaseResponseModel<List<ChatResponseModel>>> GetChatsByUserIdAsync(Guid userId);
        Task<BaseResponseModel<ChatResponseModel>> CreateChatAsync(Guid userId);
        Task<BaseResponseModel<bool>> DeleteChatAsync(Guid chatId, Guid userId);
        Task<BaseResponseModel<bool>> IsChatExistAsync(Guid chatId);
    }
}
