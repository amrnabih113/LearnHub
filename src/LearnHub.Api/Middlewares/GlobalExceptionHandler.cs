using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Infrastructure;

public class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    IHostEnvironment env) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var detail = env.IsDevelopment()
            ? exception.Message
            : "An unexpected error occurred. Please try again later.";

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = env.IsDevelopment() ? exception.GetType().Name : "ServerError",
                Title = "Application error",
                Detail = detail,
                Status = StatusCodes.Status500InternalServerError
            }
        });
    }
}