using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Application.Services;
public interface IReportService
{
    byte[] GenerateReport(List<Event> events);
}