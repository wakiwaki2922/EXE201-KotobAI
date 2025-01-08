using CursusJapaneseLearningPlatform.Repository.Bases.BaseEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Entities
{
    public class PaymentHistory : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid PaymentId { get; set; }
        public string PreviousStatus { get; set; }
        public string NewStatus { get; set; }
        public string SenderType { get; set; }

        public Payment Payment { get; set; }

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
