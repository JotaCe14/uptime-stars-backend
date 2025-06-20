using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Enums;

namespace Uptime.Stars.Application.Features.CreateMonitor;
public sealed record CreateMonitorCommand(
    string Name,
    string Description,
    Guid? GroupId,
    MonitorType Type,
    string Target,
    int IntervalInMinutes,
    int TiemoutInMilliseconds,
    string[] RequestHeaders,
    TextSearchMode? SearchMode,
    string? ExpectedText,
    string[] AlertEmails,
    string? AlertMessage,
    int AlertDelayMinutes,
    int AlertResendCycles) : ICommand<Result<Guid>>;