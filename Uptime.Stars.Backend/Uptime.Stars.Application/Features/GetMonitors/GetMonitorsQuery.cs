using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Contracts.Monitors;
using X.PagedList;

namespace Uptime.Stars.Application.Features.GetMonitors;

public record GetMonitorsQuery(int PageSize, int PageNumber) : IQuery<IPagedList<MonitorResponse>>;