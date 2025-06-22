using System.Net.NetworkInformation;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Infrastructure.Strategies.Implementations;
internal sealed class PingCheckStrategy(IPingWrapper pingWrapper) : ICheckStrategy
{
    public async Task<CheckResult> CheckAsync(ComponentMonitor monitor, CancellationToken cancellationToken = default)
    {
        using var ping = new Ping();

        try
        {
            var reply = await pingWrapper.SendPingAsync(monitor.Target, monitor.TiemoutInMilliseconds);

            if (reply is null)
            {
                return CheckResult.Down("No response");
            }

            return CheckResult.Up(reply.RoundtripTime);
        }
        catch (Exception ex)
        {
            return CheckResult.Down(ex.Message);
        }
    }
}

internal interface IPingWrapper
{
    Task<PingReplyWrapper?> SendPingAsync(string host, int timeout);
}

internal class PingWrapper : IPingWrapper
{
    public async Task<PingReplyWrapper?> SendPingAsync(string host, int timeout)
    {
        using var ping = new Ping();

        var reply = await ping.SendPingAsync(host, timeout);

        return new PingReplyWrapper(reply.RoundtripTime);
    }
}

public record PingReplyWrapper(long RoundtripTime);