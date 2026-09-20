using LifeSure.CQRS.Abstractions;
using LifeSure.CQRS.Sliders.Commands;
using LifeSure.CQRS.Sliders.Models;
using LifeSure.Data;
using LifeSure.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeSure.CQRS.Sliders.Handlers;

public class UpdateSliderCommandHandler(LifeSureDbContext context)
    : ICommandHandler<UpdateSliderCommand, bool>
{
    public async Task<bool> HandleAsync(
        UpdateSliderCommand command,
        CancellationToken cancellationToken = default)
    {
        var input = command.Input;

        SliderInputValidator.Validate(input);

        var slider = await context.Sliders
            .Include(x => x.Translations)
            .SingleOrDefaultAsync(
                x => x.Id == command.Id,
                cancellationToken);

        if (slider is null)
        {
            return false;
        }

        slider.ImageUrl = input.ImageUrl;
        slider.VideoId = string.IsNullOrEmpty(input.VideoId)
            ? null
            : input.VideoId;
        slider.ButtonUrl = input.ButtonUrl;
        slider.IsActive = input.IsActive;
        slider.DisplayOrder = input.DisplayOrder;

        foreach (var item in input.Translations)
        {
            var translation = slider.Translations
                .SingleOrDefault(
                    x => x.LanguageCode == item.LanguageCode);

            if (translation is null)
            {
                translation = new SliderTranslation
                {
                    LanguageCode = item.LanguageCode
                };

                slider.Translations.Add(translation);
            }

            translation.Subtitle = item.Subtitle;
            translation.Title = item.Title;
            translation.Description = item.Description;
            translation.ButtonText = item.ButtonText;
        }

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}