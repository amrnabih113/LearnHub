using LearnHub.Api.Extensions;
using LearnHub.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace LearnHub.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return Ok(result.Value);
    }

    protected IActionResult HandleCreatedResult(Result<Guid> result, string actionName, object? routeValues = null)
    {
        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return CreatedAtAction(
            actionName,
            routeValues,
            new { id = result.Value });
    }

    protected IActionResult HandleCreatedResult(Result<Guid> result)
    {
        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return StatusCode(StatusCodes.Status201Created, new { id = result.Value });
    }

    protected IActionResult HandleResult(Result<Created> result)
    {
        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return StatusCode(StatusCodes.Status201Created, new { message = "Operation completed successfully." });
    }

    protected IActionResult HandleResult(Result<Updated> result)
    {
        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return NoContent();
    }

    protected IActionResult HandleResult(Result<Deleted> result)
    {
        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return NoContent();
    }
}