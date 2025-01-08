using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Repository.Migrations;
using CursusJapaneseLearningPlatform.Service.BusinessModels.PaymentModels;
using CursusJapaneseLearningPlatform.Service.BusinessModels.SubcriptionModels;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
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
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayPalClient _paypalClient;
        private readonly IMapper _mapper;

        public PaymentService(IUnitOfWork unitOfWork, IPayPalClient paypalClient, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _paypalClient = paypalClient;
            _mapper = mapper;
        }
        public async Task<BaseResponseModel<IEnumerable<PaymentHistoryDto>>> GetAllPaymentHistoryAsync()
        {
            try
            {
                // Retrieve all payments
                var payments = await _unitOfWork.PaymentRepository.GetAllPaymentHistoryAsync();

                if (payments == null || !payments.Any())
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND);
                }

                // Manually map Payment to PaymentHistoryDto
                var paymentDtos = payments.Select(payment => new PaymentHistoryDto
                {
                    Id = payment.Id,
                    UserFullName = payment.User.FullName, // Assuming User has a FullName property
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod, // Assuming PaymentMethod exists on Payment
                    PaymentStatus = payment.PaymentStatus, // Assuming PaymentStatus exists on Payment
                    PackageName = payment.Subscription?.Package?.PlanName, // Assuming Package has a Name property
                    StartDate = payment.Subscription?.StartDate ?? DateTime.MinValue, // Assuming Subscription has StartDate
                    EndDate = payment.Subscription?.EndDate ?? DateTime.MinValue, // Assuming Subscription has EndDate
                    CreatedTime = payment.CreatedTime
                }).ToList();

                return BaseResponseModel<IEnumerable<PaymentHistoryDto>>.OkResponseModel(paymentDtos);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    ex);
            }
        }

        public async Task<BaseResponseModel<IEnumerable<PaymentHistoryDto>>> GetPaymentHistoryByUserIdAsync(Guid userId)
        {
            try
            {
                // Retrieve all payments for the given userId
                var payments = await _unitOfWork.PaymentRepository.GetPaymentHistoryByUserIdAsync(userId);

                if (payments == null || !payments.Any())
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND);
                }

                // Manually map the Payment entities to PaymentHistoryDto
                var paymentDtos = payments.Select(payment => new PaymentHistoryDto
                {
                    Id = payment.Id,
                    UserFullName = payment.User.FullName, // Assuming User has FullName property
                    Amount = payment.Amount,
                    PaymentMethod = payment.PaymentMethod, // Assuming PaymentMethod exists on Payment
                    PaymentStatus = payment.PaymentStatus, // Assuming PaymentStatus exists on Payment
                    PackageName = payment.Subscription?.Package?.PlanName, // Assuming Package has Name property
                    StartDate = payment.Subscription?.StartDate ?? DateTime.MinValue, // Assuming Subscription has StartDate
                    EndDate = payment.Subscription?.EndDate ?? DateTime.MinValue, // Assuming Subscription has EndDate
                    CreatedTime = payment.CreatedTime
                }).ToList();

                return BaseResponseModel<IEnumerable<PaymentHistoryDto>>.OkResponseModel(paymentDtos);
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
        public async Task<BaseResponseModel<string>> CreatePayment(SubscriptionRequestModel request, Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var package = await _unitOfWork.PackageRepository.GetByIdAsync(request.PackageId);
                var hasActiveSubscription = await _unitOfWork.SubscriptionRepository.HasActiveSubscriptionAsync(userId);

                if (package == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND);
                }

                if (hasActiveSubscription)
                {
                    throw new CustomException(
                        StatusCodes.Status400BadRequest,
                        "User already has an active subscription. Please wait until the current subscription expires.");
                }

                // Create PayPal payment
                var paypalPayment = await _paypalClient.CreatePayment(
                    package.Price,
                    "USD",
                    "sale",
                    $"Subscription for {package.PlanName}",
                    request.ReturnUrl,
                    request.CancelUrl,
                    cancellationToken
                );

                // Create subscription record first to get the ID
                var subscription = new Subscription
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    PackageId = package.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(package.Period),
                    Amount = package.Price,
                    TokenLimit = 1000,
                    IsActive = false,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    LastUpdatedBy = userId.ToString(),
                    IsDelete = false
                };

                await _unitOfWork.SubscriptionRepository.AddAsync(subscription);

                // Create payment record
                var payment = new Repository.Entities.Payment
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SubscriptionId = subscription.Id,
                    Amount = package.Price,
                    PaymentMethod = "PayPal",
                    PaymentStatus = "Pending",
                    PaypalPaymentId = paypalPayment.Id,
                    CreatedTime = DateTime.UtcNow,
                    LastUpdatedTime = DateTime.UtcNow,
                    CreatedBy = userId.ToString(),
                    LastUpdatedBy = userId.ToString(),
                    IsActive = true,
                    IsDelete = false
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveAsync();

                var approvalUrl = paypalPayment.Links.First(l => l.Rel == "approval_url").Href;
                _unitOfWork.CommitTransaction();

                return BaseResponseModel<string>.OkResponseModel(approvalUrl);
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

        public async Task<BaseResponseModel<PaymentResponseModel>> CompletePayment(string paypalPaymentId, string payerId, string token, CancellationToken cancellationToken = default)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                await _paypalClient.ExecutePayment(paypalPaymentId, payerId, token, cancellationToken);

                // Find payment by PayPal payment ID
                var payment = await _unitOfWork.PaymentRepository.GetByPaypalPaymentIdAsync(paypalPaymentId);
                if (payment == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND);
                }

                // Update payment
                payment.PaymentStatus = "Completed";
                payment.LastUpdatedTime = DateTime.UtcNow;
                _unitOfWork.PaymentRepository.Update(payment);

                // Update subscription
                var subscription = await _unitOfWork.SubscriptionRepository.GetByIdAsync(payment.SubscriptionId);
                if (subscription != null)
                {
                    subscription.IsActive = true;
                    subscription.LastUpdatedTime = DateTime.UtcNow;
                    _unitOfWork.SubscriptionRepository.Update(subscription);
                }

                await _unitOfWork.SaveAsync();

                var responseModel = new PaymentResponseModel
                {
                    PaymentId = payment.Id,
                    SubscriptionId = payment.SubscriptionId,
                    Amount = payment.Amount,
                    PaymentStatus = payment.PaymentStatus,
                    PaymentMethod = payment.PaymentMethod,
                    CreatedTime = payment.CreatedTime,
                    LastUpdatedTime = payment.LastUpdatedTime
                };

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<PaymentResponseModel>.OkResponseModel(responseModel);
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