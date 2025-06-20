namespace Uptime.Stars.Application.Services;
public interface IEventService
{
    Task<decimal?> GetUptimePercentageLastSince(Guid monitorId, TimeSpan timeSpan, CancellationToken cancellationToken = default);
}