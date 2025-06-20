using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.DisableMonitor;
public record DisableMonitorCommand(Guid MonitorId) : ICommand<Result>;