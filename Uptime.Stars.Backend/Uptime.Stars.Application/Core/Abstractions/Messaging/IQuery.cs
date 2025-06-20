using MediatR;

namespace Uptime.Stars.Application.Core.Abstractions.Messaging;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}