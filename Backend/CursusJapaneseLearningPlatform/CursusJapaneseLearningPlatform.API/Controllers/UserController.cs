using Microsoft.AspNetCore.Mvc;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests;
using Azure;
using CursusJapaneseLearningPlatform.Service.Implementations;
using Microsoft.AspNetCore.Authorization;

namespace CursusJapaneseLearningPlatform.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get user by id
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Learner")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return Ok(new BaseResponseModel<UserResponseModel>
                (statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: $"{ResponseMessages.GET_SUCCESS.Replace("{0}", "người dùng")}",
                data: result));
    }

    /// <summary>
    /// Tải lên hình ảnh cho Avatar theo ID.
    /// </summary>
    /// <param name="id">ID của User.</param>
    /// <param name="imageUploadl">File hình ảnh cần tải lên.</param>
    /// <returns>Trạng thái tải lên thành công.</returns>
    [HttpPut("upload-image/{id}")]
    [Authorize(Roles = "Admin,Learner")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponseModel<object>))]
    public async Task<IActionResult> UploadAvatar(Guid id, IFormFile imageUploadl)
    {
        string response = await _userService.UploadAvatar(imageUploadl, id);
        return Ok(new BaseResponseModel<object>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            message: $"{ResponseMessages.UPLOAD_SUCCESS.Replace("{0}", "Image")}",
            data: response));
    }

    /// <summary>
    /// Lấy user với link hình ảnh by id
    /// </summary>
    [HttpGet("get-user-with-avatar/{id}")]
    [Authorize(Roles = "Admin,Learner")]
    public async Task<IActionResult> GetUserByIdWithAvater(Guid id)
    {
        var result = await _userService.GetUserByIdWithAvaterAsync(id);
        return Ok(new BaseResponseModel<UserResponseModel>
                (statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: $"{ResponseMessages.GET_SUCCESS.Replace("{0}", "người dùng")}",
                data: result));
    }

    /// <summary>
    /// Update user by id
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Learner")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateRequestModel model)
    {
        var result = await _userService.UpdateUserAsync(id, model);
        return Ok(new BaseResponseModel<UserResponseModel>
                (statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: $"{ResponseMessages.UPDATE_SUCCESS.Replace("{0}", "người dùng")}",
                data: result));
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var response = await _userService.GetAllUsersAsync();
        return StatusCode(response.StatusCode, response);
    }
    [HttpPut("admin/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserAdmin([FromRoute] Guid id, [FromQuery] bool isDelete, [FromQuery] bool isActive)
    {
        var response = await _userService.UpdateUserAdmin(id, isDelete, isActive);
        return StatusCode(response.StatusCode, response);
    }
}
