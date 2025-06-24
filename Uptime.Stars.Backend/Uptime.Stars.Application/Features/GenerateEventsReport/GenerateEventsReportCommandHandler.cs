using Microsoft.EntityFrameworkCore;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Application.Features.GenerateEventsReport;
internal sealed class GenerateEventsReportCommandHandler(IDbContext dbContext, IReportService reportService) : ICommandHandler<GenerateEventsReportCommand, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GenerateEventsReportCommand request, CancellationToken cancellationToken)
    {
        var events = await dbContext.Set<Event>()
            .Where(@event => @event.MonitorId == request.MonitorId)
            .Include(@event => @event.Monitor)
            .ThenInclude(entity => entity.Group)
            .OrderByDescending(@event => @event.TimestampUtc)
            .ToListAsync(cancellationToken);

        if (events.Count == 0)
        {
            return Result.Failure<byte[]>(Error.Failure("GenerateEventsReport.Handle", "No events found for the specified monitor."));
        }

        return reportService.GenerateEventsReport(events);
    }
}