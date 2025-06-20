namespace Uptime.Stars.Contracts.Monitor;
public record MonitorResponse(
    Guid Id,
    string Name,
    string Description,
    string Target,
    string CreatedAtUtc,
    bool IsActive,
    IReadOnlyCollection<EventResponse> LastEvents,
    decimal Uptime24h,
    decimal Uptime30d);