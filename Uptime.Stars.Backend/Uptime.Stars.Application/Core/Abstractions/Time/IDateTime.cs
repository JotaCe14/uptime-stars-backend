namespace Uptime.Stars.Application.Core.Abstractions.Time;

public interface IDateTime
{
    DateTime UtcNow { get; }
}