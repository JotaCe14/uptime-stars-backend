using Microsoft.AspNetCore.Mvc;
using Uptime.Stars.Domain.Core.Primitives;

namespace Uptime.Stars.Api.Extensions;

public static class ProblemDetailsExtension
{
    public static ProblemDetails ToProblemDetails(this Error error)
    {
        return new ProblemDetails
        {
            Title = "An application error ocurred",
            Detail = error.Description,
            Type = error.Type.ToString(),
            Status = error.Type switch
            {
                ErrorType.Failure => StatusCodes.Status400BadRequest,
                ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
                ErrorType.Problem => StatusCodes.Status424FailedDependency,
                ErrorType.Idempotency => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            },
            Instance = error.Code
        };
    }
}
