using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Application.Services;
public interface IReportService
{
    byte[] GenerateEventsReport(List<Event> events);
    byte[] GenerateMonitorReport(List<Event> events, DateTime dateFrom, DateTime dateTo);
}