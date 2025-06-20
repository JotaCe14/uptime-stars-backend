using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.RemoveMonitor;
public record RemoveMonitorCommand(Guid MonitorId) : ICommand<Result>;