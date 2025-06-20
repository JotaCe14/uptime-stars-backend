namespace Uptime.Stars.Contracts.Events;
public class EventRequest
{
    public bool FalsePositive { get; set; } = false;
    public string? Category { get; set; } = "";
    public string? Note { get; set; } = "";
    public string? TicketId { get; set; } = "";
    public string? MaintenanceType { get; set; } = "";
}