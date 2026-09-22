using LifeSure.Data;
using LifeSure.Entities;
using LifeSure.Mediator.SiteSettings.Commands;
using LifeSure.Mediator.SiteSettings.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.Mediator.SiteSettings.Handlers;

public class SaveSiteSettingCommandHandler(LifeSureDbContext context)
    : IRequestHandler<SaveSiteSettingCommand, int>
{
    public async Task<int> Handle(
        SaveSiteSettingCommand request,
        CancellationToken cancellationToken)
    {
        var input = request.Input;

        SiteSettingInputValidator.Validate(input);

        var setting = await context.SiteSettings
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.SingletonKey == 1,
                cancellationToken);

        if (setting is null)
        {
            setting = new SiteSetting
            {
                SingletonKey = 1
            };

            context.SiteSettings.Add(setting);
        }

        setting.SiteName = input.SiteName.Trim();
        setting.Email = input.Email.Trim();
        setting.PhoneNumber = input.PhoneNumber.Trim();
        setting.Address = input.Address.Trim();
        setting.MapUrl = NormalizeOptional(input.MapUrl);
        setting.FacebookUrl = NormalizeOptional(input.FacebookUrl);
        setting.InstagramUrl = NormalizeOptional(input.InstagramUrl);
        setting.LinkedInUrl = NormalizeOptional(input.LinkedInUrl);
        setting.XUrl = NormalizeOptional(input.XUrl);

        foreach (var translationInput in input.Translations)
        {
            var translation = setting.Translations
                .FirstOrDefault(
                    x => x.LanguageCode == translationInput.LanguageCode);

            if (translation is null)
            {
                translation = new SiteSettingTranslation
                {
                    LanguageCode = translationInput.LanguageCode
                };

                setting.Translations.Add(translation);
            }

            translation.FooterDescription =
                translationInput.FooterDescription.Trim();

            translation.MetaDescription =
                translationInput.MetaDescription.Trim();
        }

        var submittedLanguages = input.Translations
            .Select(x => x.LanguageCode)
            .ToHashSet();

        var removedTranslations = setting.Translations
            .Where(x => !submittedLanguages.Contains(x.LanguageCode))
            .ToList();

        context.SiteSettingTranslations.RemoveRange(removedTranslations);

        await context.SaveChangesAsync(cancellationToken);

        return setting.Id;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}