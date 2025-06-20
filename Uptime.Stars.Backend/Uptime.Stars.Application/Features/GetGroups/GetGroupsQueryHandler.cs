using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Contracts.Groups;
using Uptime.Stars.Domain.Entities;
using X.PagedList;
using X.PagedList.EF;

namespace Uptime.Stars.Application.Features.GetGroups;
internal sealed class GetGroupsQueryHandler(IDbContext dbContext) : IQueryHandler<GetGroupsQuery, IPagedList<GroupResponse>>
{
    public async Task<IPagedList<GroupResponse>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Set<Group>()
            .Select(group => new GroupResponse(group.Id, group.Name, group.Description))
            .ToPagedListAsync(request.PageNumber, request.PageSize, default, cancellationToken);
    }
}