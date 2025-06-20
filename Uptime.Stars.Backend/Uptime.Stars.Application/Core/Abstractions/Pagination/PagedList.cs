namespace Uptime.Stars.Application.Core.Abstractions.Pagination;

public record PagedList<T>(IReadOnlyCollection<T> Data, int PageNumber, int PageSize, int TotalItemCount, int PageCount, bool HasNextPage, bool HasPreviousPage);