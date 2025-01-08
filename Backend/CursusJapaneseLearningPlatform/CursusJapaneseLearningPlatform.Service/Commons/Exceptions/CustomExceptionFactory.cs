using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
public static class CustomExceptionFactory
{
    public static CustomException CreateInternalServerError()
    {
        return new CustomException(StatusCodes.Status500InternalServerError,
                                   ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                                   ResponseMessages.INTERNAL_SERVER_ERROR);
    }

    public static CustomException CreateNotFoundError(string objectName)
    {
        return new CustomException(StatusCodes.Status404NotFound,
                                   ResponseCodeConstants.NOT_FOUND, 
                                   ResponseMessages.NOT_FOUND.Replace("{0}", $"{objectName}"));
    }

    public static CustomException CreateBadRequest(string message)
    {
        return new CustomException(StatusCodes.Status400BadRequest,
                                   ResponseCodeConstants.BADREQUEST,
                                   message);
    }
}