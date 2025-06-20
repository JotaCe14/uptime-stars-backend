using Uptime.Stars.Domain.Core.Primitives;

namespace Uptime.Stars.Domain.Entities;
public class Group : Entity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<ComponentMonitor> Components { get; set; } = [];
}