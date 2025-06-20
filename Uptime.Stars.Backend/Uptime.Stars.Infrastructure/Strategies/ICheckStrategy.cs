using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Infrastructure.Strategies;
internal interface ICheckStrategy
{
    Task<CheckResult> CheckAsync(ComponentMonitor monitor, CancellationToken cancellationToken = default);
}

public record CheckResult(bool IsUp, string? Message = null, long? LatencyMilliseconds = null)
{
    public static CheckResult Up(long latencyMilliseconds) => new(true, null, latencyMilliseconds);
    public static CheckResult Down(string message) => new(false, message);
}