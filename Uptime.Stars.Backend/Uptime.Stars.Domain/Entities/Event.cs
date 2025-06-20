using Uptime.Stars.Domain.Core.Primitives;

namespace Uptime.Stars.Domain.Entities;
public class Event : AggregateRoot
{
    protected Event() { }
    private Event(
        Guid monitorId,
        DateTime timestampUtc,
        bool isUp = true,
        string? message = null,
        long? latencyMilliseconds = null)
    {
        MonitorId = monitorId;
        IsUp = isUp;
        Message = message;
        TimestampUtc = timestampUtc;
        LatencyMilliseconds = latencyMilliseconds;
    }
    public static Event Create(
        Guid MonitorId, 
        DateTime timestampUtc,
        bool isUp = true,
        string? message = null,
        long? latencyMilliseconds = null)
    {
        return new Event(MonitorId, timestampUtc, isUp, message, latencyMilliseconds);
    }

    public void Update(
        bool falsePositive = false,
        string? category = "",
        string? note = "",
        string? ticketId = "",
        string? maintenanceType = "")
    {
        FalsePositive = falsePositive;
        Category = category;
        Note = note;
        TicketId = ticketId;
        MaintenanceType = maintenanceType;
        TimestampUtc = DateTime.UtcNow;
    }

    public Guid MonitorId { get; private set; }
    public ComponentMonitor? Monitor { get; }
    public bool IsUp { get; private set; } = true;
    public long? LatencyMilliseconds { get; private set; }
    public DateTime TimestampUtc { get; private set; } = DateTime.UtcNow;
    public string? Message { get; private set; }
    public bool FalsePositive { get; private set; } = false;
    public string? Category { get; private set; } = "";
    public string? Note { get; private set; } = "";
    public string? TicketId { get; private set; } = "";
    public string? MaintenanceType { get; set; } = "";
}