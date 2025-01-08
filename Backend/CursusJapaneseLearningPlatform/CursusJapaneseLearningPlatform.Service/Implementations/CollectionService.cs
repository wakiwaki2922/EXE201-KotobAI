using CursusJapaneseLearningPlatform.Repository.Implementations.CollectionManagementRepositories;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Repository.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Repository.Implementations;
using static StackExchange.Redis.Role;
using Azure.Core;
using CursusJapaneseLearningPlatform.Repository.Migrations;
using Microsoft.EntityFrameworkCore;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using CursusJapaneseLearningPlatform.Service.BusinessModels.CollectionModels;

namespace CursusJapaneseLearningPlatform.Service.Implementations
{


    public class CollectionService : ICollectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CollectionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<CollectionResponseModel>> CreateCollection(CollectionRequestModel request)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var hasActiveSubscription = await _unitOfWork.SubscriptionRepository.HasActiveSubscriptionAsync(request.UserId);

                if (!hasActiveSubscription)
                {
                    var userCollections = await _unitOfWork.CollectionRepository.GetCollectionsByUserIdAsync(request.UserId);
                    if (userCollections.Count() >= 5)
                    {
                        throw new CustomException(
                            StatusCodes.Status400BadRequest,
                            "Free users can only create up to 5 collections. Please subscribe to create more collections.");
                    }
                }

                var collection = new Repository.Entities.Collection
                {
                    CollectionName = request.CollectionName,
                    Description = request.Description,
                    UserId = request.UserId,
                    Status = request.Status,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow,
                    CreatedBy = "User",
                    LastUpdatedBy = "User",
                    IsActive = true,
                    IsDelete = false
                };

                var createdCollection = await _unitOfWork.CollectionRepository.AddAsync(collection);
                await _unitOfWork.SaveAsync();

                var responseModel = _mapper.Map<CollectionResponseModel>(createdCollection);
                _unitOfWork.CommitTransaction();

                return BaseResponseModel<CollectionResponseModel>.OkResponseModel(responseModel);
            }
            catch (CustomException)
            {
                _unitOfWork.RollBack();
                throw;
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
        }

        public async Task<BaseResponseModel<CollectionResponseModel>> GetCollectionWithFlashcards(Guid collectionId)
        {
            try
            {
                var collection = await _unitOfWork.CollectionRepository.GetCollectionWithFlashcardsAsync(collectionId);
                if (collection == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Collection not found");
                }

                var responseModel = _mapper.Map<CollectionResponseModel>(collection);
                return BaseResponseModel<CollectionResponseModel>.OkResponseModel(responseModel);
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

        public async Task<BaseResponseModel<bool>> UpdateCollectionStatus(Guid collectionId, bool status)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                await _unitOfWork.CollectionRepository.UpdateStatusAsync(collectionId, status);
                _unitOfWork.CommitTransaction();

                return BaseResponseModel<bool>.OkResponseModel(true);
            }
            catch (ArgumentException ex)
            {
                _unitOfWork.RollBack();
                throw new CustomException(StatusCodes.Status404NotFound,
                    ResponseCodeConstants.NOT_FOUND,
                    ex.Message);
            }
            catch (Exception)
            {
                _unitOfWork.RollBack();
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ResponseMessages.INTERNAL_SERVER_ERROR);
            }
        }

        public async Task<BaseResponseModel<CollectionResponseModel>> UpdateCollection(Guid collectionId, CollectionRequestModel request)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var collection = await _unitOfWork.CollectionRepository.GetByIdAsync(collectionId);
                if (collection == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Collection not found");
                }

                collection.CollectionName = request.CollectionName;
                collection.Description = request.Description;
                collection.Status = request.Status;
                collection.LastUpdatedBy = "User";

                await _unitOfWork.CollectionRepository.UpdateAsync(collection);
                await _unitOfWork.SaveAsync();

                var responseModel = _mapper.Map<CollectionResponseModel>(collection);
                _unitOfWork.CommitTransaction();

                return BaseResponseModel<CollectionResponseModel>.OkResponseModel(responseModel);
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

        public async Task<BaseResponseModel<CollectionResponseModel>> GetCollectionById(Guid collectionId)
        {
            try
            {
                var collection = await _unitOfWork.CollectionRepository.GetByIdAsync(collectionId);
                if (collection == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Collection not found");
                }

                var responseModel = _mapper.Map<CollectionResponseModel>(collection);
                return BaseResponseModel<CollectionResponseModel>.OkResponseModel(responseModel);
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

        public async Task<BaseResponseModel<IEnumerable<CollectionResponseModel>>> GetCollectionsByUserId(Guid userId)
        {
            try
            {
                var collections = await _unitOfWork.CollectionRepository.GetCollectionsByUserIdAsync(userId);
                var responseModels = _mapper.Map<IEnumerable<CollectionResponseModel>>(collections);
                return BaseResponseModel<IEnumerable<CollectionResponseModel>>.OkResponseModel(responseModels);
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

        public async Task<BaseResponseModel<IEnumerable<CollectionResponseModel>>> GetCollectionsByName(string collectionName)
        {
            try
            {
                var collections = await _unitOfWork.CollectionRepository.GetCollectionsByNameAsync(collectionName);
                var responseModels = _mapper.Map<IEnumerable<CollectionResponseModel>>(collections);
                return BaseResponseModel<IEnumerable<CollectionResponseModel>>.OkResponseModel(responseModels);
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

        public async Task<BaseResponseModel<IEnumerable<CollectionResponseModel>>> GetActiveSavedCollections()
        {
            try
            {
                var collections = await _unitOfWork.CollectionRepository.GetActiveCollectionsAsync();
                var responseModels = _mapper.Map<IEnumerable<CollectionResponseModel>>(collections);
                return BaseResponseModel<IEnumerable<CollectionResponseModel>>.OkResponseModel(responseModels);
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

        public async Task<BaseResponseModel<bool>> DeleteCollection(Guid collectionId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var collection = await _unitOfWork.CollectionRepository.GetByIdAsync(collectionId);
                if (collection == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        "Collection not found");
                }

                collection.IsDelete = true;
                collection.DeletedTime = DateTime.UtcNow;
                collection.DeletedBy = "User";
                collection.IsActive = false;

                await _unitOfWork.CollectionRepository.UpdateAsync(collection);
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