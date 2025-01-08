using CursusJapaneseLearningPlatform.Service.BusinessModels.FlashcardModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Interfaces
{
    public interface IFlashcardService
    {
        Task<BaseResponseModel<FlashcardResponseModel>> CreateAsync(CreateFlashcardDto dto);
        Task<BaseResponseModel<bool>> DeleteAsync(Guid id);
        Task<BaseResponseModel<FlashcardResponseModel>> GetByIdAsync(Guid id);
        Task<BaseResponseModel<List<FlashcardResponseModel>>> GetAllByCollectionIdAsync(Guid collectionId);
    }
}
