using LearnHub.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Extensions;

public static class ProblemExtensions
{
    public static IActionResult ToProblem(
        this List<Error> errors)
    {
        if (errors.Count == 0)
        {
            return new ObjectResult(
                new ProblemDetails())
            {
                StatusCode = 500
            
            };
        }


        if (errors.All(
            error => error.Type == ErrorKind.Validation))
        {
            return ValidationProblem(errors);
        }


        return Problem(errors[0]);
    }


    private static IActionResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            ErrorKind.Validation => StatusCodes.Status400BadRequest,
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return new ObjectResult(new
        {
            title = error.Code,
            detail = error.Description,
            status = statusCode
        })
        {
            StatusCode = statusCode
        };
    }


    private static IActionResult ValidationProblem(
        List<Error> errors)
    {
        var errorsDictionary =
            errors
            .GroupBy(x => x.Code)
            .ToDictionary(
                x => x.Key,
                x => x.Select(e => e.Description).ToArray());


        return new ObjectResult(
            new ValidationProblemDetails(errorsDictionary)
            {
                Status =
                    StatusCodes.Status400BadRequest
                 
            })
        {
            StatusCode =
                StatusCodes.Status400BadRequest
        };
    }
}