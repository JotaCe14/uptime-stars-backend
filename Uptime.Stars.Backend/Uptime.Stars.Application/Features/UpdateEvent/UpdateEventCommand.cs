using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.UpdateEvent;
public record UpdateEventCommand(
    Guid Id, 
    bool FalsePositive = false,
    string? Category = "",
    string? Note = "",
    string? TicketId = "",
    string? MaintenanceType = "") : ICommand<Result>;