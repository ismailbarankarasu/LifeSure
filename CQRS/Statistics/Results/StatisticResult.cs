using LifeSure.CQRS.Statistics.Models;
using LifeSure.Entities;
using LifeSure.Enums;

namespace LifeSure.CQRS.Statistics.Results;

public record StatisticResult(
    int Id,
    StatisticSource Source,
    int Value,
    int DisplayValue,
    string? Suffix,
    bool IsActive,
    int DisplayOrder,
    IReadOnlyList<StatisticTranslationDto> Translations)
{
    public static StatisticResult FromEntity(
        Statistic statistic,
        int displayValue)
    {
        return new StatisticResult(
            statistic.Id,
            statistic.Source,
            statistic.Value,
            displayValue,
            statistic.Suffix,
            statistic.IsActive,
            statistic.DisplayOrder,
            statistic.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new StatisticTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Title = x.Title
                })
                .ToList());
    }
}