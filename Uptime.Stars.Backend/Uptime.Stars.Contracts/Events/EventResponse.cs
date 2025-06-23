namespace Uptime.Stars.Contracts.Events;

public record EventResponse(
    Guid Id,
    string TimestampUtc,
    bool IsUp,
    bool IsImportant,
    string Message,
    long LatencyMilliseconds,
    bool FalsePositive,
    string Category,
    string Note,
    string TicketId,
    string MaintenanceType);