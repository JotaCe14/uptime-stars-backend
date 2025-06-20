using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.RemoveGroup;
public record RemoveGroupCommand(Guid GroupId) : ICommand<Result>;