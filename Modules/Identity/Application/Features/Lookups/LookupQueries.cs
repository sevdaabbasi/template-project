using BuildingBlocks.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Application.DTOs;

namespace Modules.Identity.Application.Features.Lookups;

public record GetCitiesQuery : IQuery<List<CityDto>>;

public class GetCitiesQueryHandler : IQueryHandler<GetCitiesQuery, List<CityDto>>
{
    private readonly IIdentityDbContext _context;

    public GetCitiesQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<CityDto>> Handle(GetCitiesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CityDto(c.Id, c.Name, c.PlateCode))
            .ToListAsync(cancellationToken);
    }
}

public record GetInterestsQuery : IQuery<List<InterestDto>>;

public class GetInterestsQueryHandler : IQueryHandler<GetInterestsQuery, List<InterestDto>>
{
    private readonly IIdentityDbContext _context;

    public GetInterestsQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<InterestDto>> Handle(GetInterestsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Interests
            .Where(i => i.IsActive)
            .OrderBy(i => i.Id)
            .Select(i => new InterestDto(i.Id, i.Name, i.IconName))
            .ToListAsync(cancellationToken);
    }
}
