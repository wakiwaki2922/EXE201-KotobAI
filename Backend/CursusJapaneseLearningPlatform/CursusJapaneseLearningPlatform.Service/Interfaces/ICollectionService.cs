using CursusJapaneseLearningPlatform.Service.BusinessModels.CollectionModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Interfaces
{
    public interface ICollectionService
    {
        Task<BaseResponseModel<CollectionResponseModel>> CreateCollection(CollectionRequestModel request);
        Task<BaseResponseModel<CollectionResponseModel>> UpdateCollection(Guid collectionId, CollectionRequestModel request);
        Task<BaseResponseModel<CollectionResponseModel>> GetCollectionById(Guid collectionId);
        Task<BaseResponseModel<CollectionResponseModel>> GetCollectionWithFlashcards(Guid collectionId);
        Task<BaseResponseModel<IEnumerable<CollectionResponseModel>>> GetCollectionsByUserId(Guid userId);
        Task<BaseResponseModel<IEnumerable<CollectionResponseModel>>> GetCollectionsByName(string collectionName);
        Task<BaseResponseModel<IEnumerable<CollectionResponseModel>>> GetActiveSavedCollections();
        Task<BaseResponseModel<bool>> DeleteCollection(Guid collectionId);
        Task<BaseResponseModel<bool>> UpdateCollectionStatus(Guid collectionId, bool status);
    }
}
