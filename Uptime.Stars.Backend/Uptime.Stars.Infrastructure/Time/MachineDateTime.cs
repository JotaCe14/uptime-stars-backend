using Uptime.Stars.Application.Core.Abstractions.Time;

namespace Uptime.Stars.Infrastructure.Time;

internal sealed class MachineDateTime : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}