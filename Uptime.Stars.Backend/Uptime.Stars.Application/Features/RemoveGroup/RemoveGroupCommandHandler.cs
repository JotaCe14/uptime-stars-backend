using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.RemoveGroup;
internal sealed class RemoveGroupCommandHandler(IGroupRepository groupRepository) : ICommandHandler<RemoveGroupCommand, Result>
{
    public async Task<Result> Handle(RemoveGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.GroupId, cancellationToken);

        if (group is null)
        {
            return Result.Failure(Error.Failure("RemoveGroup.Handle", "Group not found"));
        }

        await groupRepository.DeleteAsync(group.Id, cancellationToken);

        return Result.Success();
    }
}