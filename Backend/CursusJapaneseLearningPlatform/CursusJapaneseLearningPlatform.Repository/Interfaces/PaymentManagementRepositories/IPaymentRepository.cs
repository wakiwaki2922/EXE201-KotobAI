using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.PaymentManagementRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment> GetBySubscriptionIdAsync(Guid subscriptionId);
        Task<Payment> GetByPaypalPaymentIdAsync(string paypalPaymentId);
        Task<IEnumerable<Payment>> GetAllPaymentHistoryAsync();
        Task<IEnumerable<Payment>> GetPaymentHistoryByUserIdAsync(Guid userId);
    }
}
