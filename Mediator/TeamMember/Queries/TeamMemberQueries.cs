using LifeSure.Mediator.TeamMembers.Results;
using MediatR;

namespace LifeSure.Mediator.TeamMembers.Queries;

public record GetTeamMembersQuery(
    bool OnlyActive = false) : IRequest<List<TeamMemberResult>>;

public record GetTeamMemberByIdQuery(
    int Id) : IRequest<TeamMemberResult?>;