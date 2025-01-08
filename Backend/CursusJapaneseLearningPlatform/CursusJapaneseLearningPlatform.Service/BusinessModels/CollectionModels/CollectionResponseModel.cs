using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Repository.Entities;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.CollectionModels
{
    public class CollectionResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string CollectionName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }

        public CollectionResponseModel(Collection collection)
        {
            Id = collection.Id;
            UserId = collection.UserId;
            CollectionName = collection.CollectionName;
            Description = collection.Description;
            Status = collection.Status;
            CreatedTime = collection.CreatedTime;
            LastUpdatedTime = collection.LastUpdatedTime;
        }
    }
}
