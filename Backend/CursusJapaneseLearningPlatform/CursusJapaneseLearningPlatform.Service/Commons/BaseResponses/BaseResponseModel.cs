using Microsoft.AspNetCore.Http;
using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using CursusJapaneseLearningPlatform.Service.BusinessModels.UserModels.Responses;

namespace CursusJapaneseLearningPlatform.Service.Commons.BaseResponses;

public class BaseResponseModel<T>
{
    public T? Data { get; set; }
    public object? AdditionalData { get; set; }
    public string? Message { get; set; }
    public int StatusCode { get; set; }
    public string Code { get; set; }

    public BaseResponseModel(int statusCode, string code, T? data = default, object? additionalData = null, string? message = null)
    {
        StatusCode = statusCode;
        Code = code;
        Data = data;
        AdditionalData = additionalData;
        Message = message;
    }

    public BaseResponseModel(int statusCode, string code, string message, T data)
    {
        StatusCode = statusCode;
        Code = code;
        Message = message;
        Data = data;
    }

    public static BaseResponseModel<T> OkResponseModel(T data, object? additionalData = null, string code = ResponseCodeConstants.SUCCESS)
    {
        return new BaseResponseModel<T>(StatusCodes.Status200OK, code, data, additionalData);
    }

    public static BaseResponseModel<T> ErrorResponseModel(int statusCode, string code, string message, object? additionalData = null)
    {
        return new BaseResponseModel<T>(statusCode, code, default, additionalData, message);
    }
}
