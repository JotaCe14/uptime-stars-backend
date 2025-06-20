using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Contracts.Groups;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.GetGroup;
internal sealed class GetGroupQueryHandler(IGroupRepository groupRepository) : IQueryHandler<GetGroupQuery, Result<GroupResponse>>
{
    public async Task<Result<GroupResponse>> Handle(GetGroupQuery request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure<GroupResponse>(Error.Failure("GetGroup.Handle", "Group not found"));
        }

        return new GroupResponse(group.Id, group.Name, group.Description);
    }
}