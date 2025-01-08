using CursusJapaneseLearningPlatform.Service.BusinessModels.PaymentModels;
using CursusJapaneseLearningPlatform.Service.BusinessModels.SubcriptionModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Interfaces
{
    public interface IPaymentService
    {
        Task<BaseResponseModel<IEnumerable<PaymentHistoryDto>>> GetAllPaymentHistoryAsync();
        Task<BaseResponseModel<IEnumerable<PaymentHistoryDto>>> GetPaymentHistoryByUserIdAsync(Guid userId);
        Task<BaseResponseModel<string>> CreatePayment(SubscriptionRequestModel request, Guid userId, CancellationToken cancellationToken = default);
        Task<BaseResponseModel<PaymentResponseModel>> CompletePayment(string paymentId, string payerId, string token, CancellationToken cancellationToken = default);
    }
}
