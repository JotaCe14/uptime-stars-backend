using MediatR;

namespace Uptime.Stars.Application.Core.Abstractions.Messaging;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : INotification
{
}