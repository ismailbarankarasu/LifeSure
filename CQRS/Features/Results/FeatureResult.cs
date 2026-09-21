using LifeSure.CQRS.Features.Models;
using LifeSure.Entities;

namespace LifeSure.CQRS.Features.Results;

public record FeatureResult(int Id, string IconClass, bool IsActive, int DisplayOrder, IReadOnlyList<FeatureTranslationDto> Translations)
{
    public static FeatureResult FromEntity(Feature feature)
    {
        return new FeatureResult(
            feature.Id,
            feature.IconClass,
            feature.IsActive,
            feature.DisplayOrder,
            feature.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new FeatureTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToList());
    }
}