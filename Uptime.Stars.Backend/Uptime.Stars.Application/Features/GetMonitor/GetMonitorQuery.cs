using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Contracts.Monitors;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.GetMonitor;

public record GetMonitorQuery(Guid MonitorId, int LastEventsLimit = 50) : IQuery<Result<MonitorResponse>>;