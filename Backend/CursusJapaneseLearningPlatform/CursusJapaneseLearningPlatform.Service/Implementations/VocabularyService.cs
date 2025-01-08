using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using Microsoft.AspNetCore.Http;
using CursusJapaneseLearningPlatform.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Service.BusinessModels.VocabularyModels;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace CursusJapaneseLearningPlatform.Service.Implementations
{


    // Service Implementation
    public class VocabularyService : IVocabularyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public VocabularyService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["ExternalApiSettings:JishoBaseUrl"])
            };
        }
        public async Task<BaseResponseModel<VocabularyResponseModel>> SearchApiWord(string word)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // Check if vocabulary exists in the local database
                var existingVocabulary = await _unitOfWork.VocabularyRepository.GetByWordAsync(word);
                if (existingVocabulary != null)
                {
                    var existingResponseModel = _mapper.Map<VocabularyResponseModel>(existingVocabulary);
                    _unitOfWork.CommitTransaction();
                    return BaseResponseModel<VocabularyResponseModel>.OkResponseModel(existingResponseModel);
                }

                // If vocabulary doesn't exist, search it using the external API
                var apiResponse = await CallExternalApiToSearchWord(word);

                // Save the new vocabulary to the local database
                var vocabulary = new Vocabulary
                {
                    Word = word,
                    Meaning = apiResponse.Meaning,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow,
                    CreatedBy = "User",
                    LastUpdatedBy = "User",
                    IsActive = true,
                    IsDelete = false
                };

                var createdVocabulary = await _unitOfWork.VocabularyRepository.AddAsync(vocabulary);
                await _unitOfWork.SaveAsync();

                var responseModel = _mapper.Map<VocabularyResponseModel>(createdVocabulary);

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<VocabularyResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception)
            {
                _unitOfWork.RollBack();
                throw new CustomException(
                    StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR
                );
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ExternalApiResponseModel> CallExternalApiToSearchWord(string word)
        {
            var url = $"?keyword={word}&exactly=true";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<JishoApiResponseModel>();

            if (apiResponse?.Data?.Any() != true)
            {
                throw new CustomException(
                    StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    $"Word '{word}' not found in the external API."
                );
            }

            var firstResult = apiResponse.Data[0];
            return new ExternalApiResponseModel
            {
                Word = firstResult.Slug,
                Meaning = string.Join(", ", firstResult.Senses?
                    .SelectMany(s => s.EnglishDefinitions ?? new List<string>())
                    .Where(def => !string.IsNullOrEmpty(def))
                    .Take(3) ?? Array.Empty<string>())
            };
        }
        public async Task<BaseResponseModel<VocabularyResponseModel>> CreateVocabulary(VocabularyRequestModel request)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // Check if vocabulary exists
                if (await _unitOfWork.VocabularyRepository.ExistsAsync(request.Word))
                {
                    // Get existing vocabulary with related data
                    var existingVocabulary = await _unitOfWork.VocabularyRepository.GetByWordAsync(request.Word);
                    if (existingVocabulary != null)
                    {
                        var existingResponseModel = _mapper.Map<VocabularyResponseModel>(existingVocabulary);
                        _unitOfWork.CommitTransaction();
                        return BaseResponseModel<VocabularyResponseModel>.OkResponseModel(existingResponseModel);
                    }
                }

                // If vocabulary doesn't exist, create new one
                var vocabulary = new Vocabulary
                {
                    Word = request.Word,
                    Meaning = request.Meaning,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow,
                    CreatedBy = "User",
                    LastUpdatedBy = "User",
                    IsActive = true,
                    IsDelete = false
                };

                var createdVocabulary = await _unitOfWork.VocabularyRepository.AddAsync(vocabulary);
                await _unitOfWork.SaveAsync();

                var responseModel = _mapper.Map<VocabularyResponseModel>(createdVocabulary);

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<VocabularyResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception)
            {
                _unitOfWork.RollBack();
                throw new CustomException(
                    StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR
                );
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task<BaseResponseModel<VocabularyResponseModel>> UpdateVocabulary(Guid vocabularyId, VocabularyRequestModel request)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var vocabulary = await _unitOfWork.VocabularyRepository.GetByIdAsync(vocabularyId);
                if (vocabulary == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Vocabulary not found");
                }

                vocabulary.Word = request.Word;
                vocabulary.Meaning = request.Meaning;
                vocabulary.LastUpdatedTime = DateTime.UtcNow;
                vocabulary.LastUpdatedBy = "User";

                await _unitOfWork.VocabularyRepository.UpdateAsync(vocabulary);
                await _unitOfWork.SaveAsync();

                var responseModel = _mapper.Map<VocabularyResponseModel>(vocabulary);
                _unitOfWork.CommitTransaction();

                return BaseResponseModel<VocabularyResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception)
            {
                _unitOfWork.RollBack();
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

        public async Task<BaseResponseModel<VocabularyResponseModel>> GetVocabularyById(Guid vocabularyId)
        {
            try
            {
                var vocabulary = await _unitOfWork.VocabularyRepository.GetByIdAsync(vocabularyId);
                if (vocabulary == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Vocabulary not found");
                }

                var responseModel = _mapper.Map<VocabularyResponseModel>(vocabulary);
                return BaseResponseModel<VocabularyResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
        }

        public async Task<BaseResponseModel<IEnumerable<VocabularyResponseModel>>> GetVocabulariesByWord(string word)
        {
            try
            {
                var vocabularies = await _unitOfWork.VocabularyRepository.GetVocabulariesByWordAsync(word);
                var responseModels = _mapper.Map<IEnumerable<VocabularyResponseModel>>(vocabularies);
                return BaseResponseModel<IEnumerable<VocabularyResponseModel>>.OkResponseModel(responseModels);
            }
            catch (Exception)
            {
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
        }

        public async Task<BaseResponseModel<IEnumerable<VocabularyResponseModel>>> GetActiveVocabularies()
        {
            try
            {
                var vocabularies = await _unitOfWork.VocabularyRepository.GetActiveVocabulariesAsync();
                var responseModels = _mapper.Map<IEnumerable<VocabularyResponseModel>>(vocabularies);
                return BaseResponseModel<IEnumerable<VocabularyResponseModel>>.OkResponseModel(responseModels);
            }
            catch (Exception)
            {
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
        }

        public async Task<BaseResponseModel<bool>> DeleteVocabulary(Guid vocabularyId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var vocabulary = await _unitOfWork.VocabularyRepository.GetByIdAsync(vocabularyId);
                if (vocabulary == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Vocabulary not found");
                }

                vocabulary.IsDelete = true;
                vocabulary.DeletedTime = DateTime.UtcNow;
                vocabulary.DeletedBy = "User";
                vocabulary.IsActive = false;

                await _unitOfWork.VocabularyRepository.UpdateAsync(vocabulary);
                await _unitOfWork.SaveAsync();

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<bool>.OkResponseModel(true);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception)
            {
                _unitOfWork.RollBack();
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
    }
}
