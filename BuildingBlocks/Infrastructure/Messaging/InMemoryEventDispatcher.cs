using BuildingBlocks.Application.Abstractions;
using MediatR;

namespace BuildingBlocks.Infrastructure.Messaging
{
  
    public class InMemoryEventDispatcher : IEventDispatcher
    {
        private readonly IPublisher _publisher;

        public InMemoryEventDispatcher(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
            where TEvent : notnull
        {
            // Mesajı sisteme fırlatır, dinleyen handler'lar tetiklenir
            await _publisher.Publish(@event, cancellationToken);
        }
    }
}
