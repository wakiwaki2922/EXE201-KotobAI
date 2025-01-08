using CursusJapaneseLearningPlatform.Repository.Bases.BaseEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Entities
{
    public class Payment : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }  // Add this
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }  // Pending, Completed, Failed
        public string PaypalPaymentId { get; set; }  // Add this to store PayPal's payment ID

        // Navigation properties
        public User User { get; set; }
        public Subscription Subscription { get; set; }  // Add this

        // Base entity properties
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
