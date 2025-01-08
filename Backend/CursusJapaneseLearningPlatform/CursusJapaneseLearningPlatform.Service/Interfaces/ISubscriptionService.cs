using CursusJapaneseLearningPlatform.Service.BusinessModels.SubcriptionModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Interfaces
{
    public interface ISubscriptionService
    {
        Task<BaseResponseModel<IEnumerable<SubscriptionHistoryDto>>> GetAllSubscriptionHistoryAsync();
        Task<BaseResponseModel<IEnumerable<SubscriptionHistoryDto>>> GetSubscriptionHistoryByUserIdAsync(Guid userId);
    }
}
