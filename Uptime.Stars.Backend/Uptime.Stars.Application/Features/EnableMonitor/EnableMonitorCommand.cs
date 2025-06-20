using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.EnableMonitor;
public record EnableMonitorCommand(Guid MonitorId) : ICommand<Result>;