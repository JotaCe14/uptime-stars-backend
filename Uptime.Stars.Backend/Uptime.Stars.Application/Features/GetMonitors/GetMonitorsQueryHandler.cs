using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Contracts.Events;
using Uptime.Stars.Contracts.Monitors;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
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
                    GroupId = monitor.GroupId,
                    IsUp = monitor.IsUp,
                    IntervalInMinutes = monitor.IntervalInMinutes,
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
            var lastImportantEvents = await eventRepository.GetLastImportantByMonitorIdAsync(monitor.Id, request.LastEventsLimit, cancellationToken);

            var uptime24h = await eventService.GetUptimePercentageLastSince(
                monitor.Id,
            TimeSpan.FromHours(24),
                cancellationToken);

            var uptime30d = await eventService.GetUptimePercentageLastSince(
                monitor.Id,
                TimeSpan.FromDays(30),
                cancellationToken);

            monitor.LastImportantEvents = lastImportantEvents.Select(@event => new EventResponse(
                        @event.Id,
                        @event.TimestampUtc.ToString(DateTimeFormats.DefaultFormat),
                        @event.IsUp,
                        @event.IsImportant,
                        @event.Message ?? "",
                        @event.LatencyMilliseconds ?? 0,
                        @event.FalsePositive,
                        @event.Category is null ? "" : Enum.GetName(typeof(Category), @event.Category) ?? "",
                        @event.Note ?? "",
                        @event.TicketId ?? "",
                        @event.MaintenanceType is null ? "" : Enum.GetName(typeof(MaintenanceType), @event.MaintenanceType) ?? "",
                        monitor.Name)).ToList();

            monitor.Uptime24hPercentage = uptime24h.HasValue ? uptime24h.Value.ToString("0.##") + "%" : "";

            monitor.Uptime30dPercentage = uptime30d.HasValue ? uptime30d.Value.ToString("0.##") + "%": "";
        }

        return monitors;
    }
}