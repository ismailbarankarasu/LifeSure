using LifeSure.CQRS.Abouts.Models;
using LifeSure.Entities;

namespace LifeSure.CQRS.Abouts.Results;

public record AboutResult(
    int Id,
    string ImageUrl,
    bool IsActive,
    int DisplayOrder,
    IReadOnlyList<AboutTranslationDto> Translations)
{
    public static AboutResult FromEntity(About about)
    {
        return new AboutResult(
            about.Id,
            about.ImageUrl,
            about.IsActive,
            about.DisplayOrder,
            about.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new AboutTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Subtitle = x.Subtitle,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToList());
    }
}