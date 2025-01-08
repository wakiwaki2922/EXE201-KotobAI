using CursusJapaneseLearningPlatform.Service.BusinessModels.ChatModels.Responses;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CursusJapaneseLearningPlatform.API.Controllers
{
    /// <summary>
    /// Manages chat operations.
    /// </summary>
    [Route("api/chats")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        /// <summary>
        /// Constructor for ChatController.
        /// </summary>
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Gets all chats.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllChats()
        {
            var result = await _chatService.GetAllChatsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a chat by ID.
        /// </summary>
        [HttpGet("{chatId:guid}")]
        public async Task<IActionResult> GetChatById([FromRoute] Guid chatId)
        {
            var result = await _chatService.GetChatByIdAsync(chatId);
            return Ok(result);
        }

        /// <summary>
        /// Gets chats by user ID.
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetChatsByUserId([FromRoute] Guid userId)
        {
            var result = await _chatService.GetChatsByUserIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a chat for a user.
        /// </summary>
        [HttpPost("user/{userId:guid}")]
        public async Task<IActionResult> CreateChat([FromRoute] Guid userId)
        {
            var result = await _chatService.CreateChatAsync(userId);
            return CreatedAtAction(nameof(GetChatById), new { chatId = result.Data.Id }, result);
        }

        /// <summary>
        /// Deletes a chat for a user.
        /// </summary>
        [HttpDelete("{chatId:guid}/user/{userId:guid}")]
        public async Task<IActionResult> DeleteChat([FromRoute] Guid chatId, [FromRoute] Guid userId)
        {
            var result = await _chatService.DeleteChatAsync(chatId, userId);
            return Ok(result);
        }

        /// <summary>
        /// Checks if a chat exists by ID.
        /// </summary>
        [HttpGet("{chatId:guid}/exists")]
        public async Task<IActionResult> CheckChatExists([FromRoute] Guid chatId)
        {
            var result = await _chatService.IsChatExistAsync(chatId);
            return Ok(result);
        }
    }
}
