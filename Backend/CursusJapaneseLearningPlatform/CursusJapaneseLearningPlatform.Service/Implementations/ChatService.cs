using AutoMapper;
using CursusJapaneseLearningPlatform.Repository.Interfaces;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using Microsoft.AspNetCore.Http;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using CursusJapaneseLearningPlatform.Service.BusinessModels.ChatModels.Responses;

namespace CursusJapaneseLearningPlatform.Service.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel<List<ChatResponseModel>>> GetAllChatsAsync()
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var chats = await _unitOfWork.ChatRepository.GetAllChatsAsync();
                var responseModel = _mapper.Map<List<ChatResponseModel>>(chats);
                _unitOfWork.CommitTransaction();
                return BaseResponseModel<List<ChatResponseModel>>.OkResponseModel(responseModel);
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

        public async Task<BaseResponseModel<ChatResponseModel>> GetChatByIdAsync(Guid chatId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var chat = await _unitOfWork.ChatRepository.GetChatByIdAsync(chatId);
                if (chat == null)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        ResponseMessages.CHAT_NOT_FOUND);
                }
                var responseModel = _mapper.Map<ChatResponseModel>(chat);
                _unitOfWork.CommitTransaction();
                return BaseResponseModel<ChatResponseModel>.OkResponseModel(responseModel);
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

        public async Task<BaseResponseModel<List<ChatResponseModel>>> GetChatsByUserIdAsync(Guid userId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var chats = await _unitOfWork.ChatRepository.GetChatsByUserIdAsync(userId);
                var responseModel = _mapper.Map<List<ChatResponseModel>>(chats);
                _unitOfWork.CommitTransaction();
                return BaseResponseModel<List<ChatResponseModel>>.OkResponseModel(responseModel);
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

        public async Task<BaseResponseModel<ChatResponseModel>> CreateChatAsync(Guid userId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var chat = await _unitOfWork.ChatRepository.CreateChatAsync(userId);
                var responseModel = _mapper.Map<ChatResponseModel>(chat);
                _unitOfWork.CommitTransaction();
                return BaseResponseModel<ChatResponseModel>.OkResponseModel(responseModel);
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

        public async Task<BaseResponseModel<bool>> DeleteChatAsync(Guid chatId, Guid userId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var result = await _unitOfWork.ChatRepository.DeleteChatAsync(chatId, userId);
                if (!result)
                {
                    throw new CustomException(StatusCodes.Status404NotFound,
                        ResponseCodeConstants.NOT_FOUND,
                        ResponseMessages.CHAT_NOT_FOUND);
                }
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

        public async Task<BaseResponseModel<bool>> IsChatExistAsync(Guid chatId)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var result = await _unitOfWork.ChatRepository.IsChatExistAsync(chatId);
                _unitOfWork.CommitTransaction();
                return BaseResponseModel<bool>.OkResponseModel(result);
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

