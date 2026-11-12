using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Application.Common;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Domain.Enums;

namespace Modules.Identity.Application.Features.Users;

#region Complete Onboarding
public record CompleteOnboardingCommand(
    Guid UserId,
    int CityId,
    List<int> InterestIds
) : ICommand<UserDto>;

public class CompleteOnboardingCommandValidator : AbstractValidator<CompleteOnboardingCommand>
{
    public CompleteOnboardingCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.CityId).GreaterThan(0).WithMessage("Geçerli bir şehir seçiniz.");
        RuleFor(x => x.InterestIds).NotEmpty().WithMessage("En az bir ilgi alanı seçmelisiniz.");
    }
}

public class CompleteOnboardingCommandHandler : ICommandHandler<CompleteOnboardingCommand, UserDto>
{
    private readonly IIdentityDbContext _context;

    public CompleteOnboardingCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        var cityExists = await _context.Cities.AnyAsync(c => c.Id == request.CityId && c.IsActive, cancellationToken);
        if (!cityExists)
        {
            throw new NotFoundException("Seçilen şehir bulunamadı veya aktif değil.");
        }

        var validInterestIds = await _context.Interests
            .Where(i => request.InterestIds.Contains(i.Id) && i.IsActive)
            .Select(i => i.Id)
            .ToListAsync(cancellationToken);

        var user = await _context.Users
            .Include(u => u.City)
            .Include(u => u.UserInterests)
                .ThenInclude(ui => ui.Interest)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }

        user.CompleteOnboarding(request.CityId, validInterestIds);
        await _context.SaveChangesAsync(cancellationToken);

        // Şehir bilgisini yükle
        await _context.Cities.Where(c => c.Id == request.CityId).LoadAsync(cancellationToken);

        return UserMapper.ToDto(user);
    }
}
#endregion

#region Profile Management
public record UpdateProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? Bio,
    DateOnly? DateOfBirth = null,
    Gender? Gender = null
) : ICommand<UserDto>;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad boş bırakılamaz.").MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad boş bırakılamaz.").MaximumLength(50);
        RuleFor(x => x.Bio).MaximumLength(500);
    }
}

public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, UserDto>
{
    private readonly IIdentityDbContext _context;

    public UpdateProfileCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.City)
            .Include(u => u.UserInterests)
                .ThenInclude(ui => ui.Interest)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }

        user.UpdateProfile(request.FirstName, request.LastName, request.Bio, request.DateOfBirth, request.Gender);
        await _context.SaveChangesAsync(cancellationToken);

        return UserMapper.ToDto(user);
    }
}

public record UpdateAvatarCommand(
    Guid UserId,
    string AvatarUrl
) : ICommand<string>;

public class UpdateAvatarCommandHandler : ICommandHandler<UpdateAvatarCommand, string>
{
    private readonly IIdentityDbContext _context;

    public UpdateAvatarCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(UpdateAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }

        user.SetAvatar(request.AvatarUrl);
        await _context.SaveChangesAsync(cancellationToken);

        return user.AvatarUrl ?? request.AvatarUrl;
    }
}

public record GetMyProfileQuery(Guid UserId) : IQuery<UserDto>;

public class GetMyProfileQueryHandler : IQueryHandler<GetMyProfileQuery, UserDto>
{
    private readonly IIdentityDbContext _context;

    public GetMyProfileQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.City)
            .Include(u => u.UserInterests)
                .ThenInclude(ui => ui.Interest)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Kullanıcı profili bulunamadı.");
        }

        return UserMapper.ToDto(user);
    }
}

public record GetUserProfileByIdQuery(Guid TargetUserId) : IQuery<UserDto>;

public class GetUserProfileByIdQueryHandler : IQueryHandler<GetUserProfileByIdQuery, UserDto>
{
    private readonly IIdentityDbContext _context;

    public GetUserProfileByIdQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.City)
            .Include(u => u.UserInterests)
                .ThenInclude(ui => ui.Interest)
            .FirstOrDefaultAsync(u => u.Id == request.TargetUserId && u.IsActive, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("Kullanıcı profili bulunamadı.");
        }

        return UserMapper.ToDto(user);
    }
}
#endregion
