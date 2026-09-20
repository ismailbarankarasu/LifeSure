using LifeSure.CQRS.Sliders.Models;
using LifeSure.Entities;

namespace LifeSure.CQRS.Sliders.Results;

public record SliderResult(
    int Id,
    string ImageUrl,
    string? VideoId,
    string ButtonUrl,
    bool IsActive,
    int DisplayOrder,
    IReadOnlyList<SliderTranslationDto> Translations)
{
    public static SliderResult FromEntity(Slider slider)
    {
        return new SliderResult(
            slider.Id,
            slider.ImageUrl,
            slider.VideoId,
            slider.ButtonUrl,
            slider.IsActive,
            slider.DisplayOrder,
            slider.Translations
                .OrderBy(x => x.LanguageCode)
                .Select(x => new SliderTranslationDto
                {
                    LanguageCode = x.LanguageCode,
                    Subtitle = x.Subtitle,
                    Title = x.Title,
                    Description = x.Description,
                    ButtonText = x.ButtonText
                })
                .ToList());
    }
}