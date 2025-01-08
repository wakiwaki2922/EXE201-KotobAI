using CursusJapaneseLearningPlatform.Service.BusinessModels.FlashcardModels;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursusJapaneseLearningPlatform.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FlashcardsController : ControllerBase
    {
        private readonly IFlashcardService _flashcardService;

        public FlashcardsController(IFlashcardService flashcardService)
        {
            _flashcardService = flashcardService;
        }

        /// <summary>
        /// Creates a new flashcard.
        /// </summary>
        /// <param name="dto">The data for the flashcard to be created.</param>
        [HttpPost]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> Create([FromBody] CreateFlashcardDto dto)
        {
            var response = await _flashcardService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Deletes a flashcard by ID.
        /// </summary>
        /// <param name="id">The ID of the flashcard to delete.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> Delete([FromRoute] Guid id) // Use [FromRoute] since id comes from the URL
        {
            var response = await _flashcardService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves a flashcard by ID.
        /// </summary>
        /// <param name="id">The ID of the flashcard to retrieve.</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> GetById([FromRoute] Guid id) // Use [FromRoute] since id comes from the URL
        {
            var response = await _flashcardService.GetByIdAsync(id);
            return response.StatusCode == StatusCodes.Status200OK
                ? Ok(response)
                : StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves all flashcards for a specific collection.
        /// </summary>
        /// <param name="collectionId">The ID of the collection to get flashcards for.</param>
        [HttpGet("collection/{collectionId}")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> GetAllByCollection([FromRoute] Guid collectionId) // Use [FromRoute] since collectionId comes from the URL
        {
            var response = await _flashcardService.GetAllByCollectionIdAsync(collectionId);
            return response.StatusCode == StatusCodes.Status200OK
                ? Ok(response)
                : StatusCode(response.StatusCode, response);
        }
    }

}
