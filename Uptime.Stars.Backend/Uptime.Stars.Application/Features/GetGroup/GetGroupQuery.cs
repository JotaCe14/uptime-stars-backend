using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Contracts.Groups;
using Uptime.Stars.Domain.Core.Primitives.Result;

namespace Uptime.Stars.Application.Features.GetGroup;
public record GetGroupQuery(Guid GroupId) : IQuery<Result<GroupResponse>>;