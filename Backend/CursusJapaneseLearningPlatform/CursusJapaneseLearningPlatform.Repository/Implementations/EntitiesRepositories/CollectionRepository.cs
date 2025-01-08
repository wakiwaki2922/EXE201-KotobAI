using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Repository.Interfaces.CollectionManagementRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Implementations.CollectionManagementRepositories
{
    public class CollectionRepository : GenericRepository<Collection>, ICollectionRepository
    {
        private readonly ApplicationDbContext _context;

        public CollectionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<Collection>> GetCollectionsByUserIdAsync(Guid userId)
        {
            return await _context.Collections
                .Where(c => c.UserId == userId && c.Status)
                .Include(c => c.Flashcards)
                .ToListAsync();
        }

        public async Task<IEnumerable<Collection>> GetCollectionsByNameAsync(string collectionName)
        {
            var searchTerm = collectionName.Trim().ToLower();
            return await _context.Collections
                .Where(c => c.CollectionName.ToLower().Contains(searchTerm) && c.Status)
                .ToListAsync();
        }

        public async Task<Collection?> GetCollectionWithFlashcardsAsync(Guid collectionId)
        {
            return await _context.Collections
                .Include(c => c.Flashcards)
                .FirstOrDefaultAsync(c => c.Id == collectionId && c.Status);
        }

        public async Task<IEnumerable<Collection>> GetActiveCollectionsAsync()
        {
            return await _context.Collections
                .Where(c => c.Status)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(Guid collectionId, bool status)
        {
            var collection = await _context.Collections.FindAsync(collectionId);
            if (collection == null)
            {
                throw new ArgumentException("Collection not found");
            }

            collection.Status = status;
            collection.LastUpdatedTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}