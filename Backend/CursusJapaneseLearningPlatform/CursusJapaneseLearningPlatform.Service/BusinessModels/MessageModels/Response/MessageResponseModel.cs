using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Response
{
    public class MessageResponseModel
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public string MessageContent { get; set; }
        public string SenderType { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }
}
