using CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Response;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Interfaces
{
    public interface IMessageService
    {
        Task<BaseResponseModel<MessageResponseModel>> CreateMessageAsync(MessageRequestModel requestModel);
        Task<BaseResponseModel<IEnumerable<MessageResponseModel>>> GetAllMessagesByChatIdAsync(Guid chatId);
        Task<BaseResponseModel<bool>> DeleteMessageAsync(Guid messageId, string deletedBy);
    }
}
