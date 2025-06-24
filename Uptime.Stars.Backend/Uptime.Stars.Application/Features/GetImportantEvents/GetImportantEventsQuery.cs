using Uptime.Stars.Application.Core.Abstractions.Messaging;
using Uptime.Stars.Contracts.Events;
using X.PagedList;

namespace Uptime.Stars.Application.Features.GetEvents;
public record GetImportantEventsQuery(int PageSize, int PageNumber, Guid? MonitorId = default) : IQuery<IPagedList<EventResponse>>;