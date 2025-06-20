using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.UpdateGroup;
internal sealed class UpdateGroupCommandHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork) : ICommandHandler<UpdateGroupCommand, Result>
{
    public async Task<Result> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetByIdAsync(request.Id, cancellationToken);

        if (group is null)
        {
            return Result.Failure(Error.Failure("UpdateGroup.Handle", "Group not found"));
        }

        group.Name = request.Name;
        group.Description = request.Description;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}