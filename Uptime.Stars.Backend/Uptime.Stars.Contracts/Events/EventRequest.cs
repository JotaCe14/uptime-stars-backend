namespace Uptime.Stars.Contracts.Events;
public class EventRequest
{
    public bool FalsePositive { get; set; } = false;
    public int? Category { get; set; }
    public string? Note { get; set; } = "";
    public string? TicketId { get; set; } = "";
    public int? MaintenanceType { get; set; }
}