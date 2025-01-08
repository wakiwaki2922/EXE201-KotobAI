using CursusJapaneseLearningPlatform.Service.BusinessModels.SubcriptionModels;
using CursusJapaneseLearningPlatform.Service.Implementations;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CursusJapaneseLearningPlatform.API.Controllers
{
    /// <summary>
    /// Manages operations related to subscriptions.
    /// </summary>
    [ApiController]
    [Route("api/subscriptions")]
    [Authorize(Roles = "Admin,Learner")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// Get all subscription history (Admin only).
        /// </summary>
        [HttpGet("history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSubscriptionHistory()
        {
            var response = await _subscriptionService.GetAllSubscriptionHistoryAsync();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get subscription history by user ID.
        /// </summary>
        [HttpGet("history/user/{userId}")]
        [Authorize(Roles = "Learner,Admin")]
        public async Task<IActionResult> GetSubscriptionHistoryByUserId(Guid userId)
        {
            var response = await _subscriptionService.GetSubscriptionHistoryByUserIdAsync(userId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
