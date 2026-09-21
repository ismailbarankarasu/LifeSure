using LifeSure.Data;
using LifeSure.Entities;
using LifeSure.Mediator.TeamMembers.Commands;
using LifeSure.Mediator.TeamMembers.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.TeamMembers.Handlers;

public class SaveTeamMemberCommandHandler(LifeSureDbContext context)
    : IRequestHandler<SaveTeamMemberCommand, int?>
{
    public async Task<int?> Handle(
        SaveTeamMemberCommand request,
        CancellationToken cancellationToken)
    {
        var input = request.Input;

        TeamMemberInputValidator.Validate(input);

        TeamMember member;

        if (request.Id.HasValue)
        {
            var existing = await context.TeamMembers
                .Include(x => x.Translations)
                .SingleOrDefaultAsync(
                    x => x.Id == request.Id.Value,
                    cancellationToken);

            if (existing is null)
            {
                return null;
            }

            member = existing;
        }
        else
        {
            member = new TeamMember();
            context.TeamMembers.Add(member);
        }

        member.FullName = input.FullName.Trim();
        member.ImageUrl = input.ImageUrl;
        member.FacebookUrl = NormalizeUrl(input.FacebookUrl);
        member.InstagramUrl = NormalizeUrl(input.InstagramUrl);
        member.LinkedInUrl = NormalizeUrl(input.LinkedInUrl);
        member.XUrl = NormalizeUrl(input.XUrl);
        member.IsActive = input.IsActive;
        member.DisplayOrder = input.DisplayOrder;

        foreach (var translationInput in input.Translations)
        {
            var translation = member.Translations
                .FirstOrDefault(
                    x => x.LanguageCode == translationInput.LanguageCode);

            if (translation is null)
            {
                translation = new TeamMemberTranslation
                {
                    LanguageCode = translationInput.LanguageCode
                };

                member.Translations.Add(translation);
            }

            translation.JobTitle = translationInput.JobTitle.Trim();
        }

        var submittedLanguages = input.Translations
            .Select(x => x.LanguageCode)
            .ToHashSet();

        var removedTranslations = member.Translations
            .Where(x => !submittedLanguages.Contains(x.LanguageCode))
            .ToList();

        context.TeamMemberTranslations.RemoveRange(removedTranslations);

        await context.SaveChangesAsync(cancellationToken);

        return member.Id;
    }

    private static string? NormalizeUrl(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}

public class DeleteTeamMemberCommandHandler(LifeSureDbContext context)
    : IRequestHandler<DeleteTeamMemberCommand, bool>
{
    public async Task<bool> Handle(
        DeleteTeamMemberCommand request,
        CancellationToken cancellationToken)
    {
        var member = await context.TeamMembers
            .SingleOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (member is null)
        {
            return false;
        }

        context.TeamMembers.Remove(member);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}