using CursusJapaneseLearningPlatform.Repository.Bases.BaseEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Entities
{

    public class Package : IBaseEntity
    {
        public Guid Id { get; set; }
        public string PlanType { get; set; }
        public string PlanName { get; set; }
        public int Period { get; set; } // Số tháng
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }

        // Base entity properties
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public string CreatedBy { get; set; }
        public string LastUpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public bool IsDelete { get; set; }
    }
}
