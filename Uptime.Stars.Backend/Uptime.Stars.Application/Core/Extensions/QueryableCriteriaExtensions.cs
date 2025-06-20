using Uptime.Stars.Domain.Core.Abstractions;
using Uptime.Stars.Domain.Core.Primitives;

namespace Uptime.Stars.Application.Core.Extensions;
internal static class QueryableCriteriaExtensions
{
    public static IQueryable<TEntity> ApplyCriteria<TEntity>(this IQueryable<TEntity> inputQuery, Criteria<TEntity>? criteria)
        where TEntity : Entity
    {
        var query = inputQuery;

        if (criteria is null)
        {
            return query;
        }

        return criteria.BuildQueryWithCriteria(query);
    }
}