using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.VocabularyManagementRepositories
{
    public interface IVocabularyRepository : IGenericRepository<Vocabulary>
    {
        Task<IEnumerable<Vocabulary>> GetVocabulariesByWordAsync(string word);
        Task<IEnumerable<Vocabulary>> GetActiveVocabulariesAsync();
        Task<bool> ExistsAsync(string word);
        Task<Vocabulary?> GetByWordAsync(string word);
    }
}
