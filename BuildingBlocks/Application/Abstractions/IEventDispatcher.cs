using System.Threading.Tasks;
using System.Threading;

namespace BuildingBlocks.Application.Abstractions;

public interface IEventDispatcher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : notnull;
}
