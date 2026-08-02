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

    protected IActionResult HandleResult(Result<Updated> result)
    {
        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return NoContent();
    }

    protected IActionResult HandleResult(Result<Created> result)
    {
        if (result.IsSuccess)
        {
            return Created();
        }

        return result.Errors.ToProblem();
    }

    protected IActionResult CreatedResult<T>(Result<T> result, string actionName)
    {
        if (result.IsError)
        {
            return result.Errors.ToProblem();
        }

        return CreatedAtAction(
            actionName,
            result.Value);
    }


    protected IActionResult HandleResult(Result<Deleted> result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.Errors.ToProblem();
    }

}