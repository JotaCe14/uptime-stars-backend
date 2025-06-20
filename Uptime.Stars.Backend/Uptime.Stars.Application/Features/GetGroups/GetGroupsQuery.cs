using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Contracts.Groups;
using X.PagedList;

namespace Uptime.Stars.Application.Features.GetGroups;
public record GetGroupsQuery(int PageSize, int PageNumber) : IQuery<IPagedList<GroupResponse>>;