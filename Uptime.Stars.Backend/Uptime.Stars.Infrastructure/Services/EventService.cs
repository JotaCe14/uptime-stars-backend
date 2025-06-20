using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Infrastructure.Services;
internal sealed class EventService(IDateTime dateTime, IEventRepository eventRepository) : IEventService
{
    public async Task<decimal> GetUptimePercentageLastSince(Guid monitorId, TimeSpan timeSpan, CancellationToken cancellationToken = default)
    {
        var sinceDateTime = dateTime.UtcNow - timeSpan;

        var events = await eventRepository.GetLastByIdSinceAsync(monitorId, sinceDateTime, cancellationToken);

        if (events.Count == 0)
        {
            return 100m;
        }

        var totalEvents = events.Count;

        var uptimeEvents = events.Count(@event => @event.IsUp);

        var percentage = decimal.Round((decimal)uptimeEvents / totalEvents * 100, 2);

        return percentage;
    }
}
