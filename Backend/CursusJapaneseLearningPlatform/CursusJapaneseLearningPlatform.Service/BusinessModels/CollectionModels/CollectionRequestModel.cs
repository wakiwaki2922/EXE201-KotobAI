using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.CollectionModels
{
    public class CollectionRequestModel
    {
        public Guid UserId { get; set; }
        public string CollectionName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
