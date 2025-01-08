using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.EntityRepositories
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        /// <summary>
        /// Get all chats with their associated users and messages
        /// </summary>
        Task<IEnumerable<Chat>> GetAllChatsAsync();

        /// <summary>
        /// Get a specific chat by its ID
        /// </summary>
        Task<Chat> GetChatByIdAsync(Guid chatId);

        /// <summary>
        /// Get all chats for a specific user
        /// </summary>
        Task<IEnumerable<Chat>> GetChatsByUserIdAsync(Guid userId);

        /// <summary>
        /// Create a new chat for a user
        /// </summary>
        Task<Chat> CreateChatAsync(Guid userId);

        /// <summary>
        /// Delete a chat and its messages
        /// </summary>
        Task<bool> DeleteChatAsync(Guid chatId, Guid userId);

        /// <summary>
        /// Check if a chat exists
        /// </summary>
        Task<bool> IsChatExistAsync(Guid chatId);
    }
}