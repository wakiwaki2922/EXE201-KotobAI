using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Repository.Interfaces.CollectionManagementRepositories
{
    public interface ICollectionRepository : IGenericRepository<Collection>
    {
        Task<IEnumerable<Collection>> GetCollectionsByUserIdAsync(Guid userId);
        Task<IEnumerable<Collection>> GetCollectionsByNameAsync(string collectionName);
        Task<Collection?> GetCollectionWithFlashcardsAsync(Guid collectionId);
        Task<IEnumerable<Collection>> GetActiveCollectionsAsync();
        Task UpdateStatusAsync(Guid collectionId, bool status);
    }
}
