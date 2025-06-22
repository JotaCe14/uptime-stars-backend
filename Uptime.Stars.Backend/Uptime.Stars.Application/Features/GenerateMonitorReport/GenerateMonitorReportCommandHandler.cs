using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Application.Features.GenerateReport;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Application.Features.GenerateMonitorReport;
internal sealed class GenerateMonitorReportCommandHandler(
    IDbContext dbContext,
    IReportService reportService) : ICommandHandler<GenerateMonitorReportCommand, Result<byte[]>>
{
    public async Task<Result<byte[]>> Handle(GenerateMonitorReportCommand request, CancellationToken cancellationToken)
    {
        var dateFrom = DateTime.ParseExact(request.DateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        var dateTo = DateTime.ParseExact(request.DateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        var events = await dbContext.Set<Event>()
            .Include(entity => entity.Monitor)
            .ThenInclude(entity => entity.Group)
            .Where(entity =>
                !entity.IsUp && !entity.FalsePositive &&
                entity.TimestampUtc >= DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc) && entity.TimestampUtc <= DateTime.SpecifyKind(dateTo, DateTimeKind.Utc))
            .OrderBy(entity => entity.TimestampUtc)
            .ToListAsync(cancellationToken);

        return reportService.GenerateMonitorReport(events, dateFrom, dateTo);
    }
}