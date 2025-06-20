namespace Uptime.Stars.Domain.Core.Primitives;

public enum ErrorType
{
    Failure = 0,
    Problem = 1,
    Validation = 2,
    Idempotency = 3
}
