using LifeSure.Entities;
using LifeSure.Mediator.TeamMembers.Models;

namespace LifeSure.Mediator.TeamMembers.Results;

public record TeamMemberResult(
    int Id,
    string FullName,
    string ImageUrl,
    string? FacebookUrl,
    string? InstagramUrl,
    string? LinkedInUrl,
    string? XUrl,
    bool IsActive,
    int DisplayOrder,
    IReadOnlyList<TeamMemberTranslationDto> Translations)
{
    public static TeamMemberResult FromEntity(TeamMember member)
    {
        return new TeamMemberResult(
            member.Id,
            member.FullName,
            member.ImageUrl,
            member.FacebookUrl,
            member.InstagramUrl,
            member.LinkedInUrl,
            member.XUrl,
            member.IsActive,
            member.DisplayOrder,
            member.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new TeamMemberTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    JobTitle = x.JobTitle
                })
                .ToList());
    }
}