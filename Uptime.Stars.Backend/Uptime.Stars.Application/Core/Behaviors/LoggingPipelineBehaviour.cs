using MediatR;
using Microsoft.Extensions.Logging;
using SerilogTimings;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Core.Behaviors;
public sealed class LoggingPipelineBehaviour<TRequest, TResponse>(
    ILogger<LoggingPipelineBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        using var operation = Operation.Begin("Completed request {@RequestName}", typeof(TRequest).Name);

        logger.LogInformation("Starting request {@RequestName}", typeof(TRequest).Name);

        logger.LogDebug("Request: {@Request}", request);

        var result = await next();

        if (result.IsFailure)
        {
            logger.LogInformation("Failed request {@RequestName}: {@ErrorType} {@Error}", typeof(TRequest).Name, result.Error.Type, result.Error);
        }
        else
        {
            operation.Complete();
        }

        return result;
    }
}