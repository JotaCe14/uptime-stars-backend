namespace Uptime.Stars.Application.Core.Abstractions.Time;
public interface IClockTimer
{
    void Start();
    void Stop();
    long ElapsedMilliseconds { get; }
}