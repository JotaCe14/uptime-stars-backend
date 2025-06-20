using System.Windows.Input;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.UpdateGroup;
public record UpdateGroupCommand(Guid Id, string Name, string? Description = "") : ICommand<Result>;