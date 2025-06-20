namespace Uptime.Stars.Contracts.Monitor;

public record EventResponse(
    string TimestampUtc,
    bool IsUp,
    string Message,
    long LatencyMilliseconds,
    bool FalsePositive,
    string Category,
    string Note,
    string TicketId,
    string MaintenanceType);