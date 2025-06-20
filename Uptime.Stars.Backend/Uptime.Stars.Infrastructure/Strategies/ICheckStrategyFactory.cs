using Microsoft.Extensions.DependencyInjection;
using Uptime.Stars.Domain.Enums;

namespace Uptime.Stars.Infrastructure.Strategies;
internal interface ICheckStrategyFactory
{
    ICheckStrategy? GetStrategy(MonitorType type);
}

internal class CheckStrategyFactory(IServiceProvider serviceProvider) : ICheckStrategyFactory
{
    public ICheckStrategy? GetStrategy(MonitorType type)
    {
        return serviceProvider.GetKeyedService<ICheckStrategy>(type);
    }
}