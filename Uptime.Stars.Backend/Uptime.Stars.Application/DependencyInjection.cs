using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using System.Reflection;
using Uptime.Stars.Application.Core.Behaviors;

namespace Uptime.Stars.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddFeatureManagement();

        return services
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
            .AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                configuration.AddOpenBehavior(typeof(ValidationBehaviour<,>));
                configuration.AddOpenBehavior(typeof(TransactionBehaviour<,>));
                configuration.AddOpenBehavior(typeof(LoggingPipelineBehaviour<,>));
            });
    }
}