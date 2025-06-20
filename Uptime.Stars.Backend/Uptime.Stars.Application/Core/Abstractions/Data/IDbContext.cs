using Microsoft.EntityFrameworkCore;
using Uptime.Stars.Domain.Core.Primitives;

namespace Uptime.Stars.Application.Core.Abstractions.Data;

public interface IDbContext : IDisposable
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : Entity;

    void Migrate();
}