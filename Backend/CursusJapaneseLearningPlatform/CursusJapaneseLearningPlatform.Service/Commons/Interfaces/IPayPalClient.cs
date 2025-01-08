using CursusJapaneseLearningPlatform.Service.BusinessModels.PayPalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Commons.Interfaces
{
    public interface IPayPalClient
    {
        Task<PayPalPayment> CreatePayment(decimal amount, string currency, string intent, string description, string returnUrl, string cancelUrl, CancellationToken cancellationToken = default);
        Task<bool> ExecutePayment(string paymentId, string payerId, string token, CancellationToken cancellationToken = default);
    }
}
