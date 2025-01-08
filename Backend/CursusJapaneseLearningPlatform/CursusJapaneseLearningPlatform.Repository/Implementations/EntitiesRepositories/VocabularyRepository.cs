using CursusJapaneseLearningPlatform.Repository.Implementations.GenericRepositories;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using Microsoft.EntityFrameworkCore;
using CursusJapaneseLearningPlatform.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Repository.Interfaces.VocabularyManagementRepositories;

namespace CursusJapaneseLearningPlatform.Repository.Implementations.EntitiesRepositories
{
        

        // Repository Implementation
        public class VocabularyRepository : GenericRepository<Vocabulary>, IVocabularyRepository
        {
            private readonly ApplicationDbContext _context;

            public VocabularyRepository(ApplicationDbContext dbContext) : base(dbContext)
            {
                _context = dbContext;
            }
            public async Task<Vocabulary?> GetByWordAsync(string word)
            {
                return await _context.Vocabularies
                    .Include(v => v.Flashcards)
                    .FirstOrDefaultAsync(v => v.Word.ToLower() == word.ToLower() &&
                                            v.IsActive &&
                                            !v.IsDelete);
            }
        public async Task<IEnumerable<Vocabulary>> GetVocabulariesByWordAsync(string word)
            {
                return await _context.Vocabularies
                    .Where(v => v.Word.Contains(word) && v.IsActive && !v.IsDelete)
                    .Include(v => v.Flashcards)
                    .ToListAsync();
            }

            public async Task<IEnumerable<Vocabulary>> GetActiveVocabulariesAsync()
            {
                return await _context.Vocabularies
                    .Where(v => v.IsActive && !v.IsDelete)
                    .ToListAsync();
            }
            public async Task<bool> ExistsAsync(string word)
            {
                return await _context.Vocabularies
                    .AnyAsync(v => v.Word.ToLower() == word.ToLower() &&
                                  v.IsActive &&
                                  !v.IsDelete);
            }
        }

}
