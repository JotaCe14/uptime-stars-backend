using Microsoft.EntityFrameworkCore;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Contracts.Events;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Enums;
using X.PagedList;
using X.PagedList.EF;

namespace Uptime.Stars.Application.Features.GetEvents;
internal sealed class GetImportantEventsQueryHandler(IDbContext dbContext) : IQueryHandler<GetImportantEventsQuery, IPagedList<EventResponse>>
{
    public async Task<IPagedList<EventResponse>> Handle(GetImportantEventsQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Set<Event>()
            .Where(entity => entity.IsImportant && (request.MonitorId == null || entity.MonitorId == request.MonitorId))
            .Include(entity => entity.Monitor)
            .OrderByDescending(entity => entity.TimestampUtc)
            .Select(@event => new EventResponse(
                        @event.Id,
                        @event.TimestampUtc.ToString(DateTimeFormats.DefaultFormat),
                        @event.IsUp,
                        @event.IsImportant,
                        @event.Message ?? "",
                        @event.LatencyMilliseconds ?? 0,
                        @event.FalsePositive,
                        @event.Category == null ? "" : @event.Category == Category.Internal ? "Internal" : "External",
                        @event.Note ?? "",
                        @event.TicketId ?? "",
                        @event.MaintenanceType == null ? "" : @event.MaintenanceType == MaintenanceType.Emergency ? "Emergency" : "Planned",
                        @event.Monitor.Name))
            .ToPagedListAsync(request.PageNumber, request.PageSize, default, cancellationToken);
    }
}