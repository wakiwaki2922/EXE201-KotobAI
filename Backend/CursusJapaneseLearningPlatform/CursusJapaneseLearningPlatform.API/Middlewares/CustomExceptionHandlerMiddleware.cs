using System.Text.Json;
using CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;

namespace CursusJapaneseLearningPlatform.API.Middlewares;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

    public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomException ex)
        {
            _logger.LogError(ex, "Error occurred: {Message} at {StackTrace}", ex.ErrorMessage, ex.StackTrace);
            //_logger.LogError(ex, ex.ErrorMessage?.ToString() ?? "Unknown error occurred.");
            await HandleExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "An unexpected error occurred.");
            _logger.LogError(ex, "An unexpected error occurred at {StackTrace}", ex.StackTrace);
            await HandleExceptionAsync(context, new CustomException(context.Response.StatusCode, ResponseMessages.INTERNAL_SERVER_ERROR, StatusCodes.Status500InternalServerError));
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, CustomException ex)
    {
        var response = new BaseResponseModel<object>(ex.StatusCode, ex.ErrorCode, null, null, ex.ErrorMessage?.ToString());
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex.StatusCode;

        var result = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(result);
    }
}
