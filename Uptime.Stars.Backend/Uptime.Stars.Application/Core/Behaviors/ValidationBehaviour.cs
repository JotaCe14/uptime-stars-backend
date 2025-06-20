using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using ValidationException = Uptime.Stars.Application.Core.Exceptions.ValidationException;

namespace Uptime.Stars.Application.Core.Behaviors;

internal sealed class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    where TResponse : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IQuery<TResponse>)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        List<ValidationFailure> failures = validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
