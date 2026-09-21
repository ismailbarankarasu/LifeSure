using LifeSure.Data;
using LifeSure.Mediator.TeamMembers.Queries;
using LifeSure.Mediator.TeamMembers.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.TeamMembers.Handlers;

public class GetTeamMembersQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetTeamMembersQuery, List<TeamMemberResult>>
{
    public async Task<List<TeamMemberResult>> Handle(
        GetTeamMembersQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.TeamMembers
            .AsNoTracking()
            .AsQueryable();

        if (request.OnlyActive)
        {
            query = query.Where(x => x.IsActive);
        }

        var members = await query
            .Include(x => x.Translations)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return members
            .Select(TeamMemberResult.FromEntity)
            .ToList();
    }
}

public class GetTeamMemberByIdQueryHandler(LifeSureDbContext context)
    : IRequestHandler<GetTeamMemberByIdQuery, TeamMemberResult?>
{
    public async Task<TeamMemberResult?> Handle(
        GetTeamMemberByIdQuery request,
        CancellationToken cancellationToken)
    {
        var member = await context.TeamMembers
            .AsNoTracking()
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        return member is null
            ? null
            : TeamMemberResult.FromEntity(member);
    }
}