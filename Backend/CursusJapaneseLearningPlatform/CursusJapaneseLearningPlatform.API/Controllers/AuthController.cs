using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Requests;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;
using CursusJapaneseLearningPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CursusJapaneseLearningPlatform.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _userService;
    private readonly IEmailService _mailService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService userService, IEmailService emailService, IConfiguration configuration)
    {
        _userService = userService;
        _mailService = emailService;
        _configuration = configuration;
    }

    /// <summary>
    /// Register
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRequestModel model)
    {
        var result = await _userService.CreateUserAsync(model);
        if (result == null)
        {
            return BadRequest(new BaseResponseModel<object>
                (StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST,
                ResponseMessages.CREATE_FAIL.Replace("{0}", "người dùng"), null));
        }

        return Ok(new BaseResponseModel<object>
            (StatusCodes.Status201Created, ResponseCodeConstants.SUCCESS,
            ResponseMessages.CREATE_SUCCESS.Replace("{0}", "người dùng"), result));
    }


    /// <summary>
    /// User login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _userService.LoginAsync(model);
        return Ok(new BaseResponseModel<object>
            (StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
            ResponseMessages.CREATE_SUCCESS.Replace("{0}", "người dùng"), result));
    }

    /// <summary>
    /// Login with Google
    /// </summary>
    [HttpPost("login/google")]
    public async Task<IActionResult> LoginWithGoogle([FromBody] FirebaseLoginModel model)
    {
        var result = await _userService.LoginWithGoogleAsync(model.AccessToken);
        return Ok(new BaseResponseModel<object>
            (StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
            ResponseMessages.CREATE_SUCCESS.Replace("{0}", "người dùng"), result));
    }

    /// <summary>
    /// Validate token
    /// </summary>
    [HttpGet("token/validate")]
    public async Task<IActionResult> ValidateToken(string token)
    {
        var isValid = await _userService.IsTokenValid(token);
        if (isValid)
        {
            return Ok(new BaseResponseModel<object>
                (StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
                ResponseMessages.CREATE_SUCCESS.Replace("{0}", "người dùng"), "Token hợp lệ"));
        }

        return Unauthorized(new BaseResponseModel<object>
            (StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED,
            ResponseMessages.INTERNAL_SERVER_ERROR, "Token không hợp lệ hoặc đã hết hạn."));
    }

    /// <summary>
    /// Verify email with token
    /// </summary>
    [HttpGet("email/verify/{token}")]
    public async Task<IActionResult> VerifyEmail(string token)
    {
        var result = await _userService.VerifyEmail(token);
        if (result != null)
        {
            return Ok(new BaseResponseModel<object>
                (StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
                "Xác thực email thành công", result));
        }

        return BadRequest(new BaseResponseModel<object>
            (StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST,
            "Xác thực email thất bại", result));
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshAccessToken token)
    {
        var result = await _userService.RefreshAccessToken(token.AccessToken, token.RefreshToken);
        if (result != null)
        {
            return Ok(new BaseResponseModel<object>
                (StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
                "Lấy token mới thành công", result));
        }

        return BadRequest(new BaseResponseModel<object>
            (StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST,
            "Lấy token mới thất bại", result));
    }

    /// <summary>
    /// Get User Detail
    /// </summary>
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUserDetail(Guid id) {
        var result = await _userService.GetUserDetail(id);
        if (result != null)
        {
            return Ok(new BaseResponseModel<object>
                (StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
                "Lấy thông tin người dùng thành công", result));
        }
        return BadRequest(new BaseResponseModel<object>
            (StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST,
            "Lấy thông tin người dùng thất bại", result));
    }

    /// <summary>
    /// Forgot password
    /// </summary>
    [HttpGet("forgot-password/{email}")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var result = await _userService.ForgotPassword(email);
        if (result)
        {
            return Ok(new BaseResponseModel<object>
                (StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
                "Gửi email thành công", result));
        }
        return BadRequest(new BaseResponseModel<object>
            (StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST,
            "Gửi email thất bại", result));
    }

    /// <summary>
    /// Rest Password
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
    {
        var result = await _userService.ResetPassword(model);
        if (result)
        {
            return Ok(new BaseResponseModel<object>
                (StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,
                "Đặt lại mật khẩu thành công", result));
        }
        return BadRequest(new BaseResponseModel<object>
            (StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST,
            "Đặt lại mật khẩu thất bại", result));
    }
}
