using Application.Results.Interfaces;
using Shared.Enums;

namespace API.Extensions;

public static class ServiceResultExtensions
{
    public static IResult ToApiResult(this IServiceResult result)
    {
        return result.Type switch
        {
           ServiceResultType.Ok => Results.Ok(result.Data),
            ServiceResultType.Created => Results.Created(string.Empty, result.Data),
            ServiceResultType.NoContent => Results.NoContent(),
            ServiceResultType.BadRequest => Results.BadRequest(result.ErrorMessage),
            ServiceResultType.Unauthorized => Results.Unauthorized(),
            ServiceResultType.Forbidden => Results.Forbid(),
            ServiceResultType.NotFound => Results.NotFound(),
            ServiceResultType.Conflict => Results.Conflict(result.ErrorMessage),
            ServiceResultType.ValidationFailed => Results.BadRequest(result.ValidationErrors),
            ServiceResultType.CriticalError => Results.StatusCode(StatusCodes.Status500InternalServerError),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError),
        };
    }
}