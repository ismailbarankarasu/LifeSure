using LifeSure.Entities;
using LifeSure.Mediator.SiteSettings.Models;

namespace LifeSure.Mediator.SiteSettings.Results;

public record SiteSettingResult(
    int Id,
    string SiteName,
    string? LogoUrl,
    string Email,
    string PhoneNumber,
    string Address,
    string? MapUrl,
    string? FacebookUrl,
    string? InstagramUrl,
    string? LinkedInUrl,
    string? XUrl,
    IReadOnlyList<SiteSettingTranslationDto> Translations)
{
    public static SiteSettingResult FromEntity(SiteSetting setting)
    {
        return new SiteSettingResult(
            setting.Id,
            setting.SiteName,
            setting.LogoUrl,
            setting.Email,
            setting.PhoneNumber,
            setting.Address,
            setting.MapUrl,
            setting.FacebookUrl,
            setting.InstagramUrl,
            setting.LinkedInUrl,
            setting.XUrl,
            setting.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new SiteSettingTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    FooterDescription = x.FooterDescription,
                    MetaDescription = x.MetaDescription
                })
                .ToList());
    }
}