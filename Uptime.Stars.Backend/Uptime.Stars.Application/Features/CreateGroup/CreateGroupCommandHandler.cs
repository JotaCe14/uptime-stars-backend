using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Domain.Core.Primitives.Result;
using Uptime.Stars.Domain.Entities;
using Uptime.Stars.Domain.Repositories;

namespace Uptime.Stars.Application.Features.CreateGroup;
internal sealed class CreateGroupCommandHandler(IGroupRepository groupRepository, IUnitOfWork unitOfWork) : ICommandHandler<CreateGroupCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = new Group
        {
            Name = request.Name,
            Description = request.Description
        };

        await groupRepository.AddAsync(group, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}