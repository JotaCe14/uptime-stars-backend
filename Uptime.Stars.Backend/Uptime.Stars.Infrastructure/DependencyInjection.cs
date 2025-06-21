using Microsoft.Extensions.DependencyInjection;
using Uptime.Stars.Application.Core.Abstractions.Time;
using Uptime.Stars.Application.Services;
using Uptime.Stars.Domain.Enums;
using Uptime.Stars.Infrastructure.Jobs;
using Uptime.Stars.Infrastructure.Services;
using Uptime.Stars.Infrastructure.Strategies;
using Uptime.Stars.Infrastructure.Strategies.Implementations;
using Uptime.Stars.Infrastructure.Time;

namespace Uptime.Stars.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddHttpClient<HttpsGetCheckStrategy>();

        return services
            .AddScoped<IReportService, ReportService>()
            .AddScoped<IAlertService, SmtpAlertService>()
            .AddScoped<IEventService, EventService>()
            .AddScoped<IAlertScheduler, AlertScheduler>()
            .AddScoped<IMonitorScheduler, MonitorScheduler>()
            .AddScoped<AlertJob>()
            .AddScoped<MonitorJob>()
            .AddScoped<ICheckStrategyFactory, CheckStrategyFactory>()
            .AddScoped<IPingWrapper, PingWrapper>()
            .AddKeyedScoped<ICheckStrategy, HttpsGetCheckStrategy>(MonitorType.Https)
            .AddKeyedScoped<ICheckStrategy, PingCheckStrategy>(MonitorType.Ping)
            .AddTransient<IDateTime, MachineDateTime>()
            .AddTransient<IClockTimer, MachineTimer>();
    }
}