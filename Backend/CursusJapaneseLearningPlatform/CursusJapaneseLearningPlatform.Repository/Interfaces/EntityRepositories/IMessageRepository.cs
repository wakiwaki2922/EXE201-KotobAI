using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.EntityRepositories
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<Message> CreateMessageByChatId(Message message);
        Task<IEnumerable<Message>> GetAllMessagesByChatId(Guid chatId);
        Task<bool> DeleteMessage(Guid messageId, string deletedBy);
    }
}
