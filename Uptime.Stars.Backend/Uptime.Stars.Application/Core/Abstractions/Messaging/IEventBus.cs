namespace Uptime.Stars.Application.Core.Abstractions.Messaging;
public interface IEventBus
{
    Task PublishAsync<T>(
        T integrationEvent,
        CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent;
}