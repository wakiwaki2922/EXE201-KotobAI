using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.FlashcardRepositories
{
    public interface IFlashcardRepository : IGenericRepository<Flashcard>
    {
        Task<Flashcard> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid collectionId, Guid vocabularyId);
        Task<Flashcard> GetByCollectionAndWordAsync(Guid collectionId, string word);
        Task<Flashcard> GetByIdWithVocabularyAsync(Guid id);
        Task<List<Flashcard>> GetAllByCollectionIdAsync(Guid collectionId);
    }
}
