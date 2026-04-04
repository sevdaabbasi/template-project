using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Data;
using BuildingBlocks.Domain;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

public abstract class AppDbContextBase : DbContext, IUnitOfWork
{

     private readonly IEventDispatcher? _eventDispatcher;

     protected AppDbContextBase(
         DbContextOptions options,
         IEventDispatcher? eventDispatcher = null) : base(options)
     {
         _eventDispatcher = eventDispatcher;
     }

     public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
  var domainEvents = ChangeTracker.Entries<IHasDomainEvents>()
            .Select(x => x.Entity)
            .SelectMany(x =>
            {
                var events = x.DomainEvents.ToList();
                x.ClearDomainEvents(); // Event'leri topladıktan sonra temizle ki tekrar fırlatılmasın
                return events;
            })
            .ToList();
        // 2. Veritabanına asıl kaydı yap
        var result = await base.SaveChangesAsync(cancellationToken);
        // 3. Eğer olay (event) varsa ve dispatcher tanımlıysa, olayları sisteme fırlat
        if (_eventDispatcher != null)
        {
            foreach (var domainEvent in domainEvents)
            {
                await _eventDispatcher.PublishAsync(domainEvent, cancellationToken);
            }
        }
        return result;
    }
}
