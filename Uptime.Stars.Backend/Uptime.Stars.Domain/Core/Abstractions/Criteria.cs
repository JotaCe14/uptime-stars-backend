using Uptime.Stars.Domain.Core.Primitives;

namespace Uptime.Stars.Domain.Core.Abstractions;
public abstract class Criteria<TEntity> where TEntity : Entity
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string SortBy { get; set; } = "Id";
    public bool SortDescending { get; set; } = true;
    public abstract IQueryable<TEntity> BuildQueryWithCriteria(IQueryable<TEntity> query);
}