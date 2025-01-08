using Microsoft.AspNetCore.Mvc;
using CursusJapaneseLearningPlatform.Service.BusinessModels.RoleModels.Requests;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CursusJapaneseLearningPlatform.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Get Role By Id
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRoleById(Guid id)
    {
        var result = await _roleService.GetRoleById(id, new CancellationToken());
        return Ok(new BaseResponseModel<object>
                (statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: $"{ResponseMessages.GET_SUCCESS.Replace("{0}", "người dùng")}",
                data: result));
    }

    /// <summary>
    /// Create new Role
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRole([FromBody] RoleRequestModel model)
    {
        var result = await _roleService.CreateRole(model, new CancellationToken());
        return Ok(new BaseResponseModel<bool>
                (statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: $"{ResponseMessages.CREATE_SUCCESS.Replace("{0}", "người dùng")}",
                data: result));
    }

    /// <summary>
    /// Get All Roles
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllRoles()
    {
        var result = await _roleService.GetRolesAsync(new CancellationToken());
        return Ok(new BaseResponseModel<object>
                (statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: $"{ResponseMessages.GET_SUCCESS.Replace("{0}", "quyền")}",
                data: result));
    }
}
