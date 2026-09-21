using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

/// <summary>
/// Translates <see cref="ServiceResult"/> outcomes into HTTP responses so the
/// individual controllers do not repeat the mapping.
/// Success -> 200/201, NotFound -> 404, Conflict -> 409, Error -> 500.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Maps a value-returning result, using 200 OK on success.</summary>
    protected IActionResult FromResult<T>(ServiceResult<T> result)
        => result.IsSuccess ? Ok(result.Value) : Failure(result);

    /// <summary>Maps a void result, using 204 No Content on success.</summary>
    protected IActionResult FromResult(ServiceResult result)
        => result.IsSuccess ? NoContent() : Failure(result);

    /// <summary>
    /// Maps a result from a create operation, returning 201 Created with a
    /// Location header pointing at <paramref name="actionName"/>.
    /// </summary>
    protected IActionResult CreatedFromResult<T>(ServiceResult<T> result, string actionName, Func<T, object> routeValues)
        => result.IsSuccess
            ? CreatedAtAction(actionName, routeValues(result.Value!), result.Value)
            : Failure(result);

    private IActionResult Failure(ServiceResult result)
    {
        var message = result.ErrorMessage ?? "The request could not be completed.";

        return result.Status switch
        {
            ResultStatus.NotFound => NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = message
            }),
            ResultStatus.Conflict => Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = message
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Request failed",
                Detail = message
            })
        };
    }
}
