using CursusJapaneseLearningPlatform.Service.BusinessModels.PackageModels;
using CursusJapaneseLearningPlatform.Service.Implementations;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursusJapaneseLearningPlatform.API.Controllers
{
    /// <summary>
    /// Manages operations related to packages.
    /// </summary>
    [ApiController]
    [Route("api/packages")]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;

        /// <summary>
        /// Constructor for PackageController.
        /// </summary>
        /// <param name="packageService">Service for handling package operations.</param>
        public PackageController(IPackageService packageService)
        {
            _packageService = packageService;
        }

        /// <summary>
        /// Creates a new package.
        /// </summary>
        /// <param name="request">Package details.</param>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePackage([FromBody] PackageRequestModel request)
        {
            var response = await _packageService.CreatePackage(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves all active packages.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetActivePackages()
        {
            var response = await _packageService.GetActivePackages();
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Updates an existing package by ID.
        /// </summary>
        /// <param name="id">ID of the package to update.</param>
        /// <param name="request">Updated package details.</param>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePackage([FromRoute] Guid id, [FromBody] PackageRequestModel request)
        {
            var response = await _packageService.UpdatePackage(id, request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Deletes a package by ID.
        /// </summary>
        /// <param name="id">ID of the package to delete.</param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePackage([FromRoute] Guid id)
        {
            var response = await _packageService.DeletePackage(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Gets a package by ID.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPackageById([FromRoute] Guid id)
        {
            var response = await _packageService.GetPackageById(id);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Get all packages.
        /// </summary>
        [HttpGet("getAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPackages()
        {
            var response = await _packageService.GetAllPackages();
            return StatusCode(response.StatusCode, response);
        }
    }
}
