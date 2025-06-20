using FluentValidation.Results;
using Uptime.Stars.Domain.Core.Primitives;

namespace Uptime.Stars.Application.Core.Exceptions;

public sealed class ValidationException(IEnumerable<ValidationFailure> failures) : Exception("One or more validation failures has occurred.")
{
    public IReadOnlyCollection<Error> Errors { get; } = failures
            .Distinct()
            .Select(failure => Error.Validation(failure.ErrorCode, failure.ErrorMessage))
            .ToList();
}