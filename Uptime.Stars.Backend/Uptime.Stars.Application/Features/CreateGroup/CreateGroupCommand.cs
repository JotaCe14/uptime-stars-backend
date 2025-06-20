using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.CreateGroup;
public record CreateGroupCommand(string Name, string? Description = "") : ICommand<Result<Guid>>;