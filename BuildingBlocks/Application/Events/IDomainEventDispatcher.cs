using System.Threading.Tasks;
using BuildingBlocks.Domain;

namespace BuildingBlocks.Application.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents);
    }
}
