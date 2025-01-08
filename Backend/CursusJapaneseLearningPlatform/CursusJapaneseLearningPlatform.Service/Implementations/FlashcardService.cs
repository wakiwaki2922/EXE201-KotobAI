using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Repository.Migrations;
using CursusJapaneseLearningPlatform.Service.BusinessModels.FlashcardModels;
using CursusJapaneseLearningPlatform.Service.BusinessModels.VocabularyModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Implementations
{
    public class FlashcardService : IFlashcardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVocabularyService _vocabularyService;
        private readonly IMapper _mapper;

        public FlashcardService(
            IUnitOfWork unitOfWork,
            IVocabularyService vocabularyService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _vocabularyService = vocabularyService;
            _mapper = mapper;
        }
        public async Task<BaseResponseModel<FlashcardResponseModel>> GetByIdAsync(Guid id)
        {
            try
            {
                var flashcard = await _unitOfWork.FlashcardRepository.GetByIdWithVocabularyAsync(id);

                if (flashcard == null || flashcard.IsDelete)
                {
                    throw new CustomException(
                        StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Flashcard not found");
                }

                var responseModel = new FlashcardResponseModel
                {
                    Id = flashcard.Id,
                    CollectionId = flashcard.CollectionId,
                    Word = flashcard.Vocabulary.Word,
                    Meaning = flashcard.Vocabulary.Meaning,
                    CreatedTime = flashcard.CreatedTime
                };

                return BaseResponseModel<FlashcardResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new CustomException(
                    StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR
                );
            }
        }

        public async Task<BaseResponseModel<List<FlashcardResponseModel>>> GetAllByCollectionIdAsync(Guid collectionId)
        {
            try
            {
                // First check if collection exists
                var collection = await _unitOfWork.CollectionRepository.GetByIdAsync(collectionId);
                if (collection == null)
                {
                    throw new CustomException(
                        StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Collection not found");
                }

                var flashcards = await _unitOfWork.FlashcardRepository.GetAllByCollectionIdAsync(collectionId);

                var responseModels = flashcards.Select(flashcard => new FlashcardResponseModel
                {
                    Id = flashcard.Id,
                    CollectionId = flashcard.CollectionId,
                    Word = flashcard.Vocabulary.Word,
                    Meaning = flashcard.Vocabulary.Meaning,
                    CreatedTime = flashcard.CreatedTime
                }).ToList();

                return BaseResponseModel<List<FlashcardResponseModel>>.OkResponseModel(responseModels);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception)
            {
                throw new CustomException(
                    StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR
                );
            }
        }

        public async Task<BaseResponseModel<FlashcardResponseModel>> CreateAsync(CreateFlashcardDto dto)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                // Validate collection exists and user has access
                var collection = await _unitOfWork.CollectionRepository.GetByIdAsync(dto.CollectionId);
                if (collection == null)
                {
                    throw new CustomException(
                        StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Collection not found");
                }

                // Check if the word exists in the local database
                var existingVocabulary = await _unitOfWork.VocabularyRepository.GetByWordAsync(dto.Word);
                Guid vocabularyId;
                string meaning = null; // Initialize meaning variable

                if (existingVocabulary != null)
                {
                    // If the word exists, use its ID and meaning
                    vocabularyId = existingVocabulary.Id; // Assuming Id is of type Guid
                    meaning = existingVocabulary.Meaning; // Get the meaning from the existing vocabulary
                }
                else
                {
                    // If the word doesn't exist, search it using the external API
                    var vocabularyResponse = await _vocabularyService.CallExternalApiToSearchWord(dto.Word);
                    meaning = vocabularyResponse.Meaning; // Get the meaning from the API response

                    // Save the new vocabulary to the local database
                    var newVocabulary = new Repository.Entities.Vocabulary
                    {
                        Word = vocabularyResponse.Word,
                        Meaning = meaning,
                        CreatedTime = DateTime.UtcNow,
                        LastUpdatedTime = DateTime.UtcNow,
                        CreatedBy = "User  ",
                        LastUpdatedBy = "User  ",
                        IsActive = true,
                        IsDelete = false
                    };

                    var createdVocabulary = await _unitOfWork.VocabularyRepository.AddAsync(newVocabulary);
                    vocabularyId = createdVocabulary.Id; // Get the ID of the newly created vocabulary
                }

                // Check if flashcard already exists
                var existingFlashcard = await _unitOfWork.FlashcardRepository
                    .GetByCollectionAndWordAsync(dto.CollectionId, dto.Word);

                if (existingFlashcard != null && !existingFlashcard.IsDelete)
                {
                    throw new CustomException(
                        StatusCodes.Status400BadRequest,
                        ResponseCodeConstants.DUPLICATE,
                        "Flashcard already exists in this collection");
                }

                // Create a new flashcard
                var flashcard = new Repository.Entities.Flashcard
                {
                    Id = Guid.NewGuid(),
                    CollectionId = dto.CollectionId,
                    VocabularyId = vocabularyId, // Use the existing or newly created vocabulary ID
                    CreatedTime = DateTimeOffset.UtcNow,
                    LastUpdatedTime = DateTimeOffset.UtcNow,
                    CreatedBy = "User  ",
                    LastUpdatedBy = "User  ",
                    IsActive = true,
                    IsDelete = false
                };

                var createdFlashcard = await _unitOfWork.FlashcardRepository.AddAsync(flashcard);
                await _unitOfWork.SaveAsync();

                var responseModel = new FlashcardResponseModel
                {
                    Id = createdFlashcard.Id,
                    CollectionId = createdFlashcard.CollectionId,
                    Word = dto.Word, // Return the word used for the flashcard
                    Meaning = meaning, // Use the meaning from either existing or newly created vocabulary
                    CreatedTime = createdFlashcard.CreatedTime
                };

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<FlashcardResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception ex)
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

        public async Task<BaseResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var flashcard = await _unitOfWork.FlashcardRepository.GetByIdAsync(id);
                if (flashcard == null || flashcard.IsDelete)
                {
                    throw new CustomException(
                        StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Flashcard not found");
                }

                flashcard.IsDelete = true;
                flashcard.DeletedTime = DateTimeOffset.UtcNow;
                flashcard.DeletedBy = "User";
                flashcard.IsActive = false;
                await _unitOfWork.FlashcardRepository.UpdateAsync(flashcard);
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
    }
}
