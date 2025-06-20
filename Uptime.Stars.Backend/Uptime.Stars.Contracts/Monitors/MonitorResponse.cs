using Uptime.Stars.Contracts.Events;

namespace Uptime.Stars.Contracts.Monitors;

public class MonitorResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Guid? GroupId { get; set; }
    public string Target { get; set; } = "";
    public string CreatedAtUtc { get; set; } = "";
    public bool IsActive { get; set; }
    public IReadOnlyCollection<EventResponse>? LastEvents { get; set; }
    public string Uptime24hPercentage { get; set; } = "";
    public string Uptime30dPercentage { get; set; } = "";
}