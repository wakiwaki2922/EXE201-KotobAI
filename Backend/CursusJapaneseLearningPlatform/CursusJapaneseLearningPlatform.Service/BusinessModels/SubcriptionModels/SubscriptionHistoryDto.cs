using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.SubcriptionModels
{
    public class SubscriptionHistoryDto
    {
        public Guid Id { get; set; }
        public string UserFullName { get; set; }
        public string PackageName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }
}
