using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Service.BusinessModels.SubcriptionModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Commons.Implementations;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Implementations
{

    // Services/SubscriptionService.cs
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubscriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<IEnumerable<SubscriptionHistoryDto>>> GetAllSubscriptionHistoryAsync()
        {
            try
            {
                // Retrieve all subscriptions from the repository
                var subscriptions = await _unitOfWork.SubscriptionRepository.GetAllSubscriptionHistoryAsync();

                if (subscriptions == null || !subscriptions.Any())
                {
                    throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND);
                }

                // Map subscriptions to SubscriptionHistoryDto
                var subscriptionDtos = subscriptions.Select(subscription => new SubscriptionHistoryDto
                {
                    Id = subscription.Id,
                    UserFullName = subscription.User.FullName,
                    PackageName = subscription.Package.PlanName,  // Assuming PlanName is a field in the Package
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    Amount = subscription.Amount,
                    PaymentStatus = subscription.Payment?.PaymentStatus ?? "Pending", // Handle null PaymentStatus
                    CreatedTime = subscription.CreatedTime
                }).ToList();

                return BaseResponseModel<IEnumerable<SubscriptionHistoryDto>>.OkResponseModel(subscriptionDtos);
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

        public async Task<BaseResponseModel<IEnumerable<SubscriptionHistoryDto>>> GetSubscriptionHistoryByUserIdAsync(Guid userId)
        {
            try
            {
                // Retrieve subscriptions by userId from the repository
                var subscriptions = await _unitOfWork.SubscriptionRepository.GetSubscriptionHistoryByUserIdAsync(userId);

                if (subscriptions == null || !subscriptions.Any())
                {
                    throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND);
                }

                // Map subscriptions to SubscriptionHistoryDto
                var subscriptionDtos = subscriptions.Select(subscription => new SubscriptionHistoryDto
                {
                    Id = subscription.Id,
                    UserFullName = subscription.User.FullName,
                    PackageName = subscription.Package.PlanName,  // Assuming PlanName is a field in the Package
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    Amount = subscription.Amount,
                    PaymentStatus = subscription.Payment?.PaymentStatus ?? "Pending", // Handle null PaymentStatus
                    CreatedTime = subscription.CreatedTime
                }).ToList();

                return BaseResponseModel<IEnumerable<SubscriptionHistoryDto>>.OkResponseModel(subscriptionDtos);
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
    }
}
