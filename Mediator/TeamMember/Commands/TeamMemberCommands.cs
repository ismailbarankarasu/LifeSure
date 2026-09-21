using LifeSure.Mediator.TeamMembers.Models;
using MediatR;

namespace LifeSure.Mediator.TeamMembers.Commands;

public record SaveTeamMemberCommand(
    int? Id,
    TeamMemberInput Input) : IRequest<int?>;

public record DeleteTeamMemberCommand(int Id) : IRequest<bool>;