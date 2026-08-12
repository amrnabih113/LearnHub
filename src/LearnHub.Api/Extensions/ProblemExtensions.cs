using LearnHub.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Extensions;

public static class ProblemExtensions
{
    public static IActionResult ToProblem(this List<Error> errors)
    {
        if (errors is null || errors.Count == 0)
        {
            return new ObjectResult(new ProblemDetails
            {
                Title = "Error",
                Detail = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        if (errors.All(error => error.Type == ErrorKind.Validation))
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

        var problemDetails = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Status = statusCode
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    private static IActionResult ValidationProblem(List<Error> errors)
    {
        var errorsDictionary = errors
            .GroupBy(x => x.Code)
            .ToDictionary(
                x => x.Key,
                x => x.Select(e => e.Description).ToArray());

        var validationProblemDetails = new ValidationProblemDetails(errorsDictionary)
        {
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };

        return new ObjectResult(validationProblemDetails)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }
}