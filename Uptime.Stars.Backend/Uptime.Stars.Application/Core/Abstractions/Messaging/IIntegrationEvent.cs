using MediatR;

namespace Uptime.Stars.Application.Core.Abstractions.Messaging;

public interface IIntegrationEvent : INotification
{
    Guid Id { get; init; }
}

public abstract record IntegrationEvent(Guid Id) : IIntegrationEvent;