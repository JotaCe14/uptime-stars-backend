using ClosedXML.Excel;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Infrastructure.Services;
internal sealed class ReportService : IReportService
{
    public byte[] GenerateReport(List<Event> events)
    {
        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add("Events");

        worksheet.Cell(1, 1).Value = "TimestampUtc";
        worksheet.Cell(1, 2).Value = "Status";
        worksheet.Cell(1, 3).Value = "Latency (ms)";
        worksheet.Cell(1, 4).Value = "Message";
        worksheet.Cell(1, 5).Value = "FalsePositive";
        worksheet.Cell(1, 6).Value = "Category";
        worksheet.Cell(1, 7).Value = "Note";
        worksheet.Cell(1, 8).Value = "TicketId";
        worksheet.Cell(1, 9).Value = "MaintenanceType";


        for (int i = 0; i < events.Count; i++)
        {
            var @event = events[i];
            
            var row = i + 2;

            worksheet.Cell(row, 1).Value = @event.TimestampUtc.ToString(DateTimeFormats.DefaultFormat);
            worksheet.Cell(row, 2).Value = @event.IsUp ? "Up ✅" : "Down ⚠️";
            worksheet.Cell(row, 3).Value = @event.LatencyMilliseconds ?? 0;
            worksheet.Cell(row, 4).Value = @event.Message ?? "";
            worksheet.Cell(row, 5).Value = @event.FalsePositive ? "True" : "False";
            worksheet.Cell(row, 6).Value = @event.Category ?? "";
            worksheet.Cell(row, 7).Value = @event.Note ?? "";
            worksheet.Cell(row, 8).Value = @event.TicketId ?? "";
            worksheet.Cell(row, 9).Value = @event.MaintenanceType ?? "";
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        
        workbook.SaveAs(stream);
        
        return stream.ToArray();
    }
}