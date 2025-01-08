using CursusJapaneseLearningPlatform.Service.BusinessModels.VocabularyModels;
using CursusJapaneseLearningPlatform.Service.Implementations;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursusJapaneseLearningPlatform.API.Controllers
{
    /// <summary>
    /// Manages operations related to vocabulary entries.
    /// </summary>
    [ApiController]
    [Route("api/vocabularies")]
    [Authorize]
    public class VocabularyController : ControllerBase
    {
        private readonly IVocabularyService _vocabularyService;

        /// <summary>
        /// Constructor for VocabularyController.
        /// </summary>
        /// <param name="vocabularyService">Service for handling vocabulary operations.</param>
        public VocabularyController(IVocabularyService vocabularyService)
        {
            _vocabularyService = vocabularyService;
        }

        /// <summary>
        /// Creates a new vocabulary entry.
        /// </summary>
        /// <param name="request">Vocabulary details.</param>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateVocabulary([FromBody] VocabularyRequestModel request)
        {
            var response = await _vocabularyService.CreateVocabulary(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Updates an existing vocabulary entry by ID.
        /// </summary>
        /// <param name="id">ID of the vocabulary entry to update.</param>
        /// <param name="request">Updated vocabulary details.</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateVocabulary([FromRoute] Guid id, [FromBody] VocabularyRequestModel request)
        {
            var response = await _vocabularyService.UpdateVocabulary(id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves a specific vocabulary entry by ID.
        /// </summary>
        /// <param name="id">ID of the vocabulary entry to retrieve.</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVocabularyById([FromRoute] Guid id)
        {
            var response = await _vocabularyService.GetVocabularyById(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Searches for vocabulary entries by word.
        /// </summary>
        /// <param name="word">Word to search for in vocabulary entries.</param>
        [HttpGet("search")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> GetVocabulariesByWord([FromQuery] string word)
        {
            var response = await _vocabularyService.GetVocabulariesByWord(word);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves all active vocabulary entries.
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveVocabularies()
        {
            var response = await _vocabularyService.GetActiveVocabularies();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Deletes a vocabulary entry by ID.
        /// </summary>
        /// <param name="id">ID of the vocabulary entry to delete.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVocabulary([FromRoute] Guid id)
        {
            var response = await _vocabularyService.DeleteVocabulary(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Searches for vocabulary entries by word using api.
        /// </summary>
        /// <param name="word">Word to search for in vocabulary entries.</param>
        [HttpGet("search/api")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchApiWord([FromQuery] string word)
        {
            var response = await _vocabularyService.SearchApiWord(word);
            return StatusCode(response.StatusCode, response);
        }
    }
}
