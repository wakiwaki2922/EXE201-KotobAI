using CursusJapaneseLearningPlatform.Service.BusinessModels.CollectionModels;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursusJapaneseLearningPlatform.API.Controllers
{
    /// <summary>
    /// Manages operations related to collections.
    /// </summary>
    [ApiController]
    [Route("api/collections")]
    [Authorize]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionService _collectionService;

        /// <summary>
        /// Constructor for CollectionController.
        /// </summary>
        /// <param name="collectionService">Service for handling collection operations.</param>
        public CollectionController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> CreateCollection([FromBody] CollectionRequestModel request) 
        {
            var response = await _collectionService.CreateCollection(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Updates an existing collection by ID.
        /// </summary>
        /// <param name="id">ID of the collection to update.</param>
        /// <param name="request">Updated collection details.</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> UpdateCollection([FromRoute] Guid id, [FromBody] CollectionRequestModel request) 
        {
            var response = await _collectionService.UpdateCollection(id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves a specific collection by ID.
        /// </summary>
        /// <param name="id">ID of the collection to retrieve.</param>
        [HttpGet("{id}")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> GetCollection([FromRoute] Guid id) 
        {
            var response = await _collectionService.GetCollectionById(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves collections by user ID.
        /// </summary>
        /// <param name="userId">ID of the user to retrieve collections for.</param>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> GetCollectionsByUser([FromRoute] Guid userId)
        {
            var response = await _collectionService.GetCollectionsByUserId(userId);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Searches for collections by name.
        /// </summary>
        /// <param name="collectionName">Name of the collection to search for.</param>
        [HttpGet("search")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> SearchCollections([FromQuery] string collectionName) 
        {
            var response = await _collectionService.GetCollectionsByName(collectionName);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves all active collections.
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> GetActiveCollections()
        {
            var response = await _collectionService.GetActiveSavedCollections();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Deletes a collection by ID.
        /// </summary>
        /// <param name="id">ID of the collection to delete.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Learner")]
        public async Task<IActionResult> DeleteCollection([FromRoute] Guid id) 
        {
            var response = await _collectionService.DeleteCollection(id);
            return StatusCode(response.StatusCode, response);
        }
    }


}
