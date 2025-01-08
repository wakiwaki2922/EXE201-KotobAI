using CursusJapaneseLearningPlatform.Service.BusinessModels.MessageModels.Requests;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursusJapaneseLearningPlatform.API.Controllers
{
    /// <summary>
    /// Manages message operations.
    /// </summary>
    [Route("api/messages")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        /// <summary>
        /// Constructor for MessageController.
        /// </summary>
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        /// <summary>
        /// Gets all messages by chat ID.
        /// </summary>
        [HttpGet("chat/{chatId:guid}")]
        public async Task<IActionResult> GetAllMessagesByChatId([FromRoute] Guid chatId)
        {
            var result = await _messageService.GetAllMessagesByChatIdAsync(chatId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new message.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] MessageRequestModel requestModel)
        {
            var result = await _messageService.CreateMessageAsync(requestModel);
            return CreatedAtAction(nameof(GetAllMessagesByChatId), new { chatId = requestModel.ChatId }, result);
        }

        /// <summary>
        /// Deletes a message by ID and user name.
        /// </summary>
        [HttpDelete("{messageId:guid}/user/{userName}")]
        public async Task<IActionResult> DeleteMessage([FromRoute] Guid messageId, [FromRoute] string userName)
        {
            var result = await _messageService.DeleteMessageAsync(messageId, userName);
            return Ok(result);
        }
    }
}
