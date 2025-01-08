using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.FlashcardRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Implementations.EntitiesRepositories
{
    public class FlashcardRepository : GenericRepository<Flashcard>, IFlashcardRepository
    {
        private readonly ApplicationDbContext _context;

        public FlashcardRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = _context = dbContext;
            
        }
        public async Task<Flashcard> GetByIdWithVocabularyAsync(Guid id)
        {
            return await _context.Flashcards
                .Include(f => f.Vocabulary)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDelete);
        }

        public async Task<List<Flashcard>> GetAllByCollectionIdAsync(Guid collectionId)
        {
            return await _context.Flashcards
                .Include(f => f.Vocabulary)
                .Where(f => f.CollectionId == collectionId && !f.IsDelete)
                .OrderByDescending(f => f.CreatedTime)
                .ToListAsync();
        }
        public async Task<Flashcard> GetByIdAsync(Guid id)
        {
            return await _context.Flashcards
                .Include(f => f.Collection)
                .Include(f => f.Vocabulary)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        }

        public async Task<bool> ExistsAsync(Guid collectionId, Guid vocabularyId)
        {
            return await _context.Flashcards
                .AnyAsync(x => x.CollectionId == collectionId &&
                              x.VocabularyId == vocabularyId &&
                              !x.IsDelete);
        }
        public async Task<Flashcard> GetByCollectionAndWordAsync(Guid collectionId, string word)
        {
            return await _context.Flashcards
                .Include(f => f.Vocabulary)
                .FirstOrDefaultAsync(f =>
                    f.CollectionId == collectionId &&
                    f.Vocabulary.Word.ToLower() == word.ToLower());
        }
    }
}
