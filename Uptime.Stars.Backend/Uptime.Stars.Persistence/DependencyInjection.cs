using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Uptime.Stars.Application.Core.Abstractions.Data;
using Uptime.Stars.Domain.Repositories;
using Uptime.Stars.Persistence.Context;
using Uptime.Stars.Persistence.Repositories;

namespace Uptime.Stars.Persistence;
public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string? connectionString)
    {
        return services
            .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString))
            .AddHangfire(configuration => configuration.UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)))
            .AddScoped<IDbContext>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>())
            .AddScoped<IUnitOfWork>(serviceProvicer => serviceProvicer.GetRequiredService<ApplicationDbContext>())
            .AddScoped<IMonitorRepository, MonitorRepository>()
            .AddScoped<IEventRepository, EventRepository>();
    }
}