using System.Threading;
using Uptime.Stars.Domain.Entities;

namespace Uptime.Stars.Application.Services;
public interface IAlertService
{
    Task SendAlertAsync(ComponentMonitor monitor, Event @event, CancellationToken cancellationToken = default);
}