using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;

namespace CursusJapaneseLearningPlatform.Service.Commons.Implementations;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new CustomException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Http context is null. Please Login.");
        }

        var user = httpContext.User;
        if (user == null || user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, ResponseMessages.UNAUTHORIZED);
        }

        var currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(currentUserId))
        {
            throw new CustomException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.UNAUTHORIZED, "User ID claim is not found");
        }
        return currentUserId;
    }
}
