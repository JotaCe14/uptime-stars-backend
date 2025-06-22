using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.GenerateReport;
public record GenerateMonitorReportCommand(string DateFrom, string DateTo) : ICommand<Result<byte[]>>;