using MediatR;

namespace Uptime.Stars.Application.Core.Abstractions.Messaging;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}