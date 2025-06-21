using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.GenerateEventsReport;

public record GenerateEventsReportCommand(Guid MonitorId) : ICommand<Result<byte[]>>;