using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Application.Features.Lookups;

namespace Modules.Identity.Api.Controllers;

[ApiController]
[Route("api/v1/cities")]
[AllowAnonymous]
public class CitiesController : ControllerBase
{
    private readonly ISender _sender;

    public CitiesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Sistemde tanımlı ve aktif olan şehirleri listeler (Onboarding ve filtreler için).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<CityDto>>> GetCities(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCitiesQuery(), cancellationToken);
        return Ok(result);
    }
}

[ApiController]
[Route("api/v1/interests")]
[AllowAnonymous]
public class InterestsController : ControllerBase
{
    private readonly ISender _sender;

    public InterestsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Sistemde tanımlı ilgi alanlarını listeler (Doğa, Kültür, Spor, Sanat).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<InterestDto>>> GetInterests(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetInterestsQuery(), cancellationToken);
        return Ok(result);
    }
}

