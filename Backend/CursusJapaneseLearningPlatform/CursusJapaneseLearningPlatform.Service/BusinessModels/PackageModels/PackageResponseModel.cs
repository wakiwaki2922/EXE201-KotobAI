using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Repository.Entities;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.PackageModels
{
    public class PackageResponseModel
    {
        public Guid Id { get; set; }
        public string PlanType { get; set; }
        public string PlanName { get; set; }
        public int Period { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public bool IsDelete { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }

        public PackageResponseModel(Package package)
        {
            Id = package.Id;
            PlanType = package.PlanType;
            PlanName = package.PlanName;
            Period = package.Period;
            Price = package.Price;
            Status = package.IsActive;
            IsDelete = package.IsDelete;
            CreatedTime = package.CreatedTime;
            LastUpdatedTime = package.LastUpdatedTime;
        }
    }
}
