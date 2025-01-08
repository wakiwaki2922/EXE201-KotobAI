using CursusJapaneseLearningPlatform.Service.BusinessModels.SubcriptionModels;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursusJapaneseLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Creates a payment for the learner.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> CreatePayment([FromBody] SubscriptionRequestModel request, CancellationToken cancellationToken)
        {
            var idClaim = User.FindFirst("Id");
            if (idClaim == null || string.IsNullOrEmpty(idClaim.Value))
            {
                return Unauthorized("User is not authenticated or does not have an Id claim.");
            }

            if (!Guid.TryParse(idClaim.Value, out var userId))
            {
                return BadRequest("The Id claim is not a valid GUID.");
            }

            var response = await _paymentService.CreatePayment(request, userId, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Completes a payment based on the provided payment details.
        /// </summary>
        [HttpGet("complete")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> CompletePayment([FromQuery] string paymentId, [FromQuery] string payerId, [FromQuery] string token, CancellationToken cancellationToken)
        {
            var response = await _paymentService.CompletePayment(paymentId, payerId, token, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves payment history for a specific user.
        /// </summary>
        [HttpGet("history/{userId}")]
        [Authorize(Roles = "Learner,Admin")]
        public async Task<IActionResult> GetUserPaymentHistory([FromRoute] Guid userId) 
        {
            var response = await _paymentService.GetPaymentHistoryByUserIdAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves all payment history records.
        /// </summary>
        [HttpGet("history/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPaymentHistory()
        {
            var response = await _paymentService.GetAllPaymentHistoryAsync();
            return StatusCode(response.StatusCode, response);
        }
    }


}
