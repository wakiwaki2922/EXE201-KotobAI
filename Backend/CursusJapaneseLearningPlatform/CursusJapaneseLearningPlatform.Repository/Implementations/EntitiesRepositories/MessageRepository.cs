using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.EntityRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Implementations.EntitiesRepositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly ApplicationDbContext _context;
        
        public MessageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<Message> CreateMessageByChatId(Message message)
        {
            message.CreatedTime = DateTimeOffset.UtcNow;
            message.LastUpdatedTime = DateTimeOffset.UtcNow;
            message.IsActive = true;
            message.IsDelete = false;
            
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            
            return message;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesByChatId(Guid chatId)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId && !m.IsDelete)
                .OrderBy(m => m.CreatedTime)
                .ToListAsync();
        }

        public async Task<bool> DeleteMessage(Guid messageId, string deletedBy)
        {
            var message = await _context.Messages.FindAsync(messageId);
            
            if (message == null)
                return false;

            message.IsDelete = true;
            message.DeletedTime = DateTimeOffset.UtcNow;
            message.DeletedBy = deletedBy;
            message.LastUpdatedTime = DateTimeOffset.UtcNow;
            message.LastUpdatedBy = deletedBy;
            
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
