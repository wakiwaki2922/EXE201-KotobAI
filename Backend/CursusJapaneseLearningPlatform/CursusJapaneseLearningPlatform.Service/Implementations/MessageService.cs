using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Entities;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Response;
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
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<MessageResponseModel>> CreateMessageAsync(MessageRequestModel requestModel)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var message = _mapper.Map<Message>(requestModel);

                // Khởi tạo các trường còn thiếu
                message.Id = Guid.NewGuid(); // Tạo ID mới cho message
                message.CreatedBy = "System"; 
                message.LastUpdatedBy = "System";
                message.CreatedTime = DateTimeOffset.UtcNow;
                message.LastUpdatedTime = DateTimeOffset.UtcNow;
                message.IsActive = true;
                message.IsDelete = false;

                var createdMessage = await _unitOfWork.MessageRepository.CreateMessageByChatId(message);
                var responseModel = _mapper.Map<MessageResponseModel>(createdMessage);

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<MessageResponseModel>.OkResponseModel(responseModel);
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


        public async Task<BaseResponseModel<IEnumerable<MessageResponseModel>>> GetAllMessagesByChatIdAsync(Guid chatId)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var messages = await _unitOfWork.MessageRepository.GetAllMessagesByChatId(chatId);
                var responseModels = _mapper.Map<IEnumerable<MessageResponseModel>>(messages);

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<IEnumerable<MessageResponseModel>>.OkResponseModel(responseModels);
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

        public async Task<BaseResponseModel<bool>> DeleteMessageAsync(Guid messageId, string deletedBy)
        {
            try
            {
                _unitOfWork.BeginTransaction();

                var isDeleted = await _unitOfWork.MessageRepository.DeleteMessage(messageId, deletedBy);

                _unitOfWork.CommitTransaction();

                return BaseResponseModel<bool>.OkResponseModel(isDeleted);
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
