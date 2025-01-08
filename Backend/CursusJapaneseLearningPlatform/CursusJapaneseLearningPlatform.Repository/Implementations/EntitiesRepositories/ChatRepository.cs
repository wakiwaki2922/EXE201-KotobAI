using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.EntityRepositories;
using Microsoft.EntityFrameworkCore;

namespace CursusJapaneseLearningPlatform.Repository.Implementations.EntitiesRepositories
{
    public class ChatRepository : GenericRepository<Chat>, IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            return await _context.Chats
                .Include(c => c.User)
                .Include(c => c.Messages.Where(m => !m.IsDelete))
                .OrderByDescending(c => c.CreatedTime)
                .ToListAsync();
        }

        public async Task<Chat> GetChatByIdAsync(Guid chatId)
        {
            return await _context.Chats
                .Include(c => c.User)
                .Include(c => c.Messages.Where(m => !m.IsDelete))
                .FirstOrDefaultAsync(c => c.Id == chatId && !c.IsDelete);
        }

        public async Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId)
        {
            return await _context.Chats
                .Include(c => c.User)
                .Include(c => c.Messages.Where(m => !m.IsDelete))
                .Where(c => c.UserId == userId && !c.IsDelete)
                .OrderByDescending(c => c.CreatedTime)
                .ToListAsync();
        }

        public async Task<Chat> CreateChatAsync(Guid userId)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ChatStatus = true,
                CreatedTime = DateTimeOffset.UtcNow,
                LastUpdatedTime = DateTimeOffset.UtcNow,
                CreatedBy = userId.ToString(),
                LastUpdatedBy = userId.ToString(),
                IsActive = true,
                IsDelete = false
            };

            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();

            return await GetChatByIdAsync(chat.Id);
        }

        public async Task<bool> DeleteChatAsync(Guid chatId, Guid userId)
        {
            var chat = await GetChatByIdAsync(chatId);
            if (chat == null)
            {
                return false;
            }

            chat.IsDelete = true;
            chat.DeletedTime = DateTimeOffset.UtcNow;
            chat.DeletedBy = userId.ToString();

            var messages = await _context.Messages
                .Where(m => m.ChatId == chatId && !m.IsDelete)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsDelete = true;
                message.DeletedTime = DateTimeOffset.UtcNow;
                message.DeletedBy = userId.ToString();
            }

            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsChatExistAsync(Guid chatId)
        {
            return await _context.Chats
                .AnyAsync(c => c.Id == chatId && !c.IsDelete);
        }
    }
}