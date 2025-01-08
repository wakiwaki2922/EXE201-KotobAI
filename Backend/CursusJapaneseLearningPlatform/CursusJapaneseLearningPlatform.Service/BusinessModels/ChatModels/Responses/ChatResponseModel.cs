using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.BusinessModels.ChatModels.Responses
{
    public class ChatResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool ChatStatus { get; set; }
        public List<MessageResponseModel> Messages { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
    }

    public class MessageResponseModel
    {
        public Guid Id { get; set; }
        public string MessageContent { get; set; }
        public string SenderType { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
    }
}
