using System.Diagnostics;
using Uptime.Stars.Application.Core.Abstractions.Time;

namespace Uptime.Stars.Infrastructure.Time;
internal sealed class MachineTimer : IClockTimer
{
    private readonly Stopwatch stopwatch = new();

    public long ElapsedMilliseconds => stopwatch.ElapsedMilliseconds;

    public void Start() => stopwatch.Start();

    public void Stop() => stopwatch.Stop();
}