using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Enums;

namespace Uptime.Stars.Application.Features.UpdateEvent;
public record UpdateEventCommand(
    Guid Id,
    Category? Category,
    MaintenanceType? MaintenanceType,
    bool FalsePositive = false,
    string? Note = "",
    string? TicketId = "") : ICommand<Result>;