﻿using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Contracts.Monitor;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.GetMonitor;
internal sealed class GetMonitorQueryHandler(
    IMonitorRepository monitorRepository, 
    IEventRepository eventRepository,
    IEventService eventService) : IQueryHandler<GetMonitorQuery, Result<MonitorResponse>>
{
    public async Task<Result<MonitorResponse>> Handle(GetMonitorQuery request, CancellationToken cancellationToken)
    {
        var monitor = await monitorRepository.GetByIdAsync(request.MonitorId, cancellationToken);
        
        if (monitor is null)
        {
            return Result.Failure<MonitorResponse>(Error.Failure("GetMonitor.Handle", "Monitor not found"));
        }

        var lastEvents = await eventRepository.GetLastByIdAsync(monitor.Id, request.LastEventsLimit, cancellationToken);

        var uptime24h = await eventService.GetUptimePercentageLastSince(
            monitor.Id, 
            TimeSpan.FromHours(24), 
            cancellationToken);

        var uptime30d = await eventService.GetUptimePercentageLastSince(
            monitor.Id,
            TimeSpan.FromDays(30),
            cancellationToken);

        return new MonitorResponse
        {
            Id = monitor.Id,
            Name = monitor.Name,
            Description = monitor.Description ?? "",
            Target = monitor.Target,
            CreatedAtUtc = monitor.CreatedAt.ToString(DateTimeFormats.DefaultFormat),
            IsActive = monitor.IsActive,
            LastEvents = lastEvents.Select(@event => new EventResponse(
                @event.TimestampUtc.ToString(DateTimeFormats.DefaultFormat),
                @event.IsUp,
                @event.Message ?? "",
                @event.LatencyMilliseconds ?? 0,
                @event.FalsePositive,
                @event.Category ?? "",
                @event.Note ?? "",
                @event.TicketId ?? "",
                @event.MaintenanceType ?? "")).ToList(),
            Uptime24hPercentage = uptime24h.HasValue ? uptime24h.Value.ToString("0.##") + "%" : "",
            Uptime30dPercentage = uptime30d.HasValue ? uptime30d.Value.ToString("0.##") + "%" : ""
        };
    }
}