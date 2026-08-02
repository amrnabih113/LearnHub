using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred");

            await HandleExceptionAsync(
                context,
                exception);
        }
    }



    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType =
            "application/problem+json";


        context.Response.StatusCode =
            (int)HttpStatusCode.InternalServerError;



        var problem =
            new ProblemDetails
            {
                Status = context.Response.StatusCode,

                Title =
                    "An unexpected error occurred",

                Detail =
                    "An internal server error occurred. Please try again later.",

                Instance =
                    context.Request.Path
            };



        problem.Extensions["traceId"] =
            context.TraceIdentifier;



        var json =
            JsonSerializer.Serialize(problem);



        await context.Response.WriteAsync(json);
    }
}