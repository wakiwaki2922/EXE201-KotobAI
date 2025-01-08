using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Requests
{
    public class MessageRequestModel
    {
        public Guid ChatId { get; set; }
        public string MessageContent { get; set; }
        public string SenderType { get; set; }
    }
}
