using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Contracts.Monitor;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;
using X.PagedList;
using X.PagedList.EF;

namespace Uptime.Stars.Application.Features.GetMonitors;
internal sealed class GetMonitorsQueryHandler(
    IDbContext dbContext,
    IEventRepository eventRepository,
    IEventService eventService) : IQueryHandler<GetMonitorsQuery, IPagedList<MonitorResponse>>
{
    public async Task<IPagedList<MonitorResponse>> Handle(GetMonitorsQuery request, CancellationToken cancellationToken)
    {
        var monitors = await dbContext.Set<ComponentMonitor>()
            .Select(monitor => new MonitorResponse {
                    Id = monitor.Id,
                    Name = monitor.Name,
                    Description = monitor.Description ?? "",
                    Target = monitor.Target,
                    CreatedAtUtc = monitor.CreatedAt.ToString(DateTimeFormats.DefaultFormat),
                    IsActive = monitor.IsActive,
                    LastEvents = null,
                    Uptime24hPercentage = "",
                    Uptime30dPercentage = ""
            })
            .ToPagedListAsync(request.PageNumber, request.PageSize, default, cancellationToken);

        foreach (var monitor in monitors)
        {
            var lastEvents = await eventRepository.GetLastByIdAsync(monitor.Id, 20, cancellationToken);

            var uptime24h = await eventService.GetUptimePercentageLastSince(
                monitor.Id,
            TimeSpan.FromHours(24),
                cancellationToken);

            var uptime30d = await eventService.GetUptimePercentageLastSince(
                monitor.Id,
                TimeSpan.FromDays(30),
                cancellationToken);

            monitor.LastEvents = lastEvents.Select(@event => new EventResponse(
                        @event.TimestampUtc.ToString(DateTimeFormats.DefaultFormat),
                        @event.IsUp,
                        @event.Message ?? "",
                        @event.LatencyMilliseconds ?? 0,
                        @event.FalsePositive,
                        @event.Category ?? "",
                        @event.Note ?? "",
                        @event.TicketId ?? "",
                        @event.MaintenanceType ?? "")).ToList();

            monitor.Uptime24hPercentage = uptime24h.ToString("0.##") + "%";

            monitor.Uptime30dPercentage = uptime30d.ToString("0.##") + "%";
        }

        return monitors;
    }
}