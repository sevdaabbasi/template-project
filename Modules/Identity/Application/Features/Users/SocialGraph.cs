using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Abstractions;
using Modules.Identity.Application.DTOs;
using Modules.Identity.Domain.Entities;

namespace Modules.Identity.Application.Features.Users;

#region Follow User
public record FollowUserCommand(
    Guid FollowerId,
    Guid FollowedId
) : ICommand<bool>;

public class FollowUserCommandHandler : ICommandHandler<FollowUserCommand, bool>
{
    private readonly IIdentityDbContext _context;

    public FollowUserCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        if (request.FollowerId == request.FollowedId)
        {
            throw new InvalidOperationException("Kendinizi takip edemezsiniz.");
        }

        var follower = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.FollowerId, cancellationToken);
        var followed = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.FollowedId, cancellationToken);

        if (follower == null || followed == null)
        {
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }

        var alreadyFollowing = await _context.UserFollowers
            .AnyAsync(f => f.FollowerId == request.FollowerId && f.FollowedId == request.FollowedId, cancellationToken);

        if (alreadyFollowing)
        {
            return true; // Zaten takip ediyor
        }

        var followRelation = new UserFollower(request.FollowerId, request.FollowedId);
        await _context.UserFollowers.AddAsync(followRelation, cancellationToken);

        follower.IncrementFollowing();
        followed.IncrementFollowers();

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
#endregion

#region Unfollow User
public record UnfollowUserCommand(
    Guid FollowerId,
    Guid FollowedId
) : ICommand<bool>;

public class UnfollowUserCommandHandler : ICommandHandler<UnfollowUserCommand, bool>
{
    private readonly IIdentityDbContext _context;

    public UnfollowUserCommandHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var relation = await _context.UserFollowers
            .FirstOrDefaultAsync(f => f.FollowerId == request.FollowerId && f.FollowedId == request.FollowedId, cancellationToken);

        if (relation == null)
        {
            return false;
        }

        var follower = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.FollowerId, cancellationToken);
        var followed = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.FollowedId, cancellationToken);

        _context.UserFollowers.Remove(relation);

        follower?.DecrementFollowing();
        followed?.DecrementFollowers();

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
#endregion

#region Get Followers / Following
public record GetFollowersQuery(Guid TargetUserId, Guid CurrentUserId) : IQuery<List<UserFollowerDto>>;

public class GetFollowersQueryHandler : IQueryHandler<GetFollowersQuery, List<UserFollowerDto>>
{
    private readonly IIdentityDbContext _context;

    public GetFollowersQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserFollowerDto>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        var followers = await _context.UserFollowers
            .Where(f => f.FollowedId == request.TargetUserId)
            .Include(f => f.Follower)
            .Select(f => f.Follower)
            .ToListAsync(cancellationToken);

        var currentFollowingIds = await _context.UserFollowers
            .Where(f => f.FollowerId == request.CurrentUserId)
            .Select(f => f.FollowedId)
            .ToListAsync(cancellationToken);

        return followers.Select(u => new UserFollowerDto(
            Id: u.Id,
            FullName: u.FullName,
            AvatarUrl: u.AvatarUrl,
            Bio: u.Bio,
            IsFollowing: currentFollowingIds.Contains(u.Id)
        )).ToList();
    }
}

public record GetFollowingQuery(Guid TargetUserId, Guid CurrentUserId) : IQuery<List<UserFollowerDto>>;

public class GetFollowingQueryHandler : IQueryHandler<GetFollowingQuery, List<UserFollowerDto>>
{
    private readonly IIdentityDbContext _context;

    public GetFollowingQueryHandler(IIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserFollowerDto>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
    {
        var following = await _context.UserFollowers
            .Where(f => f.FollowerId == request.TargetUserId)
            .Include(f => f.Followed)
            .Select(f => f.Followed)
            .ToListAsync(cancellationToken);

        var currentFollowingIds = await _context.UserFollowers
            .Where(f => f.FollowerId == request.CurrentUserId)
            .Select(f => f.FollowedId)
            .ToListAsync(cancellationToken);

        return following.Select(u => new UserFollowerDto(
            Id: u.Id,
            FullName: u.FullName,
            AvatarUrl: u.AvatarUrl,
            Bio: u.Bio,
            IsFollowing: currentFollowingIds.Contains(u.Id)
        )).ToList();
    }
}
#endregion
