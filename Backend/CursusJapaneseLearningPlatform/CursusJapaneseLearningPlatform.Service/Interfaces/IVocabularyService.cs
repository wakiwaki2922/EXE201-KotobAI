using CursusJapaneseLearningPlatform.Service.BusinessModels.VocabularyModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Interfaces
{
    public interface IVocabularyService
    {
        Task<BaseResponseModel<VocabularyResponseModel>> CreateVocabulary(VocabularyRequestModel request);
        Task<BaseResponseModel<VocabularyResponseModel>> UpdateVocabulary(Guid vocabularyId, VocabularyRequestModel request);
        Task<BaseResponseModel<VocabularyResponseModel>> GetVocabularyById(Guid vocabularyId);
        Task<BaseResponseModel<IEnumerable<VocabularyResponseModel>>> GetVocabulariesByWord(string word);
        Task<BaseResponseModel<IEnumerable<VocabularyResponseModel>>> GetActiveVocabularies();
        Task<BaseResponseModel<bool>> DeleteVocabulary(Guid vocabularyId);
        Task<BaseResponseModel<VocabularyResponseModel>> SearchApiWord(string word);
        Task<ExternalApiResponseModel> CallExternalApiToSearchWord(string word);
    }
}
